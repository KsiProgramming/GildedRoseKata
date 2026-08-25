//-----------------------------------------------------------------------
// <copyright file="AgedBrieMaturingStrategyTests.cs" company="KsiProgramming">
//     Copyright (c) KsiProgramming. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace GildedRoseKata.ProposedCode.MaturingStrategies.Tests;

using FluentAssertions;

public class AgedBrieMaturingStrategyTests
{
    [Fact]
    public void Mature_WhenQualityIsAtMaximum_KeepsMaximum()
    {
        var state = new ItemState(new Quality(50), new SellIn(4));

        var strategy = new AgedBrieMaturingStrategy();

        var result = strategy.Mature(state);

        result.Quality.Value.Should().Be(50);
        result.SellIn.Value.Should().Be(3);
    }

    [Fact]
    public void Mature_WhenQualityBelowMaximum_IncreasesByOne()
    {
        var state = new ItemState(new Quality(20), new SellIn(4));

        var strategy = new AgedBrieMaturingStrategy();

        var result = strategy.Mature(state);

        result.Quality.Value.Should().Be(21);
        result.SellIn.Value.Should().Be(3);
    }

    [Fact]
    public void Mature_WhenSellInBecomesExpired_IncreasesByTwo()
    {
        var state = new ItemState(new Quality(20), new SellIn(0));

        var strategy = new AgedBrieMaturingStrategy();

        var result = strategy.Mature(state);

        result.Quality.Value.Should().Be(22);
        result.SellIn.Value.Should().Be(-1);
    }
}
