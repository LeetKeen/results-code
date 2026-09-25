using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LeetKeen.Results;


/// <summary>
/// Provides async (Task-based) extension methods for <see cref="IResult"/> and 
/// <see cref="IResult{T}"/>.
/// 
/// These extensions are designed for fluent pipelines that start from
/// <see cref="Task{TResult}"/> where the
/// TResult is an <see cref="IResult"/> or <see cref="IResult{T}"/>.
/// </summary>
#pragma warning disable LK0001 // Async method missing CancellationToken
public static class ResultExtensionsAsync
{
    /// <summary>
    /// Maps the value of a successful result into a new value.
    /// If the awaited result is a failure, the errors are propagated unchanged.
    /// </summary>
    /// <typeparam name="TIn">The type of the input value.</typeparam>
    /// <typeparam name="TOut">The type of the mapped value.</typeparam>
    /// <param name="resultTask">A task that produces the result to map.</param>
    /// <param name="map">The mapping function.</param>
    /// <param name="configureAwait">
    /// Whether to marshal the continuation back to the captured context.
    /// Passed to <see cref="Task.ConfigureAwait(bool)"/>.
    /// </param>
    /// <returns>
    /// A task that produces a successful result containing the mapped value,
    /// or a failed result containing the original errors.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="resultTask"/> or <paramref name="map"/> is 
    /// <see langword="null"/>.
    /// </exception>
    public static async Task<IResult<TOut>> MapAsync<TIn, TOut>(
        this Task<IResult<TIn>> resultTask,
        Func<TIn, TOut> map,
        bool configureAwait = false)
    {
        if (resultTask == null) throw new ArgumentNullException(nameof(resultTask));
        if (map == null) throw new ArgumentNullException(nameof(map));

        var result = await resultTask.ConfigureAwait(configureAwait);
        if (result.IsFailure())
            return Result.Fail<TOut>(result.Errors);
        return Result.Ok(map(result.Value!));
    }


    /// <summary>
    /// Maps the value of a successful result into a new value using an asynchronous mapping function.
    /// If the awaited result is a failure, the errors are propagated unchanged.
    /// </summary>
    /// <typeparam name="TIn">The type of the input value.</typeparam>
    /// <typeparam name="TOut">The type of the mapped value.</typeparam>
    /// <param name="resultTask">A task that produces the result to map.</param>
    /// <param name="map">The asynchronous mapping function.</param>
    /// <param name="configureAwait">
    /// Whether to marshal the continuation back to the captured context.
    /// Passed to <see cref="Task.ConfigureAwait(bool)"/>.
    /// </param>
    /// <returns>
    /// A task that produces a successful result containing the mapped value,
    /// or a failed result containing the original errors.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="resultTask"/> or <paramref name="map"/> is <see langword="null"/>.
    /// </exception>
    public static async Task<IResult<TOut?>> MapAsync<TIn, TOut>(
        this Task<IResult<TIn?>> resultTask,
        Func<TIn?, Task<TOut?>> map,
        bool configureAwait = false)
    {
        if (resultTask == null) throw new ArgumentNullException(nameof(resultTask));
        if (map == null) throw new ArgumentNullException(nameof(map));

        var result = await resultTask.ConfigureAwait(configureAwait);
        if (result.IsFailure())
        {
            return Result.Fail<TOut>(result.Errors);
        }

        var mapped = await map(result.Value).ConfigureAwait(configureAwait);
        return Result.Ok(mapped);
    }


    /// <summary>
    /// Binds (flat-maps) the value of a successful result into another result.
    /// If the awaited result is a failure, the errors are propagated unchanged.
    /// </summary>
    /// <typeparam name="TIn">The type of the input value.</typeparam>
    /// <typeparam name="TOut">The type of the output result value.</typeparam>
    /// <param name="resultTask">A task that produces the result to bind.</param>
    /// <param name="bind">The binding function.</param>
    /// <param name="configureAwait">
    /// Whether to marshal the continuation back to the captured context.
    /// Passed to <see cref="Task.ConfigureAwait(bool)"/>.
    /// </param>
    /// <returns>
    /// A task that produces the result returned by <paramref name="bind"/>,
    /// or a failed result containing the original errors.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="resultTask"/> or <paramref name="bind"/> is <see langword="null"/>.
    /// </exception>
    public static async Task<IResult<TOut>> BindAsync<TIn, TOut>(
        this Task<IResult<TIn>> resultTask,
        Func<TIn, IResult<TOut>> bind,
        bool configureAwait = false)
    {
        if (resultTask == null) throw new ArgumentNullException(nameof(resultTask));
        if (bind == null) throw new ArgumentNullException(nameof(bind));

        var result = await resultTask.ConfigureAwait(configureAwait);
        return result.Bind(bind);
    }


    /// <summary>
    /// Binds (flat-maps) the value of a successful result into another result using an asynchronous binding function.
    /// If the awaited result is a failure, the errors are propagated unchanged.
    /// </summary>
    /// <typeparam name="TIn">The type of the input value.</typeparam>
    /// <typeparam name="TOut">The type of the output result value.</typeparam>
    /// <param name="resultTask">A task that produces the result to bind.</param>
    /// <param name="bind">The asynchronous binding function.</param>
    /// <param name="configureAwait">
    /// Whether to marshal the continuation back to the captured context.
    /// Passed to <see cref="Task.ConfigureAwait(bool)"/>.
    /// </param>
    /// <returns>
    /// A task that produces the result returned by <paramref name="bind"/>,
    /// or a failed result containing the original errors.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="resultTask"/> or <paramref name="bind"/> is <see langword="null"/>.
    /// </exception>
    public static async Task<IResult<TOut?>> BindAsync<TIn, TOut>(
        this Task<IResult<TIn?>> resultTask,
        Func<TIn?, Task<IResult<TOut?>>> bind,
        bool configureAwait = false)
    {
        if (resultTask == null) throw new ArgumentNullException(nameof(resultTask));
        if (bind == null) throw new ArgumentNullException(nameof(bind));

        var result = await resultTask.ConfigureAwait(configureAwait);
        if (result.IsFailure())
        {
            return Result.Fail<TOut>(result.Errors);
        }

        return await bind(result.Value).ConfigureAwait(configureAwait);
    }


    /// <summary>
    /// Runs a side-effect when the awaited result is successful, without changing the result.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="resultTask">A task that produces the result to tap.</param>
    /// <param name="onSuccess">The side-effect to execute when the result is successful.</param>
    /// <param name="configureAwait">
    /// Whether to marshal the continuation back to the captured context.
    /// Passed to <see cref="Task.ConfigureAwait(bool)"/>.
    /// </param>
    /// <returns>A task that produces the original result, unchanged.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="resultTask"/> or <paramref name="onSuccess"/> is <see langword="null"/>.
    /// </exception>
    public static async Task<IResult<T>> TapAsync<T>(
        this Task<IResult<T>> resultTask,
        Action<T> onSuccess,
        bool configureAwait = false)
    {
        if (resultTask == null) throw new ArgumentNullException(nameof(resultTask));
        if (onSuccess == null) throw new ArgumentNullException(nameof(onSuccess));

        var result = await resultTask.ConfigureAwait(configureAwait);
        return result.Tap(onSuccess);
    }


    /// <summary>
    /// Runs an asynchronous side-effect when the awaited result is successful, without changing the result.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="resultTask">A task that produces the result to tap.</param>
    /// <param name="onSuccess">The asynchronous side-effect to execute when the result is successful.</param>
    /// <param name="configureAwait">
    /// Whether to marshal the continuation back to the captured context.
    /// Passed to <see cref="Task.ConfigureAwait(bool)"/>.
    /// </param>
    /// <returns>A task that produces the original result, unchanged.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="resultTask"/> or <paramref name="onSuccess"/> is <see langword="null"/>.
    /// </exception>
    public static async Task<IResult<T?>> TapAsync<T>(
        this Task<IResult<T?>> resultTask,
        Func<T?, Task> onSuccess,
        bool configureAwait = false)
    {
        if (resultTask == null) throw new ArgumentNullException(nameof(resultTask));
        if (onSuccess == null) throw new ArgumentNullException(nameof(onSuccess));

        var result = await resultTask.ConfigureAwait(configureAwait);
        if (result.IsSuccess())
        {
            await onSuccess(result.Value).ConfigureAwait(configureAwait);
        }

        return result;
    }


    /// <summary>
    /// Runs a side-effect when the awaited result is a failure, without changing the result.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="resultTask">A task that produces the result to tap.</param>
    /// <param name="onFailure">The side-effect to execute when the result is a failure.</param>
    /// <param name="configureAwait">
    /// Whether to marshal the continuation back to the captured context.
    /// Passed to <see cref="Task.ConfigureAwait(bool)"/>.
    /// </param>
    /// <returns>A task that produces the original result, unchanged.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="resultTask"/> or <paramref name="onFailure"/> is <see langword="null"/>.
    /// </exception>
    public static async Task<IResult<T>> TapErrorAsync<T>(
        this Task<IResult<T>> resultTask,
        Action<IEnumerable<IError>> onFailure,
        bool configureAwait = false)
    {
        if (resultTask == null) throw new ArgumentNullException(nameof(resultTask));
        if (onFailure == null) throw new ArgumentNullException(nameof(onFailure));

        var result = await resultTask.ConfigureAwait(configureAwait);
        return result.TapError(onFailure);
    }


    /// <summary>
    /// Runs an asynchronous side-effect when the awaited result is a failure, without changing the result.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="resultTask">A task that produces the result to tap.</param>
    /// <param name="onFailure">The asynchronous side-effect to execute when the result is a failure.</param>
    /// <param name="configureAwait">
    /// Whether to marshal the continuation back to the captured context.
    /// Passed to <see cref="Task.ConfigureAwait(bool)"/>.
    /// </param>
    /// <returns>A task that produces the original result, unchanged.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="resultTask"/> or <paramref name="onFailure"/> is <see langword="null"/>.
    /// </exception>
    public static async Task<IResult<T>> TapErrorAsync<T>(
        this Task<IResult<T>> resultTask,
        Func<IEnumerable<IError>, Task> onFailure,
        bool configureAwait = false)
    {
        if (resultTask == null) throw new ArgumentNullException(nameof(resultTask));
        if (onFailure == null) throw new ArgumentNullException(nameof(onFailure));

        var result = await resultTask.ConfigureAwait(configureAwait);
        if (result.IsFailure())
        {
            await onFailure(result.Errors).ConfigureAwait(configureAwait);
        }

        return result;
    }


    /// <summary>
    /// Ensures a predicate holds for a successful result.
    /// If the predicate fails, converts the result into a failure using <paramref name="errorFactory"/>.
    /// If the awaited result is already a failure, it is returned unchanged.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="resultTask">A task that produces the result to validate.</param>
    /// <param name="predicate">A predicate applied to the successful value.</param>
    /// <param name="errorFactory">A function producing an error for the failing value.</param>
    /// <param name="configureAwait">
    /// Whether to marshal the continuation back to the captured context.
    /// Passed to <see cref="Task.ConfigureAwait(bool)"/>.
    /// </param>
    /// <returns>
    /// A task that produces the original result if it was a failure or the predicate succeeded;
    /// otherwise a failure result created from <paramref name="errorFactory"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="resultTask"/>, <paramref name="predicate"/>, or <paramref name="errorFactory"/> is <see langword="null"/>.
    /// </exception>
    public static async Task<IResult<T>> EnsureAsync<T>(
        this Task<IResult<T>> resultTask,
        Func<T, bool> predicate,
        Func<T, IError> errorFactory,
        bool configureAwait = false)
    {
        if (resultTask == null) throw new ArgumentNullException(nameof(resultTask));
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        if (errorFactory == null) throw new ArgumentNullException(nameof(errorFactory));

        var result = await resultTask.ConfigureAwait(configureAwait);
        return result.Ensure(predicate, errorFactory);
    }


    /// <summary>
    /// Ensures an asynchronous predicate holds for a successful result.
    /// If the predicate fails, converts the result into a failure using <paramref name="errorFactory"/>.
    /// If the awaited result is already a failure, it is returned unchanged.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="resultTask">A task that produces the result to validate.</param>
    /// <param name="predicate">An asynchronous predicate applied to the successful value.</param>
    /// <param name="errorFactory">A function producing an error for the failing value.</param>
    /// <param name="configureAwait">
    /// Whether to marshal the continuation back to the captured context.
    /// Passed to <see cref="Task.ConfigureAwait(bool)"/>.
    /// </param>
    /// <returns>
    /// A task that produces the original result if it was a failure or the predicate succeeded;
    /// otherwise a failure result created from <paramref name="errorFactory"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="resultTask"/>, <paramref name="predicate"/>, or <paramref name="errorFactory"/> is <see langword="null"/>.
    /// </exception>
    public static async Task<IResult<T?>> EnsureAsync<T>(
        this Task<IResult<T?>> resultTask,
        Func<T?, Task<bool>> predicate,
        Func<T?, IError> errorFactory,
        bool configureAwait = false)
    {
        if (resultTask == null) throw new ArgumentNullException(nameof(resultTask));
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        if (errorFactory == null) throw new ArgumentNullException(nameof(errorFactory));

        var result = await resultTask.ConfigureAwait(configureAwait);
        if (result.IsFailure())
        {
            return result;
        }

        if (await predicate(result.Value).ConfigureAwait(configureAwait))
        {
            return result;
        }

        return Result.Fail<T>(errorFactory(result.Value));
    }


    /// <summary>
    /// Maps errors when the awaited result is a failure.
    /// If the awaited result is a success, it is returned unchanged.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="resultTask">A task that produces the result whose errors should be mapped.</param>
    /// <param name="mapErrors">A function that maps the error collection.</param>
    /// <param name="configureAwait">
    /// Whether to marshal the continuation back to the captured context.
    /// Passed to <see cref="Task.ConfigureAwait(bool)"/>.
    /// </param>
    /// <returns>
    /// A task that produces the original result if it was successful;
    /// otherwise a new failed result containing the mapped errors.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="resultTask"/> or <paramref name="mapErrors"/> is <see langword="null"/>.
    /// </exception>
    public static async Task<IResult<T>> MapErrorAsync<T>(
        this Task<IResult<T>> resultTask,
        Func<IEnumerable<IError>, IEnumerable<IError>> mapErrors,
        bool configureAwait = false)
    {
        if (resultTask == null) throw new ArgumentNullException(nameof(resultTask));
        if (mapErrors == null) throw new ArgumentNullException(nameof(mapErrors));

        var result = await resultTask.ConfigureAwait(configureAwait);
        return result.MapError(mapErrors);
    }


    /// <summary>
    /// Maps errors when the awaited result is a failure into a single error.
    /// If the awaited result is a success, it is returned unchanged.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="resultTask">A task that produces the result whose errors should be mapped to a single error.</param>
    /// <param name="mapError">A function that maps the error collection into a single error.</param>
    /// <param name="configureAwait">
    /// Whether to marshal the continuation back to the captured context.
    /// Passed to <see cref="Task.ConfigureAwait(bool)"/>.
    /// </param>
    /// <returns>
    /// A task that produces the original result if it was successful;
    /// otherwise a new failed result containing the mapped errors.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="resultTask"/> or <paramref name="mapError"/> is <see langword="null"/>.
    /// </exception>
    public static async Task<IResult<T>> MapErrorAsync<T>(
        this Task<IResult<T>> resultTask,
        Func<IEnumerable<IError>, IError> mapError,
        bool configureAwait = false)
    {
        if (resultTask == null) throw new ArgumentNullException(nameof(resultTask));
        if (mapError == null) throw new ArgumentNullException(nameof(mapError));

        var result = await resultTask.ConfigureAwait(configureAwait);
        return result.MapError(mapError);
    }


    /// <summary>
    /// Recovers from failure by producing a fallback value.
    /// If the awaited result is a success, it is returned unchanged.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="resultTask">A task that produces the result to recover.</param>
    /// <param name="fallback">A function that produces a fallback value from the error collection.</param>
    /// <param name="configureAwait">
    /// Whether to marshal the continuation back to the captured context.
    /// Passed to <see cref="Task.ConfigureAwait(bool)"/>.
    /// </param>
    /// <returns>
    /// A task that produces the original result if it was successful;
    /// otherwise a new successful result containing the fallback value.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="resultTask"/> or <paramref name="fallback"/> is <see langword="null"/>.
    /// </exception>
    public static async Task<IResult<T>> RecoverAsync<T>(
        this Task<IResult<T>> resultTask,
        Func<IEnumerable<IError>, T> fallback,
        bool configureAwait = false)
    {
        if (resultTask == null) throw new ArgumentNullException(nameof(resultTask));
        if (fallback == null) throw new ArgumentNullException(nameof(fallback));

        var result = await resultTask.ConfigureAwait(configureAwait);
        return result.Recover(fallback);
    }


    /// <summary>
    /// Recovers from failure by producing a fallback value using an asynchronous fallback function.
    /// If the awaited result is a success, it is returned unchanged.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="resultTask">A task that produces the result to recover.</param>
    /// <param name="fallback">An asynchronous function that produces a fallback value from the error collection.</param>
    /// <param name="configureAwait">
    /// Whether to marshal the continuation back to the captured context.
    /// Passed to <see cref="Task.ConfigureAwait(bool)"/>.
    /// </param>
    /// <returns>
    /// A task that produces the original result if it was successful;
    /// otherwise a new successful result containing the fallback value.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="resultTask"/> or <paramref name="fallback"/> is <see langword="null"/>.
    /// </exception>
    public static async Task<IResult<T>> RecoverAsync<T>(
        this Task<IResult<T>> resultTask,
        Func<IEnumerable<IError>, Task<T>> fallback,
        bool configureAwait = false)
    {
        if (resultTask == null) throw new ArgumentNullException(nameof(resultTask));
        if (fallback == null) throw new ArgumentNullException(nameof(fallback));

        var result = await resultTask.ConfigureAwait(configureAwait);
        if (result.IsSuccess())
        {
            return result;
        }

        var recovered = await fallback(result.Errors).ConfigureAwait(configureAwait);
        return Result.Ok(recovered);
    }


    /// <summary>
    /// Provides an alternative result when the awaited result is a failure.
    /// If the awaited result is a success, it is returned unchanged.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="resultTask">A task that produces the result to recover.</param>
    /// <param name="fallback">A function that produces an alternative result from the error collection.</param>
    /// <param name="configureAwait">
    /// Whether to marshal the continuation back to the captured context.
    /// Passed to <see cref="Task.ConfigureAwait(bool)"/>.
    /// </param>
    /// <returns>
    /// A task that produces the original result if it was successful;
    /// otherwise the result returned by <paramref name="fallback"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="resultTask"/> or <paramref name="fallback"/> is <see langword="null"/>.
    /// </exception>
    public static async Task<IResult<T>> OrElseAsync<T>(
        this Task<IResult<T>> resultTask,
        Func<IEnumerable<IError>, IResult<T>> fallback,
        bool configureAwait = false)
    {
        if (resultTask == null) throw new ArgumentNullException(nameof(resultTask));
        if (fallback == null) throw new ArgumentNullException(nameof(fallback));

        var result = await resultTask.ConfigureAwait(configureAwait);
        return result.OrElse(fallback);
    }


    /// <summary>
    /// Provides an alternative result when the awaited result is a failure using an asynchronous fallback function.
    /// If the awaited result is a success, it is returned unchanged.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="resultTask">A task that produces the result to recover.</param>
    /// <param name="fallback">An asynchronous function that produces an alternative result from the error collection.</param>
    /// <param name="configureAwait">
    /// Whether to marshal the continuation back to the captured context.
    /// Passed to <see cref="Task.ConfigureAwait(bool)"/>.
    /// </param>
    /// <returns>
    /// A task that produces the original result if it was successful;
    /// otherwise the result returned by <paramref name="fallback"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="resultTask"/> or <paramref name="fallback"/> is <see langword="null"/>.
    /// </exception>
    public static async Task<IResult<T>> OrElseAsync<T>(
        this Task<IResult<T>> resultTask,
        Func<IEnumerable<IError>, Task<IResult<T>>> fallback,
        bool configureAwait = false)
    {
        if (resultTask == null) throw new ArgumentNullException(nameof(resultTask));
        if (fallback == null) throw new ArgumentNullException(nameof(fallback));

        var result = await resultTask.ConfigureAwait(configureAwait);
        if (result.IsSuccess())
        {
            return result;
        }

        return await fallback(result.Errors).ConfigureAwait(configureAwait);
    }


    /// <summary>
    /// Matches the awaited result and projects it into another value.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <typeparam name="TOut">The type of the projected value.</typeparam>
    /// <param name="resultTask">A task that produces the result to match.</param>
    /// <param name="onSuccess">A function executed when the result is successful.</param>
    /// <param name="onFailure">A function executed when the result is a failure.</param>
    /// <param name="configureAwait">
    /// Whether to marshal the continuation back to the captured context.
    /// Passed to <see cref="Task.ConfigureAwait(bool)"/>.
    /// </param>
    /// <returns>A task that produces the value returned by <paramref name="onSuccess"/> or <paramref name="onFailure"/>.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="resultTask"/>, <paramref name="onSuccess"/>, or <paramref name="onFailure"/> is <see langword="null"/>.
    /// </exception>
    public static async Task<TOut> MatchAsync<T, TOut>(
        this Task<IResult<T>> resultTask,
        Func<T, TOut> onSuccess,
        Func<IEnumerable<IError>, TOut> onFailure,
        bool configureAwait = false)
    {
        if (resultTask == null) throw new ArgumentNullException(nameof(resultTask));
        if (onSuccess == null) throw new ArgumentNullException(nameof(onSuccess));
        if (onFailure == null) throw new ArgumentNullException(nameof(onFailure));

        var result = await resultTask.ConfigureAwait(configureAwait);
        return result.Match(onSuccess, onFailure);
    }


    /// <summary>
    /// Matches the awaited result and projects it into another value using asynchronous match functions.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <typeparam name="TOut">The type of the projected value.</typeparam>
    /// <param name="resultTask">A task that produces the result to match.</param>
    /// <param name="onSuccess">An asynchronous function executed when the result is successful.</param>
    /// <param name="onFailure">An asynchronous function executed when the result is a failure.</param>
    /// <param name="configureAwait">
    /// Whether to marshal the continuation back to the captured context.
    /// Passed to <see cref="Task.ConfigureAwait(bool)"/>.
    /// </param>
    /// <returns>A task that produces the value returned by <paramref name="onSuccess"/> or <paramref name="onFailure"/>.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="resultTask"/>, <paramref name="onSuccess"/>, or <paramref name="onFailure"/> is <see langword="null"/>.
    /// </exception>
    public static async Task<TOut?> MatchAsync<T, TOut>(
        this Task<IResult<T?>> resultTask,
        Func<T?, Task<TOut>> onSuccess,
        Func<IEnumerable<IError>, Task<TOut?>> onFailure,
        bool configureAwait = false)
    {
        if (resultTask == null) throw new ArgumentNullException(nameof(resultTask));
        if (onSuccess == null) throw new ArgumentNullException(nameof(onSuccess));
        if (onFailure == null) throw new ArgumentNullException(nameof(onFailure));

        var result = await resultTask.ConfigureAwait(configureAwait);
        return result.IsSuccess()
            ? await onSuccess(result.Value).ConfigureAwait(configureAwait)
            : await onFailure(result.Errors).ConfigureAwait(configureAwait);
    }
}
#pragma warning restore LK0001 // Async method missing CancellationToken
