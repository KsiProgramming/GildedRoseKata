//-----------------------------------------------------------------------
// <copyright file="ItemKindTests.cs" company="KsiProgramming">
//     Copyright (c) KsiProgramming. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace GildedRoseKata.ProposedCode.Tests;

using FluentAssertions;

public class ItemKindTests
{
    [Theory]
    [InlineData("Aged Brie", typeof(ItemKind.AgedBrie))]
    [InlineData("AgedBrie", typeof(ItemKind.AgedBrie))]
    [InlineData("Backstage passes to a TAFKAL80ETC concert", typeof(ItemKind.Backstage))]
    [InlineData("Backstage", typeof(ItemKind.Backstage))]
    [InlineData("Conjured Mana Cake", typeof(ItemKind.Conjured))]
    [InlineData("Conjured", typeof(ItemKind.Conjured))]
    [InlineData("Sulfuras, Hand of Ragnaros", typeof(ItemKind.Sulfuras))]
    [InlineData("Cellar-Aged Mead", typeof(ItemKind.CellarAgedMead))]
    [InlineData("Elixir of the Mongoose", typeof(ItemKind.Standard))]
    public void FromName_MapsNamesToExpectedKinds(string itemName, Type expectedType)
    {
        var result = ItemKind.FromName(itemName);

        result.Should().BeOfType(expectedType);
    }

    [Theory]
    [InlineData("Cellar-Aged")]
    [InlineData("Mead")]
    [InlineData("Cellar-Aged Mead Reserve")]
    [InlineData("cellar-aged mead")]
    [InlineData("Cellar-Aged Mead ")]
    public void FromName_WhenNameIsNotExactCellarAgedMead_ReturnsStandard(string itemName)
    {
        var result = ItemKind.FromName(itemName);

        result.Should().BeOfType<ItemKind.Standard>();
    }
}
