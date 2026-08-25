//-----------------------------------------------------------------------
// <copyright file="ConjuredMaturingStrategyTests.cs" company="KsiProgramming">
//     Copyright (c) KsiProgramming. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace GildedRoseKata.ProposedCode.MaturingStrategies.Tests;

using FluentAssertions;

public class ConjuredMaturingStrategyTests
{
    [Fact]
    public void Mature_WhenQualityIsAtMinimum_KeepsMinimum()
    {
        var state = new ItemState(new Quality(0), new SellIn(5));

        var strategy = new ConjuredMaturingStrategy();

        var result = strategy.Mature(state);

        result.Quality.Value.Should().Be(0);
        result.SellIn.Value.Should().Be(4);
    }

    [Fact]
    public void Mature_WhenQualityIsAboveMinimum_DecreasesByTwo()
    {
        var state = new ItemState(new Quality(5), new SellIn(5));

        var strategy = new ConjuredMaturingStrategy();

        var result = strategy.Mature(state);

        result.Quality.Value.Should().Be(3);
        result.SellIn.Value.Should().Be(4);
    }

    [Fact]
    public void Mature_WhenSellInBecomesExpired_DecreasesByFour()
    {
        var state = new ItemState(new Quality(5), new SellIn(0));

        var strategy = new ConjuredMaturingStrategy();

        var result = strategy.Mature(state);

        result.Quality.Value.Should().Be(1);
        result.SellIn.Value.Should().Be(-1);
    }
}
