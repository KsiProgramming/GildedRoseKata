//-----------------------------------------------------------------------
// <copyright file="ItemMaturationServiceTests.cs" company="KsiProgramming">
//     Copyright (c) KsiProgramming. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace GildedRoseKata.ProposedCode.Tests;

using FluentAssertions;
using GildedRoseKata.OriginalCode;
using System.Linq;

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
    [InlineData("Mature Cheese", 50, 5, 50, 4)]
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
    public void Mature_ReturnsMaturedItemWithoutChangingInput()
    {
        var service = new ItemMaturationService();
        var item = new Item
        {
            Name = "Mature Cheese",
            Quality = 10,
            SellIn = 5,
        };

        var result = service.Mature(item);

        result.Should().NotBeSameAs(item);
        result.Quality.Should().Be(11);
        result.SellIn.Should().Be(4);
        item.Quality.Should().Be(10);
        item.SellIn.Should().Be(5);
    }

    [Fact]
    public void Mature_UpdatesMultipleItemsAcrossConsecutiveMixedInventoryDays()
    {
        var service = new ItemMaturationService();
        var inventory = new[]
        {
            new Item { Name = "Mature Cheese", Quality = 10, SellIn = 1 },
            new Item { Name = "Elixir of the Mongoose", Quality = 10, SellIn = 5 },
            new Item { Name = "Mature Cheese", Quality = 49, SellIn = 0 },
        };

        var firstDay = inventory.Select(service.Mature).ToArray();
        var secondDay = firstDay.Select(service.Mature).ToArray();

        firstDay[0].Quality.Should().Be(11);
        firstDay[0].SellIn.Should().Be(0);
        firstDay[1].Quality.Should().Be(9);
        firstDay[1].SellIn.Should().Be(4);
        firstDay[2].Quality.Should().Be(50);
        firstDay[2].SellIn.Should().Be(-1);

        secondDay[0].Quality.Should().Be(13);
        secondDay[0].SellIn.Should().Be(-1);
        secondDay[1].Quality.Should().Be(8);
        secondDay[1].SellIn.Should().Be(3);
        secondDay[2].Quality.Should().Be(50);
        secondDay[2].SellIn.Should().Be(-2);

        inventory[0].Quality.Should().Be(10);
        inventory[0].SellIn.Should().Be(1);
        inventory[2].Quality.Should().Be(49);
        inventory[2].SellIn.Should().Be(0);
    }
}
