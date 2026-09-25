using System;
using System.Collections.Generic;

namespace LeetKeen.Results;

/// <summary>
/// Represents a warning that does not prevent success but signals
/// a condition the caller should be aware of.
/// Implements the <see cref="IWarning"/> interface.
/// </summary>
/// <remarks>
/// When <see cref="Code"/> is <c>0</c> no translator is needed;
/// <see cref="Text"/> is the sole message source.
/// Whether this instance is an aggregate container is determined
/// by inspecting <see cref="Children"/>: if <c>Children.Any()</c>
/// is <c>true</c> the instance is a container.
/// </remarks>
public class Warning
    : IWarning
{
    private string? text;
    private readonly List<IWarning> children = [];


    /// <inheritdoc />
    public int Code { get; set; }


    /// <summary>
    /// Gets or sets the human-readable warning text.
    /// Automatically trimmed on set; <c>null</c> is preserved as-is.
    /// </summary>
    public string? Text
    {
        get => text;
        set => text = value?.Trim();
    }


    /// <summary>
    /// Structured data payload associated with this warning.
    /// </summary>
    public DataBag Payload { get; } = new DataBag();


    /// <summary>
    /// Child warnings that form part of this warning's detail.
    /// Empty for standalone warnings.
    /// When non-empty this instance acts as an aggregate container.
    /// </summary>
    public IReadOnlyList<IWarning> Children => children;


    /// <summary>
    /// Initializes a new empty <see cref="Warning"/> with
    /// <see cref="Code"/> <c>0</c> and no text.
    /// </summary>
    public Warning() { }


    /// <summary>
    /// Initializes a new <see cref="Warning"/> with the specified
    /// code and text.
    /// </summary>
    /// <param name="code">
    /// The warning code.
    /// Use <c>0</c> when <paramref name="text"/> is the sole message
    /// source and no translator is needed.
    /// </param>
    /// <param name="text">
    /// The warning text. May be <c>null</c>.
    /// Automatically trimmed.
    /// </param>
    public Warning(int code, string? text)
    {
        Code = code;
        Text = text;
    }


    /// <summary>
    /// Adds a child warning to this warning's
    /// <see cref="Children"/> collection.
    /// </summary>
    /// <param name="child">
    /// The child warning to add. Must not be <c>null</c>.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="child"/> is <c>null</c>.
    /// </exception>
    public void AddChild(IWarning child)
    {
        if (child == null)
            throw new ArgumentNullException(nameof(child));
        children.Add(child);
    }
}
