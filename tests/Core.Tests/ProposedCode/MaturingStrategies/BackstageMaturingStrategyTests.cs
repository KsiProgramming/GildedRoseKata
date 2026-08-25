//-----------------------------------------------------------------------
// <copyright file="BackstageMaturingStrategyTests.cs" company="KsiProgramming">
//     Copyright (c) KsiProgramming. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace GildedRoseKata.ProposedCode.MaturingStrategies.Tests;

using FluentAssertions;

public class BackstageMaturingStrategyTests
{
    [Fact]
    public void Mature_WhenQualityIsAtMaximum_KeepsMaximum()
    {
        var state = new ItemState(new Quality(50), new SellIn(5));

        var strategy = new BackstageMaturingStrategy();

        var result = strategy.Mature(state);

        result.Quality.Value.Should().Be(50);
        result.SellIn.Value.Should().Be(4);
    }

    [Fact]
    public void Mature_WhenSellInStartsAtOne_DoesNotDropToZeroEarly()
    {
        var state = new ItemState(new Quality(20), new SellIn(1));

        var strategy = new BackstageMaturingStrategy();

        var result = strategy.Mature(state);

        result.Quality.Value.Should().Be(23);
        result.SellIn.Value.Should().Be(0);
    }

    [Fact]
    public void Mature_WhenSellInIsAboveTen_IncreasesByOne()
    {
        var state = new ItemState(new Quality(20), new SellIn(15));

        var strategy = new BackstageMaturingStrategy();

        var result = strategy.Mature(state);

        result.Quality.Value.Should().Be(21);
        result.SellIn.Value.Should().Be(14);
    }

    [Fact]
    public void Mature_WhenSellInIsTenOrLess_IncreasesByTwo()
    {
        var state = new ItemState(new Quality(20), new SellIn(10));

        var strategy = new BackstageMaturingStrategy();

        var result = strategy.Mature(state);

        result.Quality.Value.Should().Be(22);
        result.SellIn.Value.Should().Be(9);
    }

    [Fact]
    public void Mature_WhenSellInIsFiveOrLess_IncreasesByThree()
    {
        var state = new ItemState(new Quality(20), new SellIn(4));

        var strategy = new BackstageMaturingStrategy();

        var result = strategy.Mature(state);

        result.Quality.Value.Should().Be(23);
        result.SellIn.Value.Should().Be(3);
    }

    [Fact]
    public void Mature_WhenSellInBecomesExpired_DropsToZero()
    {
        var state = new ItemState(new Quality(20), new SellIn(0));

        var strategy = new BackstageMaturingStrategy();

        var result = strategy.Mature(state);

        result.Quality.Value.Should().Be(0);
        result.SellIn.Value.Should().Be(-1);
    }
}
