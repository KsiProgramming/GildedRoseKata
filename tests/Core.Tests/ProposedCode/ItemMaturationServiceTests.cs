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
    public void Mature_UsesInjectedPolicyForCustomItemAndPreservesItemName()
    {
        const string itemName = "Custom item";
        var service = new ItemMaturationService(new CustomItemMaturingPolicy());
        var item = new Item
        {
            Name = itemName,
            Quality = 10,
            SellIn = 5,
        };

        var result = service.Mature(item);

        result.Name.Should().Be(itemName);
        result.Quality.Should().Be(17);
        result.SellIn.Should().Be(3);
    }

    private sealed class CustomItemMaturingPolicy : IMaturingPolicy
    {
        public ItemState Mature(ItemKind kind, ItemState state)
        {
            return kind switch
            {
                ItemKind.CustomItem => new ItemState(new Quality(17), new SellIn(3)),
                _ => state,
            };
        }
    }
}
