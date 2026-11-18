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
    [InlineData("Cellar-Aged Mead", 20, 5, 21, 4)]
    [InlineData("Cellar-Aged Mead", 20, 0, 22, -1)]
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
    public void Mature_WhenInventoryIsMixed_MaturesEachItemIndependentlyWithoutMutatingInput()
    {
        var items = new[]
        {
            new Item { Name = "Cellar-Aged Mead", SellIn = 5, Quality = 20 },
            new Item { Name = "Elixir of the Mongoose", SellIn = 5, Quality = 10 },
            new Item { Name = "Cellar-Aged Mead", SellIn = 0, Quality = 20 },
            new Item { Name = "Aged Brie", SellIn = 2, Quality = 10 },
            new Item { Name = "Cellar-Aged Mead", SellIn = -5, Quality = 30 },
            new Item { Name = "Backstage passes to a TAFKAL80ETC concert", SellIn = 10, Quality = 20 },
            new Item { Name = "Cellar-Aged Mead", SellIn = 5, Quality = 49 },
            new Item { Name = "Conjured Mana Cake", SellIn = 5, Quality = 10 },
            new Item { Name = "Cellar-Aged Mead", SellIn = 0, Quality = 50 },
            new Item { Name = "Sulfuras, Hand of Ragnaros", SellIn = 0, Quality = 80 },
        };
        var expected = items
            .Select(item => new ItemMaturationService().Mature(item))
            .ToArray();
        var service = new ItemMaturationService();

        var results = items.Select(item => service.Mature(item)).ToArray();

        results.Should().HaveCount(items.Length);
        for (var i = 0; i < items.Length; i++)
        {
            results[i].Name.Should().Be(expected[i].Name);
            results[i].Quality.Should().Be(expected[i].Quality);
            results[i].SellIn.Should().Be(expected[i].SellIn);
        }

        var expectedInputs = new[]
        {
            (5, 20), (5, 10), (0, 20), (2, 10), (-5, 30), (10, 20), (5, 49), (5, 10), (0, 50), (0, 80),
        };
        for (var i = 0; i < items.Length; i++)
        {
            items[i].SellIn.Should().Be(expectedInputs[i].Item1);
            items[i].Quality.Should().Be(expectedInputs[i].Item2);
        }
    }

    [Theory]
    [InlineData("Elixir of the Mongoose", 10, 5, 9, 4)]
    [InlineData("Aged Brie", 10, 2, 11, 1)]
    [InlineData("Backstage passes to a TAFKAL80ETC concert", 20, 10, 22, 9)]
    [InlineData("Conjured Mana Cake", 10, 5, 8, 4)]
    [InlineData("Sulfuras, Hand of Ragnaros", 80, 0, 80, 0)]
    public void Mature_ForExistingKinds_PreservesKnownOutputs(string name, int quality, int sellIn, int expectedQuality, int expectedSellIn)
    {
        var service = new ItemMaturationService();
        var item = new Item { Name = name, Quality = quality, SellIn = sellIn };

        var result = service.Mature(item);

        result.Name.Should().Be(name);
        result.Quality.Should().Be(expectedQuality);
        result.SellIn.Should().Be(expectedSellIn);
    }

    [Fact]
    public void Mature_WhenNameIsMeadVariant_TreatsItemAsStandard()
    {
        var service = new ItemMaturationService();
        var item = new Item { Name = "Cellar-Aged Mead Reserve", Quality = 20, SellIn = 5 };

        var result = service.Mature(item);

        result.Quality.Should().Be(19);
        result.SellIn.Should().Be(4);
    }
}
