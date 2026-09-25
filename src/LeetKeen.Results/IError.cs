using System.Collections.Generic;

namespace LeetKeen.Results;

/// <summary>
/// Represents an error.
/// </summary>
public interface IError
{
    /// <summary>
    /// Error code.
    /// <c>0</c> means unstructured — no specific classification.
    /// Standard negative codes are defined in
    /// <see cref="StandardErrorCodes"/>.
    /// </summary>
    int Code { get; }


    /// <summary>
    /// Human-readable error text.
    /// May be <c>null</c> when the error is fully described by its
    /// <see cref="Code"/> and <see cref="Payload"/>.
    /// Implementations must trim the value on assignment.
    /// </summary>
    string? Text { get; }


    /// <summary>
    /// Structured data payload associated with this error.
    /// Never <c>null</c>; may be empty.
    /// </summary>
    DataBag Payload { get; }


    /// <summary>
    /// <c>true</c> when this is a transient failure (e.g. a network drop
    /// or a momentarily unavailable dependency) that may succeed on retry.
    /// <c>false</c> for permanent errors.
    /// </summary>
    bool IsTransient { get; }


    /// <summary>
    /// Correlation identifier propagated through a distributed call chain.
    /// Computed from <see cref="Payload"/>: returns the value of the item
    /// whose key is <c>"correlationId"</c> cast to <see cref="string"/>;
    /// <c>null</c> when the item is absent or its value is not a string.
    /// </summary>
    string? CorrelationId { get; }


    /// <summary>
    /// Child errors that contributed to or caused this error.
    /// Never <c>null</c>; empty when there are no children.
    /// </summary>
    IReadOnlyList<IError> Children { get; }


    /// <summary>
    /// Adds a child error to <see cref="Children"/>.
    /// </summary>
    /// <param name="child">
    /// The child error to add. Must not be <c>null</c>.
    /// </param>
    void AddChild(IError child);
}
