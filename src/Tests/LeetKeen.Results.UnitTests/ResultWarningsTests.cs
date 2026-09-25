using System.Collections.Generic;
using System.Linq;
using Shouldly;

namespace LeetKeen.Results.UnitTests;

/// <summary>
/// Tests for <see cref="IResult.Warnings"/> on <see cref="Result{T}"/>
/// and the static <see cref="Result"/> factory overloads that accept
/// warnings.
/// </summary>
[TestClass]
public class ResultWarningsTests
{
    private static readonly IError SampleError = new Error
    {
        Text = "something went wrong"
    };

    private static readonly IWarning SampleWarning = new Warning(
        0,
        "low disk space");


    // ----------------------------------------------------------------
    // Warnings property — default state
    // ----------------------------------------------------------------

    /// <summary>
    /// Warnings is empty on a default-constructed Result.
    /// </summary>
    [TestMethod]
    public void Warnings_IsEmptyByDefault()
    {
        IResult<int> result = Result<int>.Ok(1);

        result.Warnings.ShouldBeEmpty();
    }


    /// <summary>
    /// Warnings is never null — not on success, not on failure.
    /// </summary>
    [TestMethod]
    public void Warnings_IsNeverNull()
    {
        IResult<int> ok = Result<int>.Ok(1);
        IResult<int> fail = Result<int>.Fail(SampleError);

        ok.Warnings.ShouldNotBeNull();
        fail.Warnings.ShouldNotBeNull();
    }


    // ----------------------------------------------------------------
    // Ok with warnings — Result<T> static factory
    // ----------------------------------------------------------------

    /// <summary>
    /// Ok result with warnings: IsSuccess is true and Warnings are present.
    /// </summary>
    [TestMethod]
    public void OkWithWarnings_IsSuccess_TrueAndWarningsPresent()
    {
        IWarning warning = new Warning(0, "deprecated API used");

        IResult<string> result = Result<string>.Ok(
            "hello",
            new[] { warning });

        result.IsSuccess().ShouldBeTrue();
        result.Warnings.Count.ShouldBe(1);
        result.Warnings[0].Text.ShouldBe("deprecated API used");
    }


    /// <summary>
    /// Warnings do not influence IsSuccess on an Ok result.
    /// </summary>
    [TestMethod]
    public void OkWithWarnings_IsSuccess_NotAffectedByWarnings()
    {
        IResult<int> withoutWarnings = Result<int>.Ok(1);
        IResult<int> withWarnings = Result<int>.Ok(
            1,
            new[] { SampleWarning });

        withoutWarnings.IsSuccess().ShouldBeTrue();
        withWarnings.IsSuccess().ShouldBeTrue();
    }


    // ----------------------------------------------------------------
    // Fail with warnings — Result<T> static factory
    // ----------------------------------------------------------------

    /// <summary>
    /// Fail result with warnings: IsSuccess is false and Warnings are present.
    /// </summary>
    [TestMethod]
    public void FailWithWarnings_IsSuccess_FalseAndWarningsPresent()
    {
        IWarning warning = new Warning(0, "retry limit reached");

        IResult<int> result = Result<int>.Fail(
            SampleError,
            new[] { warning });

        result.IsSuccess().ShouldBeFalse();
        result.Warnings.Count.ShouldBe(1);
        result.Warnings[0].Text.ShouldBe("retry limit reached");
    }


    /// <summary>
    /// Warnings do not influence IsSuccess on a Fail result.
    /// </summary>
    [TestMethod]
    public void FailWithWarnings_IsSuccess_NotAffectedByWarnings()
    {
        IResult<int> withoutWarnings = Result<int>.Fail(SampleError);
        IResult<int> withWarnings = Result<int>.Fail(
            SampleError,
            new[] { SampleWarning });

        withoutWarnings.IsSuccess().ShouldBeFalse();
        withWarnings.IsSuccess().ShouldBeFalse();
    }


    // ----------------------------------------------------------------
    // Errors and Warnings can coexist
    // ----------------------------------------------------------------

    /// <summary>
    /// A result may carry both errors and warnings simultaneously.
    /// </summary>
    [TestMethod]
    public void ErrorsAndWarnings_CanBePresentSimultaneously()
    {
        IResult<int> result = Result<int>.Fail(
            SampleError,
            new[] { SampleWarning });

        result.Errors.Count().ShouldBe(1);
        result.Warnings.Count.ShouldBe(1);
    }


    // ----------------------------------------------------------------
    // Warnings accessible via IResult (non-generic interface)
    // ----------------------------------------------------------------

    /// <summary>
    /// Warnings is accessible through the non-generic IResult interface.
    /// </summary>
    [TestMethod]
    public void Warnings_AccessibleViaIResult()
    {
        IResult result = Result.Ok(new[] { SampleWarning });

        result.Warnings.Count.ShouldBe(1);
        result.Warnings[0].Text.ShouldBe("low disk space");
    }


    // ----------------------------------------------------------------
    // Static Result factory overloads
    // ----------------------------------------------------------------

    /// <summary>
    /// Result.Ok(warnings) — non-generic: IsSuccess is true,
    /// Warnings are present.
    /// </summary>
    [TestMethod]
    public void StaticResult_Ok_WithWarnings_IsSuccessTrue()
    {
        IWarning warning = new Warning(0, "cache miss");

        IResult result = Result.Ok(new[] { warning });

        result.IsSuccess().ShouldBeTrue();
        result.Warnings.Count.ShouldBe(1);
        result.Warnings[0].Text.ShouldBe("cache miss");
    }


    /// <summary>
    /// Result.Ok&lt;T&gt;(value, warnings) — generic: IsSuccess is true,
    /// Warnings are present.
    /// </summary>
    [TestMethod]
    public void StaticResult_OkGeneric_WithWarnings_IsSuccessTrue()
    {
        IWarning warning = new Warning(0, "fallback used");

        IResult<int> result = Result.Ok<int>(42, new[] { warning });

        result.IsSuccess().ShouldBeTrue();
        result.Value.ShouldBe(42);
        result.Warnings.Count.ShouldBe(1);
        result.Warnings[0].Text.ShouldBe("fallback used");
    }


    /// <summary>
    /// Result.Fail(error, warnings) — non-generic: IsSuccess is false,
    /// Warnings are present.
    /// </summary>
    [TestMethod]
    public void StaticResult_Fail_WithWarnings_IsSuccessFalse()
    {
        IWarning warning = new Warning(0, "partial data returned");

        IResult result = Result.Fail(SampleError, new[] { warning });

        result.IsSuccess().ShouldBeFalse();
        result.Warnings.Count.ShouldBe(1);
        result.Warnings[0].Text.ShouldBe("partial data returned");
    }


    /// <summary>
    /// Result.Fail&lt;T&gt;(error, warnings) — generic: IsSuccess is false,
    /// Warnings are present.
    /// </summary>
    [TestMethod]
    public void StaticResult_FailGeneric_WithWarnings_IsSuccessFalse()
    {
        IWarning warning = new Warning(0, "stale data");

        IResult<string> result = Result.Fail<string>(
            SampleError,
            new[] { warning });

        result.IsSuccess().ShouldBeFalse();
        result.Warnings.Count.ShouldBe(1);
        result.Warnings[0].Text.ShouldBe("stale data");
    }


    /// <summary>
    /// Result.Fail&lt;T&gt;(value, error, warnings): IsSuccess is false,
    /// value and warnings are set.
    /// </summary>
    [TestMethod]
    public void StaticResult_FailGenericWithValue_WithWarnings()
    {
        IWarning warning = new Warning(0, "used cached value");

        IResult<int> result = Result.Fail<int>(
            99,
            SampleError,
            new[] { warning });

        result.IsSuccess().ShouldBeFalse();
        result.Value.ShouldBe(99);
        result.Warnings.Count.ShouldBe(1);
        result.Warnings[0].Text.ShouldBe("used cached value");
    }


    // ----------------------------------------------------------------
    // Result<T> constructor with warnings
    // ----------------------------------------------------------------

    /// <summary>
    /// Constructor Result(T value, IEnumerable&lt;IWarning&gt;) stores warnings.
    /// </summary>
    [TestMethod]
    public void Constructor_WithValueAndWarnings_StoresWarnings()
    {
        IWarning w1 = new Warning(0, "first");
        IWarning w2 = new Warning(0, "second");

        Result<string> result = new Result<string>(
            "ok",
            new[] { w1, w2 });

        result.IsSuccess().ShouldBeTrue();
        result.Value.ShouldBe("ok");
        result.Warnings.Count.ShouldBe(2);
        result.Warnings[0].Text.ShouldBe("first");
        result.Warnings[1].Text.ShouldBe("second");
    }


    // ----------------------------------------------------------------
    // Multiple warnings
    // ----------------------------------------------------------------

    /// <summary>
    /// Multiple warnings can be passed and all are stored.
    /// </summary>
    [TestMethod]
    public void MultipleWarnings_AllStored()
    {
        IList<IWarning> warnings = new List<IWarning>
        {
            new Warning(0, "warning one"),
            new Warning(1, "warning two"),
            new Warning(2, "warning three"),
        };

        IResult<int> result = Result<int>.Ok(1, warnings);

        result.Warnings.Count.ShouldBe(3);
        result.Warnings.Select(w => w.Text).ShouldBe(
            new[] { "warning one", "warning two", "warning three" },
            ignoreOrder: false);
    }
}
