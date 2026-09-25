using System;
using System.Collections.Generic;
using System.Linq;

namespace LeetKeen.Results;

/// <summary>
/// Represents the result of an operation, containing a value and a collection 
/// of errors.
/// </summary>
public class Result<T>
    : IResult<T>
{
    private readonly T? value;
    private readonly List<IError> errors = [];
    private readonly List<IWarning> warnings = [];


    /// <summary>
    /// Gets the value of the result.
    /// </summary>
    public T? Value
    {
        get
        {
            return value;
        }
    }


    /// <summary>
    /// The minimum standard error code for errors.
    /// </summary>
    public static int MinStandardErrorCode => -2000;


    /// <summary>
    /// The maximum standard error code for errors.
    /// </summary>
    public static readonly int MaxStandardErrorCode = -1;


    /// <summary>
    /// Gets the collection of errors associated with the result.
    /// </summary>
    public IEnumerable<IError> Errors => errors;


    /// <summary>
    /// Gets the collection of warnings associated with the result.
    /// Never <c>null</c>; may be empty.
    /// </summary>
    public IReadOnlyList<IWarning> Warnings => warnings;


    object? IResult.Value => value;


    /// <summary>
    /// Adds an error to the result.
    /// </summary>
    /// <param name="error">The error to add.</param>
    public void AddError(IError error)
    {
        // TODO: throw if error is null
        errors.Add(error);
    }


    internal void InitWarnings(IEnumerable<IWarning>? source)
    {
        if (source == null) return;
        foreach (IWarning w in source)
        {
            if (w != null) warnings.Add(w);
        }
    }


    /// <summary>
    /// Determines whether the result is a success (contains no errors).
    /// </summary>
    /// <returns>True if the result is a success; otherwise, false.</returns>
    public bool IsSuccess() => !Errors.Any();


    /// <summary>
    /// Initializes a new instance of the <see cref="Result{T}"/> class.
    /// </summary>
    public Result() { }


    /// <summary>
    /// Initializes a new instance of the <see cref="Result{T}"/> class
    /// with a value.
    /// </summary>
    /// <param name="value">The value of the result.</param>
    public Result(T value)
    {
        this.value = value;
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="Result{T}"/> class
    /// with a value and warnings.
    /// </summary>
    /// <param name="value">
    /// The value of the result.
    /// </param>
    /// <param name="warnings">
    /// Optional warnings to associate with the result.
    /// <c>null</c> and empty are both accepted.
    /// </param>
    public Result(T value, IEnumerable<IWarning> warnings)
    {
        this.value = value;
        InitWarnings(warnings);
    }


    /// <summary>
    /// Creates a successful result with the specified value.
    /// </summary>
    /// <param name="value">The value of the result.</param>
    /// <returns>A successful result containing the value.</returns>
    public static IResult<T> Ok(T value)
    {
        return new Result<T>(value);
    }


    /// <summary>
    /// Creates a successful result with the specified value
    /// and warnings.
    /// </summary>
    /// <param name="value">
    /// The value of the result.
    /// </param>
    /// <param name="warnings">
    /// Warnings to associate with the result.
    /// </param>
    /// <returns>
    /// A successful result containing the value and warnings.
    /// </returns>
    public static IResult<T> Ok(
        T value,
        IEnumerable<IWarning> warnings)
    {
        Result<T> rv = new Result<T>(value);
        rv.InitWarnings(warnings);
        return rv;
    }


    /// <summary>
    /// Creates a failed result with the specified error.
    /// </summary>
    /// <param name="error">The error to include in the result.</param>
    /// <returns>A failed result containing the error.</returns>
    public static IResult<T> Fail(
        IError error)
    {
        var rv = new Result<T>();
        rv.AddError(error);
        return rv;
    }


    /// <summary>
    /// Creates a failed result with the specified error and warnings.
    /// </summary>
    /// <param name="error">
    /// The error to include in the result.
    /// </param>
    /// <param name="warnings">
    /// Warnings to associate with the result.
    /// </param>
    /// <returns>
    /// A failed result containing the error and warnings.
    /// </returns>
    public static IResult<T> Fail(
        IError error,
        IEnumerable<IWarning> warnings)
    {
        Result<T> rv = new Result<T>();
        rv.AddError(error);
        rv.InitWarnings(warnings);
        return rv;
    }


    //TODO: create a Result.Fail overload that takes another result and operation description. It will be used to represent failure from nested operations.


    /// <summary>
    /// Creates a failed result with the specified errors.
    /// </summary>
    /// <param name="errors">The errors to include in the result.</param>
    /// <returns>A failed result containing the errors.</returns>
    public static IResult<T> Fail(
        params IEnumerable<IError> errors)
    {
        if (errors is null) throw new ArgumentNullException(nameof(errors));
        var list = errors.ToList();
        if (list.Count == 0)
            throw new ArgumentException("At least one error is required.", nameof(errors));

        var rv = new Result<T>();
        foreach (var error in list)
        {
            if (error != null) rv.AddError(error);
        }

        return rv;
    }


    /// <summary>
    /// Gets the type of the value contained in the result.
    /// </summary>
    /// <returns>The type of the value.</returns>
    public Type GetValueType() => typeof(T);
}


/// <summary>
/// Provides static methods for creating <see cref="Result{T}"/> instances.
/// </summary>
public static class Result
{
    /// <summary>
    /// Creates a successful, non-generic result.
    /// </summary>
    /// <returns>
    /// An <see cref="IResult"/> representing a successful operation.
    /// </returns>
    public static IResult Ok()
    {
        return new Result<object>();
    }


    /// <summary>
    /// Creates a successful, non-generic result with warnings.
    /// </summary>
    /// <param name="warnings">
    /// Warnings to associate with the result.
    /// </param>
    /// <returns>
    /// An <see cref="IResult"/> representing a successful operation
    /// with the specified warnings.
    /// </returns>
    public static IResult Ok(IEnumerable<IWarning> warnings)
    {
        Result<object> rv = new Result<object>();
        rv.InitWarnings(warnings);
        return rv;
    }


    /// <summary>
    /// Creates a failed, non-generic result containing the specified error.
    /// </summary>
    /// <param name="error">The error to include in the result.</param>
    /// <returns>
    /// An <see cref="IResult"/> representing a failed operation.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="error"/> is <see langword="null"/>.
    /// </exception>
    public static IResult Fail(
        IError error)
    {
        if (error is null)
        {
            throw new ArgumentNullException(nameof(error));
        }

        Result<object> rv = new Result<object>();
        rv.AddError(error);
        return rv;
    }


    /// <summary>
    /// Creates a failed, non-generic result containing the specified
    /// error and warnings.
    /// </summary>
    /// <param name="error">
    /// The error to include in the result.
    /// </param>
    /// <param name="warnings">
    /// Warnings to associate with the result.
    /// </param>
    /// <returns>
    /// An <see cref="IResult"/> representing a failed operation
    /// with the specified warnings.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="error"/> is <see langword="null"/>.
    /// </exception>
    public static IResult Fail(
        IError error,
        IEnumerable<IWarning> warnings)
    {
        if (error is null)
        {
            throw new ArgumentNullException(nameof(error));
        }

        Result<object> rv = new Result<object>();
        rv.AddError(error);
        rv.InitWarnings(warnings);
        return rv;
    }


    /// <summary>
    /// Creates a failed, non-generic result containing the specified errors.
    /// </summary>
    /// <param name="errors">The errors to include in the result.</param>
    /// <returns>An <see cref="IResult"/> representing a failed operation.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="errors"/> is empty.</exception>
    public static IResult Fail(
        params IEnumerable<IError> errors)
    {
        if (errors is null) throw new ArgumentNullException(nameof(errors));
        var list = errors.ToList();
        if (list.Count == 0)
            throw new ArgumentException("At least one error is required.", nameof(errors));
        var rv = new Result<object>();
        foreach (var error in list)
        {
            if (error != null) rv.AddError(error);
        }
        return rv;
    }


    /// <summary>
    /// Creates a successful result with the specified value.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="value">The value of the result.</param>
    /// <returns>A successful result containing the value.</returns>
    public static IResult<T> Ok<T>(T value)
    {
        return new Result<T>(value);
    }


    /// <summary>
    /// Creates a successful result with the specified value and warnings.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="value">
    /// The value of the result.
    /// </param>
    /// <param name="warnings">
    /// Warnings to associate with the result.
    /// </param>
    /// <returns>
    /// A successful result containing the value and warnings.
    /// </returns>
    public static IResult<T> Ok<T>(
        T value,
        IEnumerable<IWarning> warnings)
    {
        Result<T> rv = new Result<T>(value);
        rv.InitWarnings(warnings);
        return rv;
    }


    /// <summary>
    /// Creates a failed result with the specified error.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="error">The error to include in the result.</param>
    /// <returns>A failed result containing the error.</returns>
    public static IResult<T> Fail<T>(
        IError error)
    {
        if (error is null)
        {
            throw new ArgumentNullException(nameof(error));
        }

        Result<T> rv = new Result<T>();
        rv.AddError(error);
        return rv;
    }


    /// <summary>
    /// Creates a failed result with the specified error and warnings.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="error">
    /// The error to include in the result.
    /// </param>
    /// <param name="warnings">
    /// Warnings to associate with the result.
    /// </param>
    /// <returns>
    /// A failed result containing the error and warnings.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="error"/> is <see langword="null"/>.
    /// </exception>
    public static IResult<T> Fail<T>(
        IError error,
        IEnumerable<IWarning> warnings)
    {
        if (error is null)
        {
            throw new ArgumentNullException(nameof(error));
        }

        Result<T> rv = new Result<T>();
        rv.AddError(error);
        rv.InitWarnings(warnings);
        return rv;
    }


    /// <summary>
    /// Creates a failed result containing the specified errors.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the value associated with the result.
    /// </typeparam>
    /// <param name="errors">
    /// A collection of errors to associate with the result.
    /// Must contain at least one non-null error.
    /// </param>
    /// <returns>
    /// An <see cref="IResult{T}"/> representing a failed operation with
    /// the specified errors.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="errors"/> is empty or contains no non-null errors.</exception>
    public static IResult<T> Fail<T>(
        params IEnumerable<IError> errors)
    {
        if (errors is null) throw new ArgumentNullException(nameof(errors));
        var list = errors.ToList();
        if (list.Count == 0)
            throw new ArgumentException("At least one error is required.", nameof(errors));
        var rv = new Result<T>();
        foreach (var error in list)
        {
            if (error != null) rv.AddError(error);
        }
        return rv;
    }


    /// <summary>
    /// Creates a failed result with the specified error and value.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="value">The value of the result.</param>
    /// <param name="error">The error to include in the result.</param>
    /// <returns>A failed result containing the error and value.</returns>
    public static IResult<T> Fail<T>(
        T value,
        IError error)
    {
        Result<T> rv = new Result<T>(value);
        rv.AddError(error);
        return rv;
    }


    /// <summary>
    /// Creates a failed result with the specified error, value,
    /// and warnings.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="value">
    /// The value of the result.
    /// </param>
    /// <param name="error">
    /// The error to include in the result.
    /// </param>
    /// <param name="warnings">
    /// Warnings to associate with the result.
    /// </param>
    /// <returns>
    /// A failed result containing the error, value, and warnings.
    /// </returns>
    public static IResult<T> Fail<T>(
        T value,
        IError error,
        IEnumerable<IWarning> warnings)
    {
        Result<T> rv = new Result<T>(value);
        rv.AddError(error);
        rv.InitWarnings(warnings);
        return rv;
    }
}
