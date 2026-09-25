using Shouldly;

namespace LeetKeen.Results.UnitTests;


[TestClass]
public static class ResultExtensionsTests
{
    #region Demo functions for tests
    private static readonly IError DemoTestError = new Error
    {
        Text = "Failure"
    };


    private static IResult<int> DemoParseInt(string input)
    {
        if (int.TryParse(input, out var value))
        {
            return value.AsOkResult();
        }

        return new Error { Text = $"Not an int: '{input}'" }.ToResult<int>();
    }


    private static IResult<int> DemoEnsurePositive(int value)
    {
        if (value > 0)
        {
            return value.AsOkResult();
        }

        return new Error { Text = "Must be positive" }.ToResult<int>();
    }


    private static IResult<int> DemoDouble(int value)
        => (value * 2).AsOkResult();


    /// <summary>
    /// Formats the specified integer value as a boxed string: [value] and returns the result.
    /// </summary>
    /// <param name="value">The integer value to format inside square brackets.</param>
    /// <returns>An <see cref="IResult{string}"/> containing the formatted string representation of <paramref name="value"/>
    /// enclosed in square brackets.</returns>
    private static IResult<string> DemoFormatBoxed(int value)
        => $"[{value}]".AsOkResult();


    private static Task<IResult<int>> DemoParseIntAsync(string input)
        => Task.FromResult(DemoParseInt(input));


    private static Task<IResult<int>> DemoEnsurePositiveAsync(int value)
        => Task.FromResult(DemoEnsurePositive(value));


    private static Task<IResult<int>> DemoDoubleAsync(int value)
        => Task.FromResult(DemoDouble(value));


    private static Task<IResult<string>> DemoFormatBoxedAsync(int value)
        => Task.FromResult(DemoFormatBoxed(value));
    #endregion


    public class Map
    {
        [TestClass]
        public class Single
        {
            /// <summary>
            /// Successfully transforms a value.
            /// </summary>
            [TestMethod]
            public void Success()
            {
                // ARRANGE
                var result = 2.AsOkResult();

                // ACT
                IResult<string> mapped = result.Map<int, string>(x => $"{x} + {x}");

                // ASSERT
                mapped.IsSuccess().ShouldBeTrue();
                mapped.Value.ShouldBe("2 + 2");
            }


            [TestMethod]
            public void SuccessNoT()
            {
                // ACT
                var mapped = 2.AsOkResult().Map(() => Result.Ok(123));

                // ASSERT
                mapped.Value.ShouldBe(123);
            }


            /// <summary>
            /// Successfully transforms a value.
            /// </summary>
            [TestMethod]
            public async Task SuccessAsync()
            {
                // ARRANGE
                var taskResult = DemoParseIntAsync("2");

                // ACT
                var mapped = await taskResult.MapAsync<int, string>(x => $"{x} + {x}");

                // ASSERT
                mapped.IsSuccess().ShouldBeTrue();
                mapped.Value.ShouldBe("2 + 2");
            }


            [TestMethod]
            public void Failure()
            {
                // ARRANGE
                var result = DemoTestError.ToResult<int>();

                // ACT
                IResult<string> mapped = result.Map<int, string>(x => $"{x} * {x}");

                // ASSERT
                mapped.IsFailure().ShouldBeTrue();
                mapped.Errors.Count().ShouldBe(1);
                mapped.Errors.First().Text.ShouldBe("Failure");
            }


            [TestMethod]
            public void FailureNoT()
            {
                // ACT
                var mapped = 2.AsOkResult().Map(() => Result.Fail<int>(DemoTestError));

                // ASSERT
                mapped.IsFailure().ShouldBeTrue();
                mapped.Errors.First().Text.ShouldBe("Failure");
            }


            [TestMethod]
            public async Task FailureAsync()
            {
                // ARRANGE
                var taskResult = Task.FromResult(DemoTestError.ToResult<int>());

                // ACT
                IResult<string> mapped = await taskResult.MapAsync<int, string>(x => $"{x} * {x}");

                // ASSERT
                mapped.IsFailure().ShouldBeTrue();
                mapped.Errors.Count().ShouldBe(1);
                mapped.Errors.First().Text.ShouldBe("Failure");
            }

        }


        [TestClass]
        public class Chain
        {
            [TestMethod]
            public void Success()
            {
                // ARRANGE
                var f = DemoParseInt("2");

                // ACT
                var mapped = f
                    .Map(DemoEnsurePositive)
                    .Map(x => DemoDouble(
                        /*good demo of manual unwrapping because of Map's nature */
                        x.Value));

                // ASSERT
                mapped.IsSuccess().ShouldBeTrue();
                mapped.Value.Value.ShouldBe(4);
            }


            [TestMethod]
            public async Task SuccessAsync()
            {
                // ARRANGE
                var f = DemoParseIntAsync("2");

                // ACT
                var mapped = await f
                    .MapAsync(DemoEnsurePositiveAsync)
                    .MapAsync(x => DemoDoubleAsync(
                        /*good demo of manual unwrapping because of Map's nature */
                        x.Value));

                // ASSERT
                mapped.IsSuccess().ShouldBeTrue();
                mapped.Value.Value.ShouldBe(4);
            }


            [TestMethod]
            public void Failure()
            {
                /* This test shows not only how uncomfotable 
                 * it is to us Map for chained ops,
                 * but also how dangerous it is
                 */

                // ARRANGE
                var f = DemoParseInt("-2");

                // ACT
                var mapped = f.Map(DemoEnsurePositive);

                /* N.B. at this point we still can validate that 
                 * the call has failed. We still preserve the errors.
                 */
                mapped.Value.IsSuccess().ShouldBeFalse();

                /* N.B.: after the following point we will loose information 
                 * about errors because we forward only a value 
                 * (which holds a default value).
                 */
                mapped = mapped.Map(x => DemoDouble(x.Value));

                // ASSERT
                mapped.IsSuccess().ShouldBeTrue();

                /* At this point result should still contain errors
                 * but we have already lost this information */
                mapped.Value.IsSuccess().ShouldBeTrue();
            }


            [TestMethod]
            public async Task FailureAsync()
            {
                /* This test shows not only how uncomfotable 
                 * it is to us Map for chained ops,
                 * but also how dangerous it is
                 */

                // ARRANGE
                var f = DemoParseIntAsync("-2");

                // ACT
                var mapped1 = await f.MapAsync(DemoEnsurePositiveAsync);

                /* N.B. at this point we still can validate that 
                 * the call has failed. We still preserve the errors.
                 */
                mapped1.Value.IsSuccess().ShouldBeFalse();

                /* N.B.: after the following point we will loose information 
                 * about errors because we forward only a value 
                 * (which holds a default value).
                 */
                var mapped2 = mapped1.Map(x => DemoDoubleAsync(x.Value));

                // ASSERT
                mapped2.IsSuccess().ShouldBeTrue();

                /* At this point result should still contain errors
                 * but we have already lost this information 
                 * and we need to write kind-a-ugly code here
                 */
                (await mapped2.Value).IsSuccess().ShouldBeTrue();
            }
        }
    }


    public class Bind
    {
        [TestClass]
        public class Single
        {
            [TestMethod]
            public void Success()
            {
                // ARRANGE
                var result = 2.AsOkResult();

                // ACT
                var bound = result.Bind(x => Result.Ok(x * 2));

                // ASSERT
                bound.IsSuccess().ShouldBeTrue();
                bound.Value.ShouldBe(4);
            }


            [TestMethod]
            public void Failure()
            {
                // ARRANGE
                var result = DemoTestError.ToResult<int>();

                // ACT
                var bound = result.Bind(x => Result.Ok(x * 2));

                // ASSERT
                bound.IsSuccess().ShouldBeFalse();
                bound.Errors.Count().ShouldBe(1);
                bound.Errors.First().Text.ShouldBe("Failure");
            }


            [TestMethod]
            public async Task SuccessAsync()
            {
                // ARRANGE
                var result = DemoParseIntAsync("2");

                // ACT
                var bound = await result.BindAsync(DemoDoubleAsync);

                // ASSERT
                bound.IsSuccess().ShouldBeTrue();
                bound.Value.ShouldBe(4);
            }


            [TestMethod]
            public async Task FailureAsync()
            {
                // ARRANGE
                var result = DemoParseIntAsync("-2");

                // ACT
                var bound = await result.BindAsync(DemoEnsurePositive);

                // ASSERT
                bound.IsSuccess().ShouldBeFalse();
                bound.Errors.Count().ShouldBe(1);
                bound.Errors.First().Text.ShouldBe("Must be positive");
            }
        }


        [TestClass]
        public class Chain
        {
            [TestMethod]
            public void Success()
            {
                // ARRANGE
                var f = DemoParseInt("2");

                // ACT
                var bound = f
                    .Bind(DemoDouble)
                    .Bind(DemoEnsurePositive)
                    .Bind(DemoFormatBoxed);

                // ASSERT
                bound.IsSuccess().ShouldBeTrue();
                bound.Value.ShouldBe("[4]");
            }


            [TestMethod]
            public void Failure()
            {
                // ARRANGE
                var result = DemoParseInt("-2");

                // ACT
                var bound = result
                    .Bind(DemoDouble)
                    .Bind(DemoEnsurePositive)
                    .Bind(DemoFormatBoxed);

                // ASSERT
                bound.IsSuccess().ShouldBeFalse();
                bound.Errors.Count().ShouldBe(1);
                bound.Errors.First().Text.ShouldBe("Must be positive");
            }


            [TestMethod]
            public async Task SuccessAsync()
            {
                // ARRANGE
                var f = DemoParseIntAsync("2");

                // ACT
                var bound = await f
                    .BindAsync(DemoDouble)
                    .BindAsync(DemoEnsurePositiveAsync)
                    .BindAsync(DemoFormatBoxedAsync);

                // ASSERT
                bound.IsSuccess().ShouldBeTrue();
                bound.Value.ShouldBe("[4]");
            }


            [TestMethod]
            public async Task FailureAsync()
            {
                // ARRANGE
                var result = DemoParseIntAsync("-2");

                // ACT
                var bound = await result
                    .BindAsync(DemoDouble)
                    .BindAsync(DemoEnsurePositiveAsync)
                    .BindAsync(DemoFormatBoxedAsync);

                // ASSERT
                bound.IsSuccess().ShouldBeFalse();
                bound.Errors.Count().ShouldBe(1);
                bound.Errors.First().Text.ShouldBe("Must be positive");
            }
        }
    }


    public class MapError
    {
        [TestClass]
        public class ToMultipleErrors
        {
            [TestMethod]
            public void Failure_MapsErrors()
            {
                var result = DemoTestError.ToResult<int>();

                var mapped = result.MapError(errors
                    => errors.Select(e => new Error
                    {
                        Text = e.Text + "!"
                    }));

                mapped.IsFailure().ShouldBeTrue();
                mapped.Errors.First().Text.ShouldBe("Failure!");
            }
        }


        [TestClass]
        public class ToSingleErrors
        {
            [TestMethod]
            public void Failure_MapsErrors()
            {
                var result = DemoTestError.ToResult<int>();

                var mapped = result.MapError(errors
                    => new Error
                    {
                        Text = errors.First().Text + "!"
                    });

                mapped.IsFailure().ShouldBeTrue();
                mapped.Errors.First().Text.ShouldBe("Failure!");
            }
        }
    }


    [TestClass]
    public class Tap
    {
        [TestMethod]
        public void Success_RunsSideEffect()
        {
            var touched = 0;
            var result = 21.AsOkResult();

            var same = result.Tap(v => touched = v);

            same.ShouldBeSameAs(result);
            touched.ShouldBe(21);
        }


        [TestMethod]
        public void Failure_NoSideEffect()
        {
            var touched = 0;
            var result = Result.Fail<int>(
                DemoTestError);

            var same = result.Tap(v => touched = v);

            same.ShouldBeSameAs(result);
            touched.ShouldBe(0);
        }


        //TODO: add TapAsync tests here
    }


    //TODO: TapError tests


    [TestClass]
    public class Ensure
    {
        [TestMethod]
        public void Success_FailsWhenPredicateFalse()
        {
            var result = 2.AsOkResult();

            var ensured = result.Ensure(
                x => x > 10,
                x => new Error { Text = $"Too small: {x}" });

            ensured.IsFailure().ShouldBeTrue();
            ensured.Errors.First().Text.ShouldBe("Too small: 2");
        }


        //TODO: add async tests
    }


    [TestClass]
    public class Recover
    {
        [TestMethod]
        public void Recover_Failure_ProvidesFallbackValue()
        {
            var result = DemoTestError.ToResult<int>();

            var recovered = result.Recover(_ => 123);

            recovered.IsSuccess().ShouldBeTrue();
            recovered.Value.ShouldBe(123);
        }
    }


    [TestClass]
    public class Match
    {
        [TestMethod]
        public void ProjectsToValue()
        {
            var ok = 2.AsOkResult();
            var fail = DemoTestError.ToResult<int>();

            ok.Match(v => v.ToString(), _ => "bad").ShouldBe("2");
            fail.Match(v => v.ToString(), _ => "bad").ShouldBe("bad");
        }


        [TestMethod]
        public async Task ProjectsToValueAsync()
        {
            var ok = Task.FromResult(2.AsOkResult());
            var fail = Task.FromResult(DemoTestError.ToResult<int>());

            (await ok.MatchAsync(v => v.ToString(), _ => "bad")).ShouldBe("2");
            (await fail.MatchAsync(v => v.ToString(), _ => "bad")).ShouldBe("bad");
        }
    }
}
