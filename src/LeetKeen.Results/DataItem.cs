using System;

namespace LeetKeen.Results;

/// <summary>
/// Represents a single named data entry in a <see cref="IDataBag"/>.
/// </summary>
public sealed class DataItem
{
    /// <summary>
    /// Gets the key that identifies this item within a bag.
    /// </summary>
    public string Key { get; }


    /// <summary>
    /// Gets the value stored in this item.
    /// May be <c>null</c>.
    /// </summary>
    public object? Value { get; }


    /// <summary>
    /// Gets the visibility level of this item.
    /// <c>0</c> means public (no visibility restriction).
    /// Any non-zero value marks the item as sensitive or restricted.
    /// Negative values are reserved for future use.
    /// </summary>
    public int Visibility { get; }


    /// <summary>
    /// Initializes a new instance of <see cref="DataItem"/>.
    /// </summary>
    /// <param name="key">
    /// The key identifying this item. Must not be null or empty.
    /// </param>
    /// <param name="value">
    /// The value stored in this item.
    /// </param>
    /// <param name="visibility">
    /// The visibility level.
    /// <c>0</c> = public; non-zero = sensitive.
    /// Defaults to <c>0</c>.
    /// </param>
    public DataItem(string key, object? value, int visibility = 0)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentException(
                "Key must not be null or empty.",
                nameof(key));

        Key = key;
        Value = value;
        Visibility = visibility;
    }
}
