//-----------------------------------------------------------------------
// <copyright file="ItemMaturationServiceTests.cs" company="KsiProgramming">
//     Copyright (c) KsiProgramming. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace GildedRoseKata.ProposedCode.Tests;

using FluentAssertions;
using GildedRoseKata.OriginalCode;

public class ItemMaturationServiceTests
{
    [Theory]
    [InlineData("Elixir of the Mongoose", 10, 5, 9, 4)]
    [InlineData("Aged Brie", 10, 2, 11, 1)]
    [InlineData("AgedBrie", 10, 2, 11, 1)]
    [InlineData("Backstage passes to a TAFKAL80ETC concert", 20, 10, 22, 9)]
    [InlineData("Backstage", 20, 5, 23, 4)]
    [InlineData("Conjured Mana Cake", 10, 5, 8, 4)]
    [InlineData("Conjured", 10, 0, 6, -1)]
    [InlineData("Sulfuras, Hand of Ragnaros", 80, 0, 80, 0)]
    [InlineData("Vintage Wine", 10, 20, 11, 19)]
    [InlineData("Vintage Wine", 10, 11, 11, 10)]
    [InlineData("Vintage Wine", 10, 10, 12, 9)]
    [InlineData("Vintage Wine", 10, 1, 12, 0)]
    [InlineData("Vintage Wine", 10, 0, 12, -1)]
    [InlineData("Vintage", 10, -5, 12, -6)]
    [InlineData("Vintage Wine", 50, 20, 50, 19)]
    [InlineData("Vintage Wine", 49, 5, 50, 4)]
    public void Mature_UsesExpectedRulesForResolvedItemKinds(string name, int quality, int sellIn, int expectedQuality, int expectedSellIn)
    {
        var service = new ItemMaturationService();
        var item = new Item
        {
            Name = name,
            Quality = quality,
            SellIn = sellIn,
        };

        var result = service.Mature(item);

        result.Quality.Should().Be(expectedQuality);
        result.SellIn.Should().Be(expectedSellIn);
    }

    [Fact]
    public void Mature_WhenItemIsVintageWine_PreservesName()
    {
        var service = new ItemMaturationService();
        var item = new Item
        {
            Name = "Vintage Wine",
            Quality = 10,
            SellIn = 20,
        };

        var result = service.Mature(item);

        result.Name.Should().Be("Vintage Wine");
    }
}
