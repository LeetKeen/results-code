using System;

namespace LeetKeen.Results;

/// <summary>
/// Provides fluent extension methods for building
/// <see cref="IWarning"/> and <see cref="Warning"/> instances.
/// </summary>
public static class WarningExtensions
{
    /// <summary>
    /// Adds a data item to the warning's <see cref="IWarning.Payload"/>.
    /// </summary>
    /// <typeparam name="TWarning">
    /// The concrete warning type; must implement
    /// <see cref="IWarning"/>.
    /// </typeparam>
    /// <param name="warning">
    /// The warning to add data to. Must not be <c>null</c>.
    /// </param>
    /// <param name="key">
    /// The key for the data item. Must not be <c>null</c> or empty.
    /// </param>
    /// <param name="value">
    /// The value for the data item. May be <c>null</c>.
    /// </param>
    /// <param name="visibility">
    /// The visibility level.
    /// <c>0</c> = public (default); non-zero = sensitive.
    /// </param>
    /// <returns>
    /// The same <paramref name="warning"/> instance for fluent chaining.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="warning"/> or
    /// <paramref name="key"/> is <c>null</c>.
    /// </exception>
    public static TWarning WithData<TWarning>(
        this TWarning warning,
        string key,
        object? value,
        int visibility = 0)
        where TWarning : IWarning
    {
        if (warning == null)
            throw new ArgumentNullException(nameof(warning));
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        warning.Payload.Add(key, value, visibility);
        return warning;
    }


    /// <summary>
    /// Adds a child warning to the warning's
    /// <see cref="Warning.Children"/> collection.
    /// </summary>
    /// <typeparam name="TWarning">
    /// The concrete warning type; must derive from
    /// <see cref="Warning"/>.
    /// </typeparam>
    /// <param name="warning">
    /// The warning to add a child to. Must not be <c>null</c>.
    /// </param>
    /// <param name="child">
    /// The child warning to add. Must not be <c>null</c>.
    /// </param>
    /// <returns>
    /// The same <paramref name="warning"/> instance for fluent chaining.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="warning"/> or
    /// <paramref name="child"/> is <c>null</c>.
    /// </exception>
    public static TWarning WithChild<TWarning>(
        this TWarning warning,
        IWarning child)
        where TWarning : Warning
    {
        if (warning == null)
            throw new ArgumentNullException(nameof(warning));
        if (child == null)
            throw new ArgumentNullException(nameof(child));

        warning.AddChild(child);
        return warning;
    }
}
