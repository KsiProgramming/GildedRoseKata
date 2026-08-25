//-----------------------------------------------------------------------
// <copyright file="ItemMaturationServiceTests.cs" company="KsiProgramming">
//     Copyright (c) KsiProgramming. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace GildedRoseKata.ProposedCode.Tests;

using FluentAssertions;
using GildedRoseKata.OriginalCode;
using GildedRoseKata.ProposedCode.MaturingStrategies;

public class ItemMaturationServiceTests
{
    private static readonly IMaturingPolicy[] Policies =
    [
        new DefaultMaturingStrategy(),
        new AgedBrieMaturingStrategy(),
        new BackstageMaturingStrategy(),
        new ConjuredMaturingStrategy(),
        new SulfurasMaturingStrategy(),
    ];

    [Fact]
    public void Mature_WhenItemNameIsUnknown_UsesStandardPolicy()
    {
        var service = new ItemMaturationService(Policies);
        var item = new Item
        {
            Name = "Elixir of the Mongoose",
            Quality = 10,
            SellIn = 5,
        };

        var result = service.Mature(item);

        result.Quality.Should().Be(9);
        result.SellIn.Should().Be(4);
    }

    [Fact]
    public void Mature_WhenItemIsAgedBrie_UsesSpecificPolicy()
    {
        var service = new ItemMaturationService(Policies);
        var item = new Item
        {
            Name = "Aged Brie",
            Quality = 10,
            SellIn = 2,
        };

        var result = service.Mature(item);

        result.Quality.Should().Be(11);
        result.SellIn.Should().Be(1);
    }

    [Fact]
    public void Mature_WhenItemIsSulfuras_LeavesItUntouched()
    {
        var service = new ItemMaturationService(Policies);
        var item = new Item
        {
            Name = "Sulfuras, Hand of Ragnaros",
            Quality = 80,
            SellIn = 0,
        };

        var result = service.Mature(item);

        result.Quality.Should().Be(80);
        result.SellIn.Should().Be(0);
    }
}