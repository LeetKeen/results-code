using System;
using Shouldly;

namespace LeetKeen.Results.UnitTests;

[TestClass]
public class ErrorTests
{
    // ----------------------------------------------------------------
    // Text
    // ----------------------------------------------------------------

    /// <summary>
    /// Text property is null on a default-constructed Error.
    /// </summary>
    [TestMethod]
    public void Text_IsNullByDefault()
    {
        Error error = new Error();

        error.Text.ShouldBeNull();
    }


    /// <summary>
    /// Setting Text with surrounding whitespace stores the trimmed value.
    /// </summary>
    [TestMethod]
    public void Text_Setter_TrimsWhitespace()
    {
        Error error = new Error { Text = "  hello  " };

        error.Text.ShouldBe("hello");
    }


    /// <summary>
    /// Setting Text to null stores null without throwing.
    /// </summary>
    [TestMethod]
    public void Text_Setter_PreservesNull()
    {
        Error error = new Error { Text = null };

        error.Text.ShouldBeNull();
    }


    /// <summary>
    /// The two-argument constructor assigns Code and trims Text.
    /// </summary>
    [TestMethod]
    public void Constructor_SetsCodeAndText()
    {
        Error error = new Error(-1, "  trimmed  ");

        error.Code.ShouldBe(-1);
        error.Text.ShouldBe("trimmed");
    }


    // ----------------------------------------------------------------
    // IsTransient
    // ----------------------------------------------------------------

    /// <summary>
    /// IsTransient is false on a default-constructed Error.
    /// </summary>
    [TestMethod]
    public void IsTransient_DefaultIsFalse()
    {
        Error error = new Error();

        error.IsTransient.ShouldBeFalse();
    }


    /// <summary>
    /// IsTransient can be set to true via the property setter.
    /// </summary>
    [TestMethod]
    public void IsTransient_CanBeSetToTrue()
    {
        Error error = new Error { IsTransient = true };

        error.IsTransient.ShouldBeTrue();
    }


    /// <summary>
    /// IsTransient set on the concrete type is readable through IError.
    /// </summary>
    [TestMethod]
    public void IsTransient_PropagatesViaInterface()
    {
        Error error = new Error { IsTransient = true };

        ((IError)error).IsTransient.ShouldBeTrue();
    }


    // ----------------------------------------------------------------
    // CorrelationId
    // ----------------------------------------------------------------

    /// <summary>
    /// CorrelationId is null when Payload contains no correlationId entry.
    /// </summary>
    [TestMethod]
    public void CorrelationId_ReturnsNull_WhenPayloadHasNoCorrelationId()
    {
        Error error = new Error();

        error.CorrelationId.ShouldBeNull();
    }


    /// <summary>
    /// CorrelationId returns the string value stored under
    /// 'correlationId' in Payload.
    /// </summary>
    [TestMethod]
    public void CorrelationId_ReturnsValue_WhenPayloadContainsCorrelationId()
    {
        Error error = new Error();
        error.Payload.Add("correlationId", "abc-123");

        error.CorrelationId.ShouldBe("abc-123");
    }


    /// <summary>
    /// CorrelationId is null when the payload value is a non-string type.
    /// </summary>
    [TestMethod]
    public void CorrelationId_ReturnsNull_WhenValueIsNotString()
    {
        Error error = new Error();
        error.Payload.Add("correlationId", 42);

        error.CorrelationId.ShouldBeNull();
    }


    /// <summary>
    /// CorrelationId is null when the payload value is explicitly null.
    /// </summary>
    [TestMethod]
    public void CorrelationId_ReturnsNull_WhenValueIsNull()
    {
        Error error = new Error();
        error.Payload.Add("correlationId", null);

        error.CorrelationId.ShouldBeNull();
    }


    /// <summary>
    /// CorrelationId stored on the concrete type is readable through IError.
    /// </summary>
    [TestMethod]
    public void CorrelationId_IsAccessibleViaInterface()
    {
        Error error = new Error();
        error.Payload.Add("correlationId", "trace-xyz");

        ((IError)error).CorrelationId.ShouldBe("trace-xyz");
    }


    /// <summary>
    /// The DataItem for correlationId has visibility 0 (public)
    /// when added via Payload.Add.
    /// </summary>
    [TestMethod]
    public void CorrelationId_Visibility_IsZero()
    {
        // The underlying DataItem for correlationId must use
        // visibility = 0 (public).
        Error error = new Error();
        error.Payload.Add("correlationId", "v1");

        DataItem item = error.Payload.Items[0];
        item.Visibility.ShouldBe(0);
    }


    // ----------------------------------------------------------------
    // Children
    // ----------------------------------------------------------------

    /// <summary>
    /// Children collection is empty on a default-constructed Error.
    /// </summary>
    [TestMethod]
    public void Children_EmptyByDefault()
    {
        Error error = new Error();

        error.Children.ShouldBeEmpty();
    }


    /// <summary>
    /// AddChild appends the child error to the Children collection.
    /// </summary>
    [TestMethod]
    public void AddChild_AddsErrorToChildren()
    {
        Error parent = new Error { Text = "parent" };
        Error child = new Error { Text = "child" };

        parent.AddChild(child);

        parent.Children.Count.ShouldBe(1);
        parent.Children[0].Text.ShouldBe("child");
    }


    /// <summary>
    /// AddChild throws ArgumentNullException when the child argument is null.
    /// </summary>
    [TestMethod]
    public void AddChild_Null_ThrowsArgumentNullException()
    {
        Error error = new Error();

        Should.Throw<ArgumentNullException>(() => error.AddChild(null!));
    }


    /// <summary>
    /// WithChild adds the child and returns the same error instance
    /// for chaining.
    /// </summary>
    [TestMethod]
    public void WithChild_Extension_AddsChildAndReturnsError()
    {
        Error parent = new Error { Text = "parent" };
        Error child = new Error { Text = "child" };

        Error returned = parent.WithChild(child);

        returned.ShouldBeSameAs(parent);
        parent.Children.Count.ShouldBe(1);
        parent.Children[0].Text.ShouldBe("child");
    }


    /// <summary>
    /// WithChildren adds all supplied children to the Children collection.
    /// </summary>
    [TestMethod]
    public void WithChildren_Extension_AddsMultipleChildren()
    {
        Error parent = new Error { Text = "parent" };
        Error child1 = new Error { Text = "child1" };
        Error child2 = new Error { Text = "child2" };

        parent.WithChildren([child1, child2]);

        parent.Children.Count.ShouldBe(2);
        parent.Children[0].Text.ShouldBe("child1");
        parent.Children[1].Text.ShouldBe("child2");
    }


    /// <summary>
    /// Children added via AddChild are readable through the IError interface.
    /// </summary>
    [TestMethod]
    public void Children_IsAccessibleViaInterface()
    {
        Error parent = new Error { Text = "parent" };
        Error child = new Error { Text = "child" };

        ((IError)parent).AddChild(child);

        ((IError)parent).Children.Count.ShouldBe(1);
        ((IError)parent).Children[0].Text.ShouldBe("child");
    }


    // ----------------------------------------------------------------
    // Payload / WithData
    // ----------------------------------------------------------------

    /// <summary>
    /// Payload has no items on a default-constructed Error.
    /// </summary>
    [TestMethod]
    public void Payload_EmptyByDefault()
    {
        Error error = new Error();

        error.Payload.Items.ShouldBeEmpty();
    }


    /// <summary>
    /// WithData adds a key/value entry with visibility 0 to Payload.
    /// </summary>
    [TestMethod]
    public void WithData_AddsItemToPayload()
    {
        Error error = new Error();
        error.WithData("key", "value");

        error.Payload.Items.Count.ShouldBe(1);
        error.Payload.Items[0].Key.ShouldBe("key");
        error.Payload.Items[0].Value.ShouldBe("value");
        error.Payload.Items[0].Visibility.ShouldBe(0);
    }


    /// <summary>
    /// WithData stores the specified visibility value on the DataItem.
    /// </summary>
    [TestMethod]
    public void WithData_WithVisibility_SetsVisibility()
    {
        Error error = new Error();
        error.WithData("secret", "token", visibility: 1);

        error.Payload.Items[0].Visibility.ShouldBe(1);
    }
}
