using System;
using System.Collections.Generic;

namespace LeetKeen.Results;

/// <summary>
/// Represents a result of an operation.
/// </summary>
public interface IResult
{
    /// <summary>
    /// Checks if the result is a success.
    /// </summary>
    /// <returns>
    /// <c>True</c> if the result is a success, otherwise <c>false</c>.
    /// </returns>
    bool IsSuccess();


    /// <summary>
    /// A collection of errors that occurred during the operation.
    /// </summary>
    IEnumerable<IError> Errors { get; }


    /// <summary>
    /// A collection of warnings associated with the operation.
    /// Warnings are informational and do not affect
    /// <see cref="IsSuccess"/>. May be present alongside errors.
    /// Never <c>null</c>; may be empty.
    /// </summary>
    IReadOnlyList<IWarning> Warnings { get; }


    /// <summary>
    /// Gets a type of <see cref="Value"/>, even when <see cref="Value"/>'s 
    /// value is <c>null</c>.
    /// </summary>
    /// <returns>
    /// The <see cref="Type"/> of the <see cref="Value"/> property, 
    /// even if <see cref="Value"/> is <c>null</c>.
    /// </returns>
    Type GetValueType();


    /// <summary>
    /// Adds an error to the result.
    /// </summary>
    /// <param name="error">
    /// An error to add to the result.
    /// </param>
    void AddError(IError error);


    /// <summary>
    /// A value of a successful result.
    /// </summary>
    object? Value { get; }
}


/// <summary>
/// Represents a result of an operation with a value 
/// of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">
/// The type of the result value.
/// </typeparam>
public interface IResult<out T> : IResult
{
    /// <summary>
    /// Gets the value of the result.
    /// </summary>
    new T? Value { get; }
}
