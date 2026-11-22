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

    [Theory]
    [InlineData(-10, 1)]
    [InlineData(60, 50)]
    public void AC006_NormalizesInvalidImportedQuality(int importedQuality, int expectedQuality)
    {
        var service = new ItemMaturationService();
        var item = new Item
        {
            Name = "Mature Cheese",
            Quality = importedQuality,
            SellIn = 5,
        };

        var result = service.Mature(item);

        result.Quality.Should().Be(expectedQuality);
        result.Quality.Should().BeInRange(0, 50);
    }

    [Fact]
    public void AC008_PreservesExistingProductBehaviorInMixedInventory()
    {
        var service = new ItemMaturationService();
        var items = new[]
        {
            new Item { Name = "Mature Cheese", Quality = 10, SellIn = 5 },
            new Item { Name = "Elixir of the Mongoose", Quality = 10, SellIn = 5 },
            new Item { Name = "Aged Brie", Quality = 10, SellIn = 2 },
            new Item { Name = "Backstage passes to a TAFKAL80ETC concert", Quality = 20, SellIn = 10 },
            new Item { Name = "Conjured Mana Cake", Quality = 10, SellIn = 5 },
            new Item { Name = "Sulfuras, Hand of Ragnaros", Quality = 80, SellIn = 0 },
        };

        var results = new[]
        {
            service.Mature(items[0]),
            service.Mature(items[1]),
            service.Mature(items[2]),
            service.Mature(items[3]),
            service.Mature(items[4]),
            service.Mature(items[5]),
        };

        results[0].Quality.Should().Be(11);
        results[0].SellIn.Should().Be(4);
        results[1].Quality.Should().Be(9);
        results[1].SellIn.Should().Be(4);
        results[2].Quality.Should().Be(11);
        results[2].SellIn.Should().Be(1);
        results[3].Quality.Should().Be(22);
        results[3].SellIn.Should().Be(9);
        results[4].Quality.Should().Be(8);
        results[4].SellIn.Should().Be(4);
        results[5].Quality.Should().Be(80);
        results[5].SellIn.Should().Be(0);
    }

    [Fact]
    public void AC009_LeavesInputUnchangedOutsideDailyUpdate()
    {
        var service = new ItemMaturationService();
        var item = new Item
        {
            Name = "Mature Cheese",
            Quality = 10,
            SellIn = 5,
        };

        var result = service.Mature(item);

        item.Name.Should().Be("Mature Cheese");
        item.Quality.Should().Be(10);
        item.SellIn.Should().Be(5);
        result.Should().NotBeSameAs(item);
    }

    [Fact]
    public void AC013_PreservesOriginalCodeBehavior()
    {
        var items = new[]
        {
            new Item { Name = "Elixir of the Mongoose", Quality = 10, SellIn = 0 },
            new Item { Name = "Aged Brie", Quality = 10, SellIn = 0 },
            new Item { Name = "Backstage passes to a TAFKAL80ETC concert", Quality = 20, SellIn = 5 },
            new Item { Name = "Sulfuras, Hand of Ragnaros", Quality = 80, SellIn = 0 },
        };
        var legacyItems = new[]
        {
            new Item { Name = items[0].Name, Quality = items[0].Quality, SellIn = items[0].SellIn },
            new Item { Name = items[1].Name, Quality = items[1].Quality, SellIn = items[1].SellIn },
            new Item { Name = items[2].Name, Quality = items[2].Quality, SellIn = items[2].SellIn },
            new Item { Name = items[3].Name, Quality = items[3].Quality, SellIn = items[3].SellIn },
        };

        new UpdateQualityService(legacyItems).UpdateQuality();
        var service = new ItemMaturationService();
        var results = new[]
        {
            service.Mature(items[0]),
            service.Mature(items[1]),
            service.Mature(items[2]),
            service.Mature(items[3]),
        };

        for (var index = 0; index < items.Length; index++)
        {
            results[index].Quality.Should().Be(legacyItems[index].Quality);
            results[index].SellIn.Should().Be(legacyItems[index].SellIn);
        }
    }
}
