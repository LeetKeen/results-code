using System;
using System.Collections.Generic;

namespace LeetKeen.Results;

/// <summary>
/// Provides extension methods for <see cref="IError"/>
/// and <see cref="IResult"/>.
/// </summary>
public static class ErrorExtensions
{
    /// <summary>
    /// Converts an <see cref="IError"/> to a failed
    /// <see cref="IResult{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="error">The error to convert.</param>
    /// <returns>A failed result containing the error.</returns>
    public static IResult<T> ToResult<T>(
        this IError error)
    {
        if (error == null)
            throw new ArgumentNullException(nameof(error));

        return Result.Fail<T>(error);
    }


    /// <summary>
    /// Converts an <see cref="IError"/> to a failed
    /// <see cref="IResult{T}"/> with a value.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="error">The error to convert.</param>
    /// <param name="value">The value to include in the result.</param>
    /// <returns>A failed result containing the error and value.</returns>
    public static IResult<T> ToResult<T>(
        this IError error,
        T value)
    {
        if (error == null)
            throw new ArgumentNullException(nameof(error));

        return Result.Fail(value, error);
    }


    /// <summary>
    /// Adds a data item to the error's <see cref="IError.Payload"/>.
    /// </summary>
    /// <param name="error">The error to add data to.</param>
    /// <param name="key">The key for the data item.</param>
    /// <param name="value">The value for the data item.</param>
    /// <param name="visibility">
    /// The visibility level.
    /// <c>0</c> = public (default); non-zero = sensitive.
    /// </param>
    /// <returns>The error with the added data item.</returns>
    public static TError WithData<TError>(
        this TError error,
        string key,
        object? value,
        int visibility = 0)
        where TError : IError
    {
        if (error == null)
            throw new ArgumentNullException(nameof(error));
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        error.Payload.Add(key, value, visibility);
        return error;
    }


    /// <summary>
    /// Adds a child error to the error's <see cref="IError.Children"/>.
    /// </summary>
    /// <param name="error">The error to add a child to.</param>
    /// <param name="child">The child error to add.</param>
    /// <returns>The error with the added child.</returns>
    public static TError WithChild<TError>(
        this TError error,
        IError child)
        where TError : IError
    {
        if (error == null)
            throw new ArgumentNullException(nameof(error));
        if (child == null)
            throw new ArgumentNullException(nameof(child));

        error.AddChild(child);
        return error;
    }


    /// <summary>
    /// Adds multiple child errors to the error's
    /// <see cref="IError.Children"/>.
    /// </summary>
    /// <param name="error">The error to add children to.</param>
    /// <param name="children">
    /// The child errors to add.
    /// Must not contain null elements.
    /// </param>
    /// <returns>The error with the added children.</returns>
    public static TError WithChildren<TError>(
        this TError error,
        IEnumerable<IError> children)
        where TError : IError
    {
        if (error == null)
            throw new ArgumentNullException(nameof(error));
        if (children == null)
            throw new ArgumentNullException(nameof(children));

        foreach (IError child in children)
        {
            error.AddChild(child);
        }

        return error;
    }
}
