//-----------------------------------------------------------------------
// <copyright file="ItemMaturationServiceTests.cs" company="KsiProgramming">
//     Copyright (c) KsiProgramming. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace GildedRoseKata.ProposedCode.Tests;

using System.Collections.Generic;
using FluentAssertions;
using GildedRoseKata.OriginalCode;

public class ItemMaturationServiceTests
{
    private const string MatureCheeseName = "Mature Cheese";

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
    [InlineData(10, 5, 11, 4)]
    [InlineData(10, 1, 11, 0)]
    [InlineData(10, 0, 12, -1)]
    [InlineData(10, -1, 12, -2)]
    [InlineData(49, 5, 50, 4)]
    [InlineData(49, 0, 50, -1)]
    [InlineData(50, 5, 50, 4)]
    [InlineData(50, 0, 50, -1)]
    public void Mature_MatureCheese_FollowsAcceptanceRules(int quality, int sellIn, int expectedQuality, int expectedSellIn)
    {
        var service = new ItemMaturationService();
        var item = NewItem(MatureCheeseName, quality, sellIn);

        var result = service.Mature(item);

        result.Name.Should().Be(MatureCheeseName);
        result.Quality.Should().Be(expectedQuality);
        result.SellIn.Should().Be(expectedSellIn);
    }

    [Theory]
    [InlineData(-5, 5)]
    [InlineData(-5, 0)]
    [InlineData(-5, -3)]
    [InlineData(60, 5)]
    [InlineData(60, 0)]
    [InlineData(60, -3)]
    public void Mature_MatureCheese_WithInvalidQuality_ReturnsQualityWithinRange(int quality, int sellIn)
    {
        var service = new ItemMaturationService();

        var result = service.Mature(NewItem(MatureCheeseName, quality, sellIn));

        result.Quality.Should().BeInRange(0, 50);
        result.SellIn.Should().Be(sellIn - 1);
    }

    [Theory]
    [InlineData(0, 10)]
    [InlineData(10, 5)]
    [InlineData(49, 0)]
    [InlineData(50, -5)]
    [InlineData(-5, 3)]
    [InlineData(60, -2)]
    public void Mature_MatureCheese_OverManyDays_KeepsQualityWithinRangeAndDecreasesSellInByOne(int quality, int sellIn)
    {
        var service = new ItemMaturationService();
        var item = NewItem(MatureCheeseName, quality, sellIn);

        for (var day = 1; day <= 60; day++)
        {
            var previousSellIn = item.SellIn;

            item = service.Mature(item);

            item.Quality.Should().BeInRange(0, 50);
            item.SellIn.Should().Be(previousSellIn - 1);
        }

        item.SellIn.Should().Be(sellIn - 60);
    }

    [Theory]
    [InlineData("mature cheese")]
    [InlineData("Mature Cheese Deluxe")]
    [InlineData("MatureCheese")]
    public void Mature_SimilarNames_FollowStandardRule(string name)
    {
        var service = new ItemMaturationService();

        var result = service.Mature(NewItem(name, 10, 5));

        result.Quality.Should().Be(9);
        result.SellIn.Should().Be(4);
    }

    [Fact]
    public void Mature_MixedInventory_AppliesEachProductsOwnRules()
    {
        var service = new ItemMaturationService();
        var inventory = new List<Item>
        {
            NewItem(MatureCheeseName, 10, 5),
            NewItem("Aged Brie", 10, 2),
            NewItem("Backstage passes to a TAFKAL80ETC concert", 20, 10),
            NewItem("Sulfuras, Hand of Ragnaros", 80, 0),
            NewItem("Conjured Mana Cake", 10, 5),
            NewItem("Elixir of the Mongoose", 10, 5),
        };
        var expected = new[]
        {
            (Quality: 11, SellIn: 4),
            (Quality: 11, SellIn: 1),
            (Quality: 22, SellIn: 9),
            (Quality: 80, SellIn: 0),
            (Quality: 8, SellIn: 4),
            (Quality: 9, SellIn: 4),
        };

        var results = new List<Item>();
        foreach (var item in inventory)
        {
            results.Add(service.Mature(item));
        }

        for (var i = 0; i < inventory.Count; i++)
        {
            results[i].Name.Should().Be(inventory[i].Name);
            results[i].Quality.Should().Be(expected[i].Quality, inventory[i].Name);
            results[i].SellIn.Should().Be(expected[i].SellIn, inventory[i].Name);

            if (inventory[i].Name != MatureCheeseName)
            {
                var again = new ItemMaturationService().Mature(NewItem(inventory[i].Name, inventory[i].Quality, inventory[i].SellIn));

                results[i].Quality.Should().Be(again.Quality, inventory[i].Name);
                results[i].SellIn.Should().Be(again.SellIn, inventory[i].Name);
            }
        }
    }

    [Fact]
    public void Mature_SeveralMatureCheeseItems_FollowRulesIndependentlyOverSeveralDays()
    {
        var service = new ItemMaturationService();
        var starts = new[]
        {
            (Quality: 10, SellIn: 3),
            (Quality: 0, SellIn: 1),
            (Quality: 45, SellIn: 0),
            (Quality: 49, SellIn: 5),
            (Quality: 50, SellIn: -2),
            (Quality: 20, SellIn: -1),
        };
        var items = new List<Item>();
        foreach (var start in starts)
        {
            items.Add(NewItem(MatureCheeseName, start.Quality, start.SellIn));
        }

        var expectedQualities = new int[starts.Length];
        var expectedSellIns = new int[starts.Length];
        for (var i = 0; i < starts.Length; i++)
        {
            expectedQualities[i] = starts[i].Quality;
            expectedSellIns[i] = starts[i].SellIn;
        }

        for (var day = 1; day <= 8; day++)
        {
            for (var i = 0; i < items.Count; i++)
            {
                var increase = expectedSellIns[i] > 0 ? 1 : 2;
                expectedQualities[i] = System.Math.Min(50, expectedQualities[i] + increase);
                expectedSellIns[i]--;

                items[i] = service.Mature(items[i]);

                items[i].Quality.Should().Be(expectedQualities[i], $"item {i} on day {day}");
                items[i].SellIn.Should().Be(expectedSellIns[i], $"item {i} on day {day}");
            }
        }
    }

    [Fact]
    public void Mature_DoesNotMutateInputItem_AndReturnsNewInstance()
    {
        var service = new ItemMaturationService();
        var item = NewItem(MatureCheeseName, 10, 5);

        var result = service.Mature(item);

        item.Name.Should().Be(MatureCheeseName);
        item.Quality.Should().Be(10);
        item.SellIn.Should().Be(5);
        result.Should().NotBeSameAs(item);
        result.Quality.Should().Be(11);
        result.SellIn.Should().Be(4);
    }

    [Fact]
    public void CreatingAndReadingMatureCheeseItem_DoesNotChangeQualityOrSellIn()
    {
        var item = NewItem(MatureCheeseName, 10, 5);

        var name = item.Name;
        var quality = item.Quality;
        var sellIn = item.SellIn;
        var text = item.ToString();

        name.Should().Be(MatureCheeseName);
        quality.Should().Be(10);
        sellIn.Should().Be(5);
        text.Should().NotBeNull();
        item.Quality.Should().Be(10);
        item.SellIn.Should().Be(5);
    }

    private static Item NewItem(string name, int quality, int sellIn)
    {
        return new Item
        {
            Name = name,
            Quality = quality,
            SellIn = sellIn,
        };
    }
}
