using System;
using System.Collections.Generic;
using System.Linq;

namespace LeetKeen.Results;

/// <summary>
/// Represents an error.
/// Implements the <see cref="IError"/> interface.
/// </summary>
public class Error
    : IError
{
    private string? text;
    private readonly List<IError> children = [];


    /// <inheritdoc />
    public int Code { get; set; }


    /// <summary>
    /// Gets or sets the human-readable error text.
    /// Automatically trimmed on set; <c>null</c> is preserved as-is.
    /// </summary>
    public string? Text
    {
        get => text;
        set => text = value?.Trim();
    }


    /// <summary>
    /// Structured data payload associated with this error.
    /// </summary>
    public DataBag Payload { get; } = new DataBag();


    /// <summary>
    /// <c>true</c> when this is a transient failure that may succeed on
    /// retry; <c>false</c> for permanent errors.
    /// </summary>
    public bool IsTransient { get; set; }


    /// <summary>
    /// Correlation identifier read from <see cref="Payload"/>.
    /// Returns the string value of the item with key
    /// <c>"correlationId"</c>; <c>null</c> when the item is absent or
    /// its value is not a <see cref="string"/>.
    /// </summary>
    public string? CorrelationId =>
        Payload.Items
            .FirstOrDefault(i => i.Key == "correlationId")
            ?.Value as string;


    /// <summary>
    /// Child errors that contributed to or caused this error.
    /// </summary>
    public IReadOnlyList<IError> Children => children;


    //TODO: when serializing, make sure children do not reference each
    // other even through an intermediary.

    /// <summary>
    /// Initializes a new instance of the <see cref="Error"/> class.
    /// </summary>
    public Error() { }


    /// <summary>
    /// Initializes a new instance of the <see cref="Error"/> class with
    /// a code and text.
    /// </summary>
    /// <param name="code">The error code.</param>
    /// <param name="text">
    /// The error text. May be <c>null</c>.
    /// Automatically trimmed.
    /// </param>
    public Error(int code, string? text)
    {
        Code = code;
        Text = text;
    }


    /// <summary>
    /// Adds a child error to this error.
    /// </summary>
    /// <param name="child">The child error to add.</param>
    public void AddChild(IError child)
    {
        if (child == null)
            throw new ArgumentNullException(nameof(child));
        children.Add(child);
    }


    /// <summary>
    /// Creates a new error with the specified code and text.
    /// </summary>
    /// <param name="code">
    /// The error code.
    /// </param>
    /// <param name="text">
    /// The error text. May be <c>null</c>.
    /// </param>
    /// <returns>
    /// An <see cref="IError"/> representing the new error.
    /// </returns>
    public static IError New(int code, string? text)
    {
        return new Error(code, text);
    }


    /// <summary>
    /// Creates a standard EntityNotFound error.
    /// </summary>
    /// <param name="entityId">
    /// The ID of the entity.
    /// </param>
    /// <param name="entityKind">
    /// The kind of the entity (optional).
    /// </param>
    /// <returns>
    /// An <see cref="IError"/> representing the entity not found error.
    /// </returns>
    [Obsolete("Use StandardErrors.Business.EntityNotFound instead.")]
    public static IError EntityNotFoundNew(
        object entityId,
        string entityKind = "")
        => NewEntityNotFoundNew(entityId, entityKind);


    /// <summary>
    /// Creates a standard EntityNotFound error.
    /// </summary>
    /// <param name="entityId">The ID of the entity.</param>
    /// <param name="entityKind">The kind of the entity (optional).</param>
    /// <returns>
    /// An <see cref="IError"/> representing the entity not found error.
    /// </returns>
    public static IError NewEntityNotFoundNew(
        object entityId,
        string entityKind = "")
    {
        if (entityId == null)
            throw new ArgumentNullException(nameof(entityId));

        string msg = string.IsNullOrWhiteSpace(entityKind)
            ? "Entity not found."
            : $"Entity of kind {entityKind} not found.";

        Error rv = new Error(StandardErrorCodes.Business.EntityNotFound, msg)
            .WithData("id", entityId);
        return rv;
    }
}
