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
    public void Update()
    {
        var request = new ItemMaturingRequest
        {
            Quality = new(55),
            SellIn = new(4),
        };

        var strategy = new AgedBrieMaturingStrategy();

        var result = strategy.Update(request);

        result.Quality.Value.Should().Be(55);
        result.SellIn.Value.Should().Be(3);
    }

    [Fact]
    public void Update_WithQualityBelowLimit50()
    {
        var request = new ItemMaturingRequest
        {
            Quality = new(20),
            SellIn = new(4),
        };

        var strategy = new AgedBrieMaturingStrategy();

        var result = strategy.Update(request);

        result.Quality.Value.Should().Be(21);
        result.SellIn.Value.Should().Be(3);
    }

    [Fact]
    public void Update_WithQualityBelowLimit50AndSellInIsExpired()
    {
        var request = new ItemMaturingRequest
        {
            Quality = new(20),
            SellIn = new(0),
        };

        var strategy = new AgedBrieMaturingStrategy();

        var result = strategy.Update(request);

        result.Quality.Value.Should().Be(22);
        result.SellIn.Value.Should().Be(-1);
    }
}
