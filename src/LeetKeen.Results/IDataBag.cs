using System.Collections.Generic;

namespace LeetKeen.Results;

/// <summary>
/// Represents a read-only bag of named <see cref="DataItem"/> entries.
/// </summary>
/// <remarks>
/// Implementations must guarantee that <see cref="Items"/> is never
/// <c>null</c>. The order of items must reflect the order in which they
/// were added. Thread-safety is not required by this contract.
/// </remarks>
public interface IDataBag
{
    /// <summary>
    /// Gets all items stored in this bag.
    /// Never <c>null</c>; empty when no items have been added.
    /// Order of items reflects the order in which they were added.
    /// </summary>
    IReadOnlyList<DataItem> Items { get; }
}
