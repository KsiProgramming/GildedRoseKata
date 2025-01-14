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
    public void Update()
    {
        var request = new ItemMaturingRequest
        {
            Quality = new(0),
            SellIn = new(5),
        };

        var strategy = new ConjuredMaturingStrategy();

        var result = strategy.Update(request);

        result.Quality.Value.Should().Be(0);
        result.SellIn.Value.Should().Be(4);
    }

    [Fact]
    public void Update_WithQualityIsAboveMinimum()
    {
        var request = new ItemMaturingRequest
        {
            Quality = new(5),
            SellIn = new(5),
        };

        var strategy = new ConjuredMaturingStrategy();

        var result = strategy.Update(request);

        result.Quality.Value.Should().Be(3);
        result.SellIn.Value.Should().Be(4);
    }

    [Fact]
    public void Update_WithQualityIsAboveMinimumAndSellInIsExpired()
    {
        var request = new ItemMaturingRequest
        {
            Quality = new(5),
            SellIn = new(1),
        };

        var strategy = new ConjuredMaturingStrategy();

        var result = strategy.Update(request);

        result.Quality.Value.Should().Be(1);
        result.SellIn.Value.Should().Be(0);
    }
}
