using System;
using System.Linq;
using Shouldly;

namespace LeetKeen.Results.UnitTests;

[TestClass]
public class WarningTests
{
    // ----------------------------------------------------------------
    // Standalone warning (no children)
    // ----------------------------------------------------------------

    /// <summary>
    /// A warning with no children has Text as its sole message.
    /// </summary>
    [TestMethod]
    public void StandaloneWarning_Text_IsTheMessage()
    {
        Warning warning = new Warning(0, "low disk space");

        warning.Text.ShouldBe("low disk space");
        warning.Children.ShouldBeEmpty();
    }


    /// <summary>
    /// A standalone warning is not a container:
    /// Children.Any() returns false.
    /// </summary>
    [TestMethod]
    public void StandaloneWarning_ChildrenAny_IsFalse()
    {
        Warning warning = new Warning(0, "standalone");

        warning.Children.Any().ShouldBeFalse();
    }


    /// <summary>
    /// Code = 0 on a standalone warning signals no translator is needed.
    /// </summary>
    [TestMethod]
    public void StandaloneWarning_CodeZero_MeansNoTranslator()
    {
        Warning warning = new Warning(0, "some text");

        warning.Code.ShouldBe(0);
    }


    // ----------------------------------------------------------------
    // Aggregate container (with children)
    // ----------------------------------------------------------------

    /// <summary>
    /// A warning with at least one child is a container:
    /// Children.Any() returns true.
    /// </summary>
    [TestMethod]
    public void AggregateWarning_ChildrenAny_IsTrue()
    {
        Warning parent = new Warning(0, null);
        Warning child = new Warning(0, "child detail");
        parent.AddChild(child);

        parent.Children.Any().ShouldBeTrue();
    }


    /// <summary>
    /// Children added via AddChild are present in the collection
    /// and accessible through IWarning.
    /// </summary>
    [TestMethod]
    public void AggregateWarning_Children_ContainsAddedChild()
    {
        Warning parent = new Warning(0, null);
        Warning child = new Warning(0, "detail");

        parent.AddChild(child);

        parent.Children.Count.ShouldBe(1);
        parent.Children[0].Text.ShouldBe("detail");
    }


    /// <summary>
    /// AddChild throws ArgumentNullException when child is null.
    /// </summary>
    [TestMethod]
    public void AddChild_Null_ThrowsArgumentNullException()
    {
        Warning warning = new Warning();

        Should.Throw<ArgumentNullException>(() => warning.AddChild(null!));
    }


    // ----------------------------------------------------------------
    // Nested children, Payload, Code = 0
    // ----------------------------------------------------------------

    /// <summary>
    /// Nested children are accessible through the Children hierarchy.
    /// </summary>
    [TestMethod]
    public void NestedChildren_AreAccessible()
    {
        Warning root = new Warning(0, "root");
        Warning middle = new Warning(0, "middle");
        Warning leaf = new Warning(0, "leaf");

        middle.AddChild(leaf);
        root.AddChild(middle);

        root.Children.Count.ShouldBe(1);
        IWarning middleViaInterface = root.Children[0];
        middleViaInterface.Text.ShouldBe("middle");
        middleViaInterface.Children.Count.ShouldBe(1);
        middleViaInterface.Children[0].Text.ShouldBe("leaf");
    }


    /// <summary>
    /// Payload is empty on a default-constructed Warning.
    /// </summary>
    [TestMethod]
    public void Payload_IsEmptyByDefault()
    {
        Warning warning = new Warning();

        warning.Payload.Items.ShouldBeEmpty();
    }


    /// <summary>
    /// Payload items are accessible through IWarning.
    /// </summary>
    [TestMethod]
    public void Payload_IsAccessibleViaInterface()
    {
        Warning warning = new Warning(0, "msg");
        warning.Payload.Add("key", "value");

        ((IWarning)warning).Payload.Items.Count.ShouldBe(1);
        ((IWarning)warning).Payload.Items[0].Key.ShouldBe("key");
    }


    // ----------------------------------------------------------------
    // Text trimming
    // ----------------------------------------------------------------

    /// <summary>
    /// Text is trimmed on set via the constructor.
    /// </summary>
    [TestMethod]
    public void Text_Constructor_TrimsWhitespace()
    {
        Warning warning = new Warning(0, "  trimmed  ");

        warning.Text.ShouldBe("trimmed");
    }


    /// <summary>
    /// Text is trimmed on set via the property setter.
    /// </summary>
    [TestMethod]
    public void Text_Setter_TrimsWhitespace()
    {
        Warning warning = new Warning { Text = "  hello  " };

        warning.Text.ShouldBe("hello");
    }


    /// <summary>
    /// Text is null on a default-constructed Warning.
    /// </summary>
    [TestMethod]
    public void Text_IsNullByDefault()
    {
        Warning warning = new Warning();

        warning.Text.ShouldBeNull();
    }


    // ----------------------------------------------------------------
    // WarningExtensions.WithData
    // ----------------------------------------------------------------

    /// <summary>
    /// WithData adds a key/value entry with visibility 0 to Payload.
    /// </summary>
    [TestMethod]
    public void WithData_AddsItemToPayload()
    {
        Warning warning = new Warning();
        warning.WithData("region", "us-east-1");

        warning.Payload.Items.Count.ShouldBe(1);
        warning.Payload.Items[0].Key.ShouldBe("region");
        warning.Payload.Items[0].Value.ShouldBe("us-east-1");
        warning.Payload.Items[0].Visibility.ShouldBe(0);
    }


    /// <summary>
    /// WithData stores the specified visibility value on the DataItem.
    /// </summary>
    [TestMethod]
    public void WithData_WithVisibility_SetsVisibility()
    {
        Warning warning = new Warning();
        warning.WithData("internal", "data", visibility: 1);

        warning.Payload.Items[0].Visibility.ShouldBe(1);
    }


    /// <summary>
    /// WithData returns the same warning instance for fluent chaining.
    /// </summary>
    [TestMethod]
    public void WithData_ReturnsSameInstance()
    {
        Warning warning = new Warning();
        Warning returned = warning.WithData("k", "v");

        returned.ShouldBeSameAs(warning);
    }


    /// <summary>
    /// WithData throws ArgumentNullException when warning is null.
    /// </summary>
    [TestMethod]
    public void WithData_NullWarning_ThrowsArgumentNullException()
    {
        Warning? warning = null;

        Should.Throw<ArgumentNullException>(
            () => warning!.WithData("k", "v"));
    }


    // ----------------------------------------------------------------
    // WarningExtensions.WithChild
    // ----------------------------------------------------------------

    /// <summary>
    /// WithChild adds the child and returns the same warning instance.
    /// </summary>
    [TestMethod]
    public void WithChild_AddsChildAndReturnsInstance()
    {
        Warning parent = new Warning(0, "parent");
        Warning child = new Warning(0, "child");

        Warning returned = parent.WithChild(child);

        returned.ShouldBeSameAs(parent);
        parent.Children.Count.ShouldBe(1);
        parent.Children[0].Text.ShouldBe("child");
    }


    /// <summary>
    /// WithChild throws ArgumentNullException when child is null.
    /// </summary>
    [TestMethod]
    public void WithChild_NullChild_ThrowsArgumentNullException()
    {
        Warning warning = new Warning();

        Should.Throw<ArgumentNullException>(
            () => warning.WithChild(null!));
    }


    // ----------------------------------------------------------------
    // IWarning interface accessibility
    // ----------------------------------------------------------------

    /// <summary>
    /// All IWarning members are readable through the interface.
    /// </summary>
    [TestMethod]
    public void IWarning_AllMembers_AccessibleViaInterface()
    {
        Warning warning = new Warning(42, "msg");
        warning.Payload.Add("k", "v");
        warning.AddChild(new Warning(0, "child"));

        ((IWarning)warning).Code.ShouldBe(42);
        ((IWarning)warning).Text.ShouldBe("msg");
        ((IWarning)warning).Payload.Items.Count.ShouldBe(1);
        ((IWarning)warning).Children.Count.ShouldBe(1);
    }
}
