//-----------------------------------------------------------------------
// <copyright file="DefaultMaturingStrategyTests.cs" company="KsiProgramming">
//     Copyright (c) KsiProgramming. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace GildedRoseKata.ProposedCode.MaturingStrategies.Tests;

using FluentAssertions;

public class DefaultMaturingStrategyTests
{
    [Fact]
    public void Mature_WhenQualityIsAtMinimum_KeepsMinimum()
    {
        var state = new ItemState(new Quality(0), new SellIn(2));

        var strategy = new DefaultMaturingStrategy();

        var result = strategy.Mature(state);

        result.Quality.Value.Should().Be(0);
        result.SellIn.Value.Should().Be(1);
    }

    [Fact]
    public void Mature_WhenQualityIsAboveMinimum_DecreasesByOne()
    {
        var state = new ItemState(new Quality(1), new SellIn(2));

        var strategy = new DefaultMaturingStrategy();

        var result = strategy.Mature(state);

        result.Quality.Value.Should().Be(0);
        result.SellIn.Value.Should().Be(1);
    }

    [Fact]
    public void Mature_WhenSellInBecomesExpired_DecreasesByTwo()
    {
        var state = new ItemState(new Quality(2), new SellIn(0));

        var strategy = new DefaultMaturingStrategy();

        var result = strategy.Mature(state);

        result.Quality.Value.Should().Be(0);
        result.SellIn.Value.Should().Be(-1);
    }
}
