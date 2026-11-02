//-----------------------------------------------------------------------
// <copyright file="UpdateQualityServiceTests.cs" company="KsiProgramming">
//     Copyright (c) KsiProgramming. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace GildedRoseKata.OriginalCode.Tests;

using FluentAssertions;
using GildedRoseKata.OriginalCode;

/// <summary>
/// Characterisation tests that pin the current behaviour of <see cref="UpdateQualityService"/>.
/// </summary>
public class UpdateQualityServiceTests
{
    private const string AgedBrie = "Aged Brie";
    private const string Sulfuras = "Sulfuras, Hand of Ragnaros";
    private const string BackstagePasses = "Backstage passes to a TAFKAL80ETC concert";
    private const string NormalItem = "Elixir of the Mongoose";
    private const string ConjuredItem = "Conjured Mana Cake";

    [Theory]
    [InlineData(10, 5, 9, 4)]
    [InlineData(1, 1, 0, 0)]
    [InlineData(50, 5, 49, 4)]
    public void UpdateQuality_NormalItem_DecreasesQualityAndSellInByOne(int quality, int sellIn, int expectedQuality, int expectedSellIn)
    {
        var item = UpdateSingle(NormalItem, quality, sellIn);

        AssertItem(item, expectedQuality, expectedSellIn);
    }

    [Theory]
    [InlineData(10, 0, 8, -1)]
    [InlineData(10, -1, 8, -2)]
    public void UpdateQuality_NormalItem_DegradesTwiceAsFastAfterSellDate(int quality, int sellIn, int expectedQuality, int expectedSellIn)
    {
        var item = UpdateSingle(NormalItem, quality, sellIn);

        AssertItem(item, expectedQuality, expectedSellIn);
    }

    [Theory]
    [InlineData(0, 5, 0, 4)]
    [InlineData(1, 0, 0, -1)]
    [InlineData(0, 0, 0, -1)]
    public void UpdateQuality_NormalItem_QualityNeverNegative(int quality, int sellIn, int expectedQuality, int expectedSellIn)
    {
        var item = UpdateSingle(NormalItem, quality, sellIn);

        AssertItem(item, expectedQuality, expectedSellIn);
    }

    [Fact]
    public void UpdateQuality_AgedBrie_IncreasesQualityByOneBeforeSellDate()
    {
        var item = UpdateSingle(AgedBrie, 10, 5);

        AssertItem(item, 11, 4);
    }

    [Theory]
    [InlineData(10, 0, 12, -1)]
    [InlineData(10, -1, 12, -2)]
    public void UpdateQuality_AgedBrie_IncreasesQualityByTwoAfterSellDate(int quality, int sellIn, int expectedQuality, int expectedSellIn)
    {
        var item = UpdateSingle(AgedBrie, quality, sellIn);

        AssertItem(item, expectedQuality, expectedSellIn);
    }

    [Theory]
    [InlineData(50, 5, 50, 4)]
    [InlineData(49, 0, 50, -1)]
    [InlineData(50, 0, 50, -1)]
    [InlineData(48, 0, 50, -1)]
    public void UpdateQuality_AgedBrie_QualityNeverExceedsFifty(int quality, int sellIn, int expectedQuality, int expectedSellIn)
    {
        var item = UpdateSingle(AgedBrie, quality, sellIn);

        AssertItem(item, expectedQuality, expectedSellIn);
    }

    [Theory]
    [InlineData(80, 0)]
    [InlineData(80, 5)]
    [InlineData(80, -1)]
    public void UpdateQuality_Sulfuras_NeverChanges(int quality, int sellIn)
    {
        var item = UpdateSingle(Sulfuras, quality, sellIn);

        AssertItem(item, quality, sellIn);
    }

    [Theory]
    [InlineData(20, 11, 21, 10)]
    [InlineData(20, 15, 21, 14)]
    public void UpdateQuality_BackstagePasses_IncreasesByOneWhenMoreThanTenDaysLeft(int quality, int sellIn, int expectedQuality, int expectedSellIn)
    {
        var item = UpdateSingle(BackstagePasses, quality, sellIn);

        AssertItem(item, expectedQuality, expectedSellIn);
    }

    [Theory]
    [InlineData(20, 10, 22, 9)]
    [InlineData(20, 6, 22, 5)]
    public void UpdateQuality_BackstagePasses_IncreasesByTwoWhenTenToSixDaysLeft(int quality, int sellIn, int expectedQuality, int expectedSellIn)
    {
        var item = UpdateSingle(BackstagePasses, quality, sellIn);

        AssertItem(item, expectedQuality, expectedSellIn);
    }

    [Theory]
    [InlineData(20, 5, 23, 4)]
    [InlineData(20, 1, 23, 0)]
    public void UpdateQuality_BackstagePasses_IncreasesByThreeWhenFiveOrFewerDaysLeft(int quality, int sellIn, int expectedQuality, int expectedSellIn)
    {
        var item = UpdateSingle(BackstagePasses, quality, sellIn);

        AssertItem(item, expectedQuality, expectedSellIn);
    }

    [Theory]
    [InlineData(49, 10, 50, 9)]
    [InlineData(48, 5, 50, 4)]
    [InlineData(50, 5, 50, 4)]
    public void UpdateQuality_BackstagePasses_QualityNeverExceedsFifty(int quality, int sellIn, int expectedQuality, int expectedSellIn)
    {
        var item = UpdateSingle(BackstagePasses, quality, sellIn);

        AssertItem(item, expectedQuality, expectedSellIn);
    }

    [Theory]
    [InlineData(20, 0, 0, -1)]
    [InlineData(20, -1, 0, -2)]
    [InlineData(50, 0, 0, -1)]
    [InlineData(0, 0, 0, -1)]
    public void UpdateQuality_BackstagePasses_QualityDropsToZeroAfterConcert(int quality, int sellIn, int expectedQuality, int expectedSellIn)
    {
        var item = UpdateSingle(BackstagePasses, quality, sellIn);

        AssertItem(item, expectedQuality, expectedSellIn);
    }

    /// <summary>
    /// The README says conjured items should degrade twice as fast, but the original code
    /// has no special handling for them. This test documents the current behaviour, not the required one.
    /// </summary>
    [Theory]
    [InlineData(10, 5, 9, 4)]
    [InlineData(10, 0, 8, -1)]
    public void UpdateQuality_ConjuredItem_BehavesLikeNormalItem(int quality, int sellIn, int expectedQuality, int expectedSellIn)
    {
        var item = UpdateSingle(ConjuredItem, quality, sellIn);

        AssertItem(item, expectedQuality, expectedSellIn);
    }

    [Fact]
    public void UpdateQuality_MultipleItems_UpdatesEachIndependently()
    {
        var normal = CreateItem(NormalItem, 10, 5);
        var brie = CreateItem(AgedBrie, 10, 5);
        var sulfuras = CreateItem(Sulfuras, 80, 5);
        var backstage = CreateItem(BackstagePasses, 20, 10);
        var conjured = CreateItem(ConjuredItem, 10, 5);
        var service = new UpdateQualityService(new List<Item> { normal, brie, sulfuras, backstage, conjured });

        service.UpdateQuality();

        AssertItem(normal, 9, 4);
        AssertItem(brie, 11, 4);
        AssertItem(sulfuras, 80, 5);
        AssertItem(backstage, 22, 9);
        AssertItem(conjured, 9, 4);
    }

    [Fact]
    public void UpdateQuality_EmptyList_DoesNotThrow()
    {
        var service = new UpdateQualityService(new List<Item>());

        var act = () => service.UpdateQuality();

        act.Should().NotThrow();
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

    private static Item UpdateSingle(string name, int quality, int sellIn)
    {
        var item = CreateItem(name, quality, sellIn);
        var service = new UpdateQualityService(new List<Item> { item });

        service.UpdateQuality();

        return item;
    }

    private static void AssertItem(Item item, int expectedQuality, int expectedSellIn)
    {
        item.Quality.Should().Be(expectedQuality);
        item.SellIn.Should().Be(expectedSellIn);
    }
}
