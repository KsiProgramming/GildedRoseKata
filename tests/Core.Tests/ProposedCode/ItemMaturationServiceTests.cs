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

    [Fact]
    public void AC001_SellInDecreasesOnEachConsecutiveUpdate()
    {
        var service = new ItemMaturationService();
        var item = CreateItem("Mature Cheese", 10, 5);

        item = service.Mature(item);
        item = service.Mature(item);

        item.SellIn.Should().Be(3);
        item.Quality.Should().Be(12);
    }

    [Theory]
    [InlineData(5, 4)]
    [InlineData(1, 0)]
    public void AC002_QualityIncreasesByOneBeforeSellBy(int sellIn, int expectedSellIn)
    {
        var result = new ItemMaturationService().Mature(CreateItem("Mature Cheese", 10, sellIn));

        result.SellIn.Should().Be(expectedSellIn);
        result.Quality.Should().Be(11);
    }

    [Theory]
    [InlineData(0, -1)]
    [InlineData(-1, -2)]
    public void AC003_QualityIncreasesByTwoOnOrAfterSellBy(int sellIn, int expectedSellIn)
    {
        var result = new ItemMaturationService().Mature(CreateItem("Mature Cheese", 10, sellIn));

        result.SellIn.Should().Be(expectedSellIn);
        result.Quality.Should().Be(12);
    }

    [Theory]
    [InlineData(49)]
    [InlineData(50)]
    public void AC004_QualityNeverExceedsFifty(int quality)
    {
        var result = new ItemMaturationService().Mature(CreateItem("Mature Cheese", quality, 5));

        result.Quality.Should().Be(50);
    }

    [Theory]
    [InlineData(-1, 1)]
    [InlineData(51, 50)]
    public void AC005_OutOfRangeQualityUsesEstablishedClamping(int quality, int expectedQuality)
    {
        var result = new ItemMaturationService().Mature(CreateItem("Mature Cheese", quality, 5));

        result.Quality.Should().Be(expectedQuality);
        result.Quality.Should().BeInRange(0, 50);
    }

    [Fact]
    public void AC006_OnlyExactMatureCheeseNameGetsItsRules()
    {
        var service = new ItemMaturationService();
        var exact = service.Mature(CreateItem("Mature Cheese", 10, 5));
        var similar = service.Mature(CreateItem("Mature Cheeses", 10, 5));

        (exact.Quality, exact.SellIn).Should().Be((11, 4));
        (similar.Quality, similar.SellIn).Should().Be((9, 4));
    }

    [Fact]
    public void AC007_MixedInventoryPreservesExistingProductRules()
    {
        var service = new ItemMaturationService();
        var results = new[]
        {
            service.Mature(CreateItem("Mature Cheese", 10, 5)),
            service.Mature(CreateItem("Aged Brie", 10, 2)),
            service.Mature(CreateItem("Backstage passes to a TAFKAL80ETC concert", 20, 10)),
            service.Mature(CreateItem("Sulfuras, Hand of Ragnaros", 80, 0)),
            service.Mature(CreateItem("Conjured Mana Cake", 10, 5)),
        };

        results.Select(item => (item.Quality, item.SellIn)).Should().Equal(
            (11, 4),
            (11, 1),
            (22, 9),
            (80, 0),
            (8, 4));
    }

    [Fact]
    public void AC008_DailyUpdateReturnsUpdatedItemWithoutMutatingInput()
    {
        var service = new ItemMaturationService();
        var original = CreateItem("Mature Cheese", 10, 5);

        var result = service.Mature(original);

        original.Quality.Should().Be(10);
        original.SellIn.Should().Be(5);
        result.Quality.Should().Be(11);
        result.SellIn.Should().Be(4);
    }

    [Fact]
    public void AC009_MultipleItemsMatureCorrectlyOverConsecutiveDays()
    {
        var service = new ItemMaturationService();
        var items = new[]
        {
            CreateItem("Mature Cheese", 10, 1),
            CreateItem("Mature Cheese", 49, 0),
        };

        for (var day = 0; day < 2; day++)
        {
            items = items.Select(service.Mature).ToArray();
        }

        items.Select(item => (item.Quality, item.SellIn)).Should().Equal(
            (13, -1),
            (50, -2));
    }

    [Fact]
    public void AC010_SameStartingItemProducesTheSpecifiedDeterministicResult()
    {
        var service = new ItemMaturationService();
        var first = service.Mature(CreateItem("Mature Cheese", 10, 5));
        var second = service.Mature(CreateItem("Mature Cheese", 10, 5));

        (first.Quality, first.SellIn).Should().Be((11, 4));
        (second.Quality, second.SellIn).Should().Be((11, 4));
    }

    private static Item CreateItem(string name, int quality, int sellIn)
    {
        return new Item
        {
            Name = name,
            Quality = quality,
            SellIn = sellIn,
        };
    }
}
