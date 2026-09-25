using System.Collections.Generic;
using Shouldly;

namespace LeetKeen.Results.UnitTests;

[TestClass]
public class DataBagTests
{
    [TestMethod]
    public void EmptyBag_HasNoItems()
    {
        // ACT
        DataBag bag = new DataBag();

        // ASSERT
        bag.Items.ShouldBeEmpty();
    }


    [TestMethod]
    public void Add_PublicItem_Visibility_IsZero()
    {
        // ACT
        DataBag bag = new DataBag()
            .Add("key1", "value1");

        // ASSERT
        bag.Items.Count.ShouldBe(1);
        DataItem item = bag.Items[0];
        item.Key.ShouldBe("key1");
        item.Value.ShouldBe("value1");
        item.Visibility.ShouldBe(0);
    }


    [TestMethod]
    public void Add_SensitiveItem_Visibility_IsNonZero()
    {
        // ACT
        DataBag bag = new DataBag()
            .Add("secret", "hidden", visibility: 1);

        // ASSERT
        bag.Items.Count.ShouldBe(1);
        DataItem item = bag.Items[0];
        item.Visibility.ShouldBe(1);
        item.Key.ShouldBe("secret");
    }


    [TestMethod]
    public void Add_FluentChaining_AddsAllItemsInOrder()
    {
        // ACT
        DataBag bag = new DataBag()
            .Add("key1", 42)
            .Add("key2", "hello", visibility: 2)
            .Add("key3", true);

        // ASSERT
        bag.Items.Count.ShouldBe(3);
        bag.Items[0].Key.ShouldBe("key1");
        bag.Items[0].Value.ShouldBe(42);
        bag.Items[1].Key.ShouldBe("key2");
        bag.Items[1].Visibility.ShouldBe(2);
        bag.Items[2].Key.ShouldBe("key3");
    }


    [TestMethod]
    public void Add_Returns_SameInstance()
    {
        // ARRANGE
        DataBag bag = new DataBag();

        // ACT
        DataBag returned = bag.Add("k", "v");

        // ASSERT
        returned.ShouldBeSameAs(bag);
    }


    [TestMethod]
    public void Items_Implements_IReadOnlyList()
    {
        // ACT
        DataBag bag = new DataBag()
            .Add("k", "v");

        // ASSERT
        bag.Items.ShouldBeAssignableTo<IReadOnlyList<DataItem>>();
    }
}
