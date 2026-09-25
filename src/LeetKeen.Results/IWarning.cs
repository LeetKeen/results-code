using System.Collections.Generic;

namespace LeetKeen.Results;

/// <summary>
/// Represents a warning that does not prevent success but signals
/// a condition the caller should be aware of.
/// </summary>
/// <remarks>
/// A warning with no children is a standalone advisory; its
/// <see cref="Text"/> is the primary message.
/// A warning whose <see cref="Children"/> collection is non-empty
/// acts as an aggregate container — callers determine this via
/// <c>Children.Any()</c>; there is no separate flag.
/// <para>
/// When <see cref="Code"/> is <c>0</c> no translator is needed;
/// <see cref="Text"/> is the sole message source.
/// Non-zero codes identify well-known warning kinds whose human text
/// may be produced by a code-to-text translator.
/// </para>
/// Thread-safety is not required by this contract.
/// </remarks>
public interface IWarning
{
    /// <summary>
    /// Warning code.
    /// <c>0</c> means no translator is needed; <see cref="Text"/> is
    /// the sole message source.
    /// Non-zero values identify well-known warning kinds.
    /// </summary>
    int Code { get; }


    /// <summary>
    /// Human-readable warning text.
    /// May be <c>null</c> when the warning is fully described by its
    /// <see cref="Code"/> and <see cref="Payload"/>.
    /// Implementations must trim the value on assignment.
    /// </summary>
    string? Text { get; }


    /// <summary>
    /// Structured data payload associated with this warning.
    /// Never <c>null</c>; may be empty.
    /// </summary>
    DataBag Payload { get; }


    /// <summary>
    /// Child warnings that form part of this warning's detail.
    /// Never <c>null</c>; empty for standalone warnings.
    /// When non-empty this warning acts as an aggregate container.
    /// </summary>
    IReadOnlyList<IWarning> Children { get; }
}
