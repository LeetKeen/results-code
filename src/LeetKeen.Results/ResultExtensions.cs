using System;
using System.Collections.Generic;

namespace LeetKeen.Results;

/// <summary>
/// Provides extension methods for <see cref="IResult"/>.
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Checks if the result is a failure.
    /// </summary>
    /// <param name="result">
    /// A result to check.
    /// </param>
    /// <returns>
    /// <c>True</c> if the result is a failure, otherwise <c>false</c>.
    /// </returns>
    public static bool IsFailure(this IResult result)
    {
        if (result == null) throw new ArgumentNullException(nameof(result));
        return !result.IsSuccess();
    }


    /// <summary>
    /// Checks if the result is a failure. (Obsolete,
    /// use <see cref="IsFailure(IResult)"/> instead.)
    /// </summary>
    /// <param name="result">
    /// A result to check.
    /// </param>
    /// <returns>
    /// <c>True</c> if the result is a failure, otherwise <c>false</c>.
    /// </returns>
    [Obsolete($"Use {nameof(IsFailure)} instead.")]
    public static bool IsFailed(this IResult result)
    {
        if (result == null) throw new ArgumentNullException(nameof(result));
        return !result.IsSuccess();
    }


    /// <summary>
    /// Wraps a value in a successful <see cref="Result{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="value">The value to wrap.</param>
    /// <returns>A successful result containing the value.</returns>
    public static IResult<T> AsOkResult<T>(
        this T value)
    {
        var rv = new Result<T>(value);
        return rv;
    }


    /// <summary>
    /// Maps the value of a successful result to a new value.
    /// </summary>
    /// <typeparam name="TIn">
    /// The type of the input value.
    /// </typeparam>
    /// <typeparam name="TOut">
    /// The type of the output value.
    /// </typeparam>
    /// <param name="result">
    /// The result to map.
    /// </param>
    /// <param name="map">
    /// The mapping function.
    /// </param>
    /// <returns>
    /// A new result containing the mapped value or the original errors.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="result"/> or <paramref name="map"/> is null.
    /// </exception>
    public static IResult<TOut?> Map<TIn, TOut>(
        this IResult<TIn?> result,
        Func<TIn?, TOut?> map)
    {
        if (result == null) throw new ArgumentNullException(nameof(result));
        if (map == null) throw new ArgumentNullException(nameof(map));

        if (result.IsFailure())
        {
            return Result.Fail<TOut>(result.Errors);
        }

        return Result.Ok(map(result.Value));
    }


    /// <summary>
    /// Maps the value of a successful result to a new result.
    /// </summary>
    /// <param name="result">
    /// The result to map.
    /// </param>
    /// <param name="next">
    /// A function that produces the next result.
    /// </param>
    /// <returns>
    /// A new result containing the mapped value or the original errors.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="result"/> or <paramref name="next"/> is null.
    /// </exception>
    public static IResult Map(
        this IResult result,
        Func<IResult> next)
    {
        if (result == null) throw new ArgumentNullException(nameof(result));
        if (next == null) throw new ArgumentNullException(nameof(next));

        if (result.IsFailure())
        {
            return Result.Fail(result.Errors);
        }

        return next();
    }


    /// <summary>
    /// Binds the value of a successful result to a new result.
    /// </summary>
    /// <typeparam name="TIn">
    /// The type of the input value.
    /// </typeparam>
    /// <typeparam name="TOut">
    /// The type of the output value.
    /// </typeparam>
    /// <param name="result">
    /// The result to bind.
    /// </param>
    /// <param name="bind">
    /// The binding function.
    /// </param>
    /// <returns>
    /// A new result containing the bound value or the original errors.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="result"/> or <paramref name="bind"/> is null.
    /// </exception>
    public static IResult<TOut> Bind<TIn, TOut>(
        this IResult<TIn> result,
        Func<TIn, IResult<TOut>> bind)
    {
        if (result == null) throw new ArgumentNullException(nameof(result));
        if (bind == null) throw new ArgumentNullException(nameof(bind));

        if (result.IsFailure())
        {
            return Result.Fail<TOut>(result.Errors);
        }

        return bind(result.Value!);
    }


    /// <summary>
    /// Binds the value of a successful result to a new result.
    /// </summary>
    /// <param name="result">
    /// The result to bind.
    /// </param>
    /// <param name="bind">
    /// The binding function.
    /// </param>
    /// <returns>
    /// A new result containing the bound value or the original errors.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="result"/> or <paramref name="bind"/> is null.
    /// </exception>
    public static IResult Bind(
        this IResult result,
        Func<IResult> bind)
    {
        if (result == null) throw new ArgumentNullException(nameof(result));
        if (bind == null) throw new ArgumentNullException(nameof(bind));

        if (result.IsFailure())
        {
            return Result.Fail(result.Errors);
        }

        return bind();
    }


    /// <summary>
    /// Runs the specified action if the result is successful.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the result value.
    /// </typeparam>
    /// <param name="result">
    /// The result to tap.
    /// </param>
    /// <param name="onSuccess">
    /// The action to run if the result is successful.
    /// </param>
    /// <returns>
    /// The original result.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="result"/> or <paramref name="onSuccess"/> is null.
    /// </exception>
    public static IResult<T> Tap<T>(
        this IResult<T> result,
        Action<T> onSuccess)
    {
        if (result == null) throw new ArgumentNullException(nameof(result));
        if (onSuccess == null) throw new ArgumentNullException(nameof(onSuccess));

        if (result.IsSuccess())
        {
            onSuccess(result.Value!);
        }

        return result;
    }


    /// <summary>
    /// Runs the specified action if the result is successful.
    /// </summary>
    /// <param name="result">
    /// The result to tap.
    /// </param>
    /// <param name="onSuccess">
    /// The action to run if the result is successful.
    /// </param>
    /// <returns>
    /// The original result.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="result"/> or <paramref name="onSuccess"/> is null.
    /// </exception>
    public static IResult Tap(
        this IResult result,
        Action onSuccess)
    {
        if (result == null) throw new ArgumentNullException(nameof(result));
        if (onSuccess == null) throw new ArgumentNullException(nameof(onSuccess));

        if (result.IsSuccess())
        {
            onSuccess();
        }

        return result;
    }


    /// <summary>
    /// Runs the specified action if the result is a failure.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the result value.
    /// </typeparam>
    /// <param name="result">
    /// The result to tap.
    /// </param>
    /// <param name="onFailure">
    /// The action to run if the result is a failure.
    /// </param>
    /// <returns>
    /// The original result.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="result"/> or <paramref name="onFailure"/> is null.
    /// </exception>
    public static IResult<T> TapError<T>(
        this IResult<T> result,
        Action<IEnumerable<IError>> onFailure)
    {
        if (result == null) throw new ArgumentNullException(nameof(result));
        if (onFailure == null) throw new ArgumentNullException(nameof(onFailure));

        if (result.IsFailure())
        {
            onFailure(result.Errors);
        }

        return result;
    }


    /// <summary>
    /// Runs the specified action if the result is a failure.
    /// </summary>
    /// <param name="result">
    /// The result to tap.
    /// </param>
    /// <param name="onFailure">
    /// The action to run if the result is a failure.
    /// </param>
    /// <returns>
    /// The original result.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="result"/> or <paramref name="onFailure"/> is null.
    /// </exception>
    public static IResult TapError(
        this IResult result,
        Action<System.Collections.Generic.IEnumerable<IError>> onFailure)
    {
        if (result == null) throw new ArgumentNullException(nameof(result));
        if (onFailure == null) throw new ArgumentNullException(nameof(onFailure));

        if (result.IsFailure())
        {
            onFailure(result.Errors);
        }

        return result;
    }


    /// <summary>
    /// Ensures that the result's value satisfies the given predicate.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the result value.
    /// </typeparam>
    /// <param name="result">
    /// The result to ensure the predicate on.
    /// </param>
    /// <param name="predicate">
    /// The predicate to test the result's value.
    /// </param>
    /// <param name="errorFactory">
    /// A function that creates an error if the predicate is not satisfied.
    /// </param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="result"/>, <paramref name="predicate"/>, 
    /// or <paramref name="errorFactory"/> is null.
    /// </exception>
    public static IResult<T> Ensure<T>(
        this IResult<T> result,
        Func<T, bool> predicate,
        Func<T, IError> errorFactory)
    {
        if (result == null) throw new ArgumentNullException(nameof(result));
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        if (errorFactory == null) throw new ArgumentNullException(nameof(errorFactory));

        if (result.IsFailure())
        {
            return result;
        }

        if (predicate(result.Value!))
        {
            return result;
        }

        return Result.Fail<T>(errorFactory(result.Value!));
    }


    /// <summary>
    /// Maps the errors of a failed result to new list of errors.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the result value.
    /// </typeparam>
    /// <param name="result">
    /// The result to map errors on.
    /// </param>
    /// <param name="mapErrors">
    /// A function that maps the existing errors to new list of errors.
    /// </param>
    /// <returns>
    /// A new result with the mapped errors or the original successful result.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="result"/> or <paramref name="mapErrors"/> is null.
    /// </exception>
    public static IResult<T> MapError<T>(
        this IResult<T> result,
        Func<IEnumerable<IError>, IEnumerable<IError>> mapErrors)
    {
        if (result == null) throw new ArgumentNullException(nameof(result));
        if (mapErrors == null) throw new ArgumentNullException(nameof(mapErrors));

        if (result.IsSuccess())
        {
            return result;
        }

        return Result.Fail<T>(mapErrors(result.Errors));
    }


    /// <summary>
    /// Maps the errors of a failed result to a new error.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the result value.
    /// </typeparam>
    /// <param name="result">
    /// The result to map errors on.
    /// </param>
    /// <param name="mapError">
    /// A function that maps the existing errors to a new error.
    /// </param>
    /// <returns>
    /// A new result with the mapped error or the original successful result.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="result"/> or <paramref name="mapError"/> is null.
    /// </exception>
    public static IResult<T> MapError<T>(
        this IResult<T> result,
        Func<IEnumerable<IError>, IError> mapError)
    {
        if (result == null) throw new ArgumentNullException(nameof(result));
        if (mapError == null) throw new ArgumentNullException(nameof(mapError));

        if (result.IsSuccess())
        {
            return result;
        }

        return Result.Fail<T>(mapError(result.Errors));
    }


    /// <summary>
    /// Maps the errors of a failed result to new errors.
    /// </summary>
    /// <param name="result">
    /// The result to map errors on.
    /// </param>
    /// <param name="mapErrors">
    /// A function that maps the existing errors to new errors.
    /// </param>
    /// <returns>
    /// A new result with the mapped errors or the original successful result.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="result"/> or <paramref name="mapErrors"/> is null.
    /// </exception>
    public static IResult MapError(
        this IResult result,
        Func<IEnumerable<IError>, IEnumerable<IError>> mapErrors)
    {
        if (result == null) throw new ArgumentNullException(nameof(result));
        if (mapErrors == null) throw new ArgumentNullException(nameof(mapErrors));

        if (result.IsSuccess())
        {
            return result;
        }

        return Result.Fail(mapErrors(result.Errors));
    }


    /// <summary>
    /// Recovers a failed result by providing a fallback value.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the result value.
    /// </typeparam>
    /// <param name="result">
    /// The result to recover.
    /// </param>
    /// <param name="fallback">
    /// A function that provides a fallback value based on the errors.
    /// </param>
    /// <returns>
    /// A successful result with the original value or the fallback value.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="result"/> or <paramref name="fallback"/> is null.
    /// </exception>
    public static IResult<T> Recover<T>(
        this IResult<T> result,
        Func<IEnumerable<IError>, T> fallback)
    {
        if (result == null) throw new ArgumentNullException(nameof(result));
        if (fallback == null) throw new ArgumentNullException(nameof(fallback));

        if (result.IsSuccess())
        {
            return result;
        }

        return Result.Ok(fallback(result.Errors));
    }


    /// <summary>
    /// Provides an alternative result if the original result is a failure.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="result"></param>
    /// <param name="fallback"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static IResult<T> OrElse<T>(
        this IResult<T> result,
        Func<IEnumerable<IError>, IResult<T>> fallback)
    {
        if (result == null) throw new ArgumentNullException(nameof(result));
        if (fallback == null) throw new ArgumentNullException(nameof(fallback));

        if (result.IsSuccess())
        {
            return result;
        }

        return fallback(result.Errors);
    }


    /// <summary>
    /// Matches the result and invokes the appropriate function based on its state.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the result value.
    /// </typeparam>
    /// <typeparam name="TOut">
    /// The type of the output value.
    /// </typeparam>
    /// <param name="result">
    /// The result to match.
    /// </param>
    /// <param name="onSuccess">
    /// The function to invoke if the result is successful.
    /// </param>
    /// <param name="onFailure">
    /// The function to invoke if the result is a failure.
    /// </param>
    /// <returns>
    /// The output of the invoked function.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="result"/>, <paramref name="onSuccess"/>,
    /// </exception>
    public static TOut Match<T, TOut>(
        this IResult<T> result,
        Func<T, TOut> onSuccess,
        Func<IEnumerable<IError>, TOut> onFailure)
    {
        if (result == null) throw new ArgumentNullException(nameof(result));
        if (onSuccess == null) throw new ArgumentNullException(nameof(onSuccess));
        if (onFailure == null) throw new ArgumentNullException(nameof(onFailure));

        return result.IsSuccess()
            ? onSuccess(result.Value!)
            : onFailure(result.Errors);
    }


    /// <summary>
    /// Matches the result and invokes the appropriate function based on its state.
    /// </summary>
    /// <typeparam name="TOut">
    /// The type of the output value.
    /// </typeparam>
    /// <param name="result">
    /// The result to match.
    /// </param>
    /// <param name="onSuccess">
    /// The function to invoke if the result is successful.
    /// </param>
    /// <param name="onFailure">
    /// The function to invoke if the result is a failure.
    /// </param>
    /// <returns>
    /// The output of the invoked function.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="result"/>, <paramref name="onSuccess"/>,
    /// or <paramref name="onFailure"/> is null.
    /// </exception>
    public static TOut Match<TOut>(
        this IResult result,
        Func<TOut> onSuccess,
        Func<IEnumerable<IError>, TOut> onFailure)
    {
        if (result == null) throw new ArgumentNullException(nameof(result));
        if (onSuccess == null) throw new ArgumentNullException(nameof(onSuccess));
        if (onFailure == null) throw new ArgumentNullException(nameof(onFailure));

        return result.IsSuccess()
            ? onSuccess()
            : onFailure(result.Errors);
    }


    /// <summary>
    /// Maps the value of a successful result to a new value.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the output value.
    /// </typeparam>
    /// <param name="result">
    /// The result to map.
    /// </param>
    /// <param name="map">
    /// The mapping function.
    /// </param>
    /// <returns>
    /// A new result containing the mapped value or the original errors.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="result"/> or <paramref name="map"/> is null.
    /// </exception>
    public static IResult<T> Map<T>(
        this IResult result,
        Func<object, T> map)
    {
        if (result == null) throw new ArgumentNullException(nameof(result));
        if (map == null) throw new ArgumentNullException(nameof(map));

        if (result.IsFailure())
        {
            return Result.Fail<T>(result.Errors);
        }

        return Result.Ok(map(result.Value!));
    }


    /// <summary>
    /// Binds the value of a successful result to a new result.
    /// </summary>
    /// <typeparam name="TOut">
    /// The type of the output value.
    /// </typeparam>
    /// <param name="result">
    /// The result to bind.
    /// </param>
    /// <param name="bind">
    /// The binding function.
    /// </param>
    /// <returns>
    /// A new result containing the bound value or the original errors.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="result"/> or <paramref name="bind"/> is null.
    /// </exception>
    public static IResult<TOut> Bind<TOut>(
        this IResult result,
        Func<object, IResult<TOut>> bind)
    {
        if (result == null) throw new ArgumentNullException(nameof(result));
        if (bind == null) throw new ArgumentNullException(nameof(bind));

        if (result.IsFailure())
        {
            return Result.Fail<TOut>(result.Errors);
        }

        return bind(result.Value!);
    }


    /// <summary>
    /// Ensures that the result's value satisfies the given predicate.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the result value.
    /// </typeparam>
    /// <param name="result">
    /// The result to ensure the predicate on.
    /// </param>
    /// <param name="predicate">
    /// The predicate to test the result's value.
    /// </param>
    /// <param name="errorFactory">
    /// A function that creates an error if the predicate is not satisfied.
    /// </param>
    /// <returns>
    /// A new result containing the original value or an error if the predicate
    /// is not satisfied.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="result"/>, <paramref name="predicate"/>,
    /// or <paramref name="errorFactory"/> is null.
    /// </exception>
    public static IResult<T> Ensure<T>(
        this IResult result,
        Func<object?, bool> predicate,
        Func<object?, IError> errorFactory)
    {
        if (result == null) throw new ArgumentNullException(nameof(result));
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        if (errorFactory == null) throw new ArgumentNullException(nameof(errorFactory));

        if (result.IsFailure())
        {
            return Result.Fail<T>(result.Errors);
        }

        if (predicate(result.Value))
        {
            return Result.Ok((T)result.Value!);
        }

        return Result.Fail<T>(errorFactory(result.Value));
    }


    /// <summary>
    /// Recovers a failed result by providing a fallback value.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the result value.
    /// </typeparam>
    /// <param name="result">
    /// The result to recover.
    /// </param>
    /// <param name="fallback">
    /// A function that provides a fallback value based on the errors.
    /// </param>
    /// <returns>
    /// A successful result with the original value or the fallback value.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="result"/> or <paramref name="fallback"/> is null.
    /// </exception>
    public static IResult<T> Recover<T>(
        this IResult result,
        Func<System.Collections.Generic.IEnumerable<IError>, T> fallback)
    {
        if (result == null) throw new ArgumentNullException(nameof(result));
        if (fallback == null) throw new ArgumentNullException(nameof(fallback));

        if (result.IsSuccess())
        {
            return Result.Ok((T)result.Value!);
        }

        return Result.Ok(fallback(result.Errors));
    }


    /// <summary>
    /// Provides an alternative result if the original result is a failure.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the result value.
    /// </typeparam>
    /// <param name="result">
    /// The result to recover.
    /// </param>
    /// <param name="fallback">
    /// A function that provides a fallback result based on the errors.
    /// </param>
    /// <returns>
    /// A successful result with the original value or the fallback result.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="result"/> or <paramref name="fallback"/> is null.
    /// </exception>
    public static IResult<T> OrElse<T>(
        this IResult result,
        Func<System.Collections.Generic.IEnumerable<IError>, IResult<T>> fallback)
    {
        if (result == null) throw new ArgumentNullException(nameof(result));
        if (fallback == null) throw new ArgumentNullException(nameof(fallback));

        if (result.IsSuccess())
        {
            return Result.Ok((T)result.Value!);
        }

        return fallback(result.Errors);
    }


    /// <summary>
    /// Matches the result and invokes the appropriate function based on its state.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the result value.
    /// </typeparam>
    /// <typeparam name="TOut">
    /// The type of the output value.
    /// </typeparam>
    /// <param name="result">
    /// The result to match.
    /// </param>
    /// <param name="onSuccess">
    /// The function to invoke if the result is successful.
    /// </param>
    /// <param name="onFailure">
    /// The function to invoke if the result is a failure.
    /// </param>
    /// <returns>
    /// The output of the invoked function.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="result"/>, <paramref name="onSuccess"/>,
    /// or <paramref name="onFailure"/> is null.
    /// </exception>
    public static TOut? Match<T, TOut>(
        this IResult result,
        Func<object?, TOut?> onSuccess,
        Func<IEnumerable<IError>, TOut?> onFailure)
    {
        if (result == null) throw new ArgumentNullException(nameof(result));
        if (onSuccess == null) throw new ArgumentNullException(nameof(onSuccess));
        if (onFailure == null) throw new ArgumentNullException(nameof(onFailure));

        return result.IsSuccess()
            ? onSuccess(result.Value)
            : onFailure(result.Errors);
    }
}
