using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LeetKeen.Results;

/// <summary>
/// Default mutable implementation of <see cref="IDataBag"/>.
/// Supports fluent construction via <see cref="Add"/>.
/// </summary>
public sealed class DataBag : IDataBag
{
    private readonly List<DataItem> items = [];
    private readonly ReadOnlyCollection<DataItem> readOnlyItems;


    /// <summary>
    /// Initializes a new empty <see cref="DataBag"/>.
    /// </summary>
    public DataBag()
    {
        readOnlyItems = items.AsReadOnly();
    }


    /// <inheritdoc />
    public IReadOnlyList<DataItem> Items => readOnlyItems;


    /// <summary>
    /// Adds a new item to this bag and returns this instance
    /// for fluent chaining.
    /// </summary>
    /// <param name="key">
    /// The key identifying the item. Must not be null or empty.
    /// </param>
    /// <param name="value">
    /// The value to store.
    /// </param>
    /// <param name="visibility">
    /// The visibility level.
    /// <c>0</c> = public (default); non-zero = sensitive.
    /// </param>
    /// <returns>
    /// This <see cref="DataBag"/> instance.
    /// </returns>
    public DataBag Add(string key, object? value, int visibility = 0)
    {
        items.Add(new DataItem(key, value, visibility));
        return this;
    }
}
