//-----------------------------------------------------------------------
// <copyright file="ItemMaturationServiceTests.cs" company="KsiProgramming">
//     Copyright (c) KsiProgramming. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace GildedRoseKata.ProposedCode.Tests;

using System.Linq;
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
    [InlineData("Mature Cheese", 10, 5, 11, 4)]
    [InlineData("Mature Cheese", 10, 1, 11, 0)]
    [InlineData("Mature Cheese", 10, 0, 12, -1)]
    [InlineData("Mature Cheese", 10, -1, 12, -2)]
    [InlineData("Mature Cheese", 49, 5, 50, 4)]
    [InlineData("Mature Cheese", 49, 0, 50, -1)]
    [InlineData("Mature Cheese", 50, 0, 50, -1)]
    [InlineData("Mature Cheese ", 10, 5, 9, 4)]
    [InlineData("mature cheese", 10, 5, 9, 4)]
    [InlineData("Mature Cheese Aged", 10, 5, 9, 4)]
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
    public void Mature_WhenInventoryContainsMultipleItems_MaturesEachItemIndependently()
    {
        var service = new ItemMaturationService();
        var items = new[]
        {
            new Item { Name = "Mature Cheese", Quality = 10, SellIn = 1 },
            new Item { Name = "Mature Cheese", Quality = 10, SellIn = 0 },
            new Item { Name = "Elixir of the Mongoose", Quality = 10, SellIn = 5 },
            new Item { Name = "Aged Brie", Quality = 10, SellIn = 2 },
        };

        var maturedItems = items.Select(service.Mature).ToArray();

        maturedItems.Select(item => (item.Name, item.Quality, item.SellIn)).Should().Equal(
            ("Mature Cheese", 11, 0),
            ("Mature Cheese", 12, -1),
            ("Elixir of the Mongoose", 9, 4),
            ("Aged Brie", 11, 1));
    }

    [Fact]
    public void Mature_WhenCalledOnTheSameItemAcrossDays_IsDeterministic()
    {
        var service = new ItemMaturationService();
        var firstRun = MatureForThreeDays(service, new Item { Name = "Mature Cheese", Quality = 48, SellIn = 2 });
        var secondRun = MatureForThreeDays(service, new Item { Name = "Mature Cheese", Quality = 48, SellIn = 2 });

        firstRun.Should().Equal(secondRun);
        firstRun.Should().Equal((1, 49), (0, 50), (-1, 50));
    }

    [Fact]
    public void Mature_DoesNotMutateTheInputItems()
    {
        var service = new ItemMaturationService();
        var items = new[]
        {
            new Item { Name = "Mature Cheese", Quality = 10, SellIn = 5 },
            new Item { Name = "Mature Cheese", Quality = 10, SellIn = 0 },
        };

        var results = items.Select(service.Mature).ToArray();

        results[0].Should().NotBeSameAs(items[0]);
        results[1].Should().NotBeSameAs(items[1]);
        items.Select(item => (item.Name, item.Quality, item.SellIn)).Should().Equal(
            ("Mature Cheese", 10, 5),
            ("Mature Cheese", 10, 0));
    }

    private static (int SellIn, int Quality)[] MatureForThreeDays(ItemMaturationService service, Item item)
    {
        var dailyResults = new (int SellIn, int Quality)[3];

        for (var day = 0; day < dailyResults.Length; day++)
        {
            item = service.Mature(item);
            dailyResults[day] = (item.SellIn, item.Quality);
        }

        return dailyResults;
    }
}
