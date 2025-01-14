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
    public void Update()
    {
        var request = new ItemMaturingRequest
        {
            Quality = new(0),
            SellIn = new(2),
        };

        var stretagy = new DefaultMaturingStrategy();

        var result = stretagy.Update(request);

        result.Quality.Value.Should().Be(0);
        result.SellIn.Value.Should().Be(1);
    }

    [Fact]
    public void Update_WithQualityIsAboveMinimum()
    {
        var request = new ItemMaturingRequest
        {
            Quality = new(1),
            SellIn = new(2),
        };

        var stretagy = new DefaultMaturingStrategy();

        var result = stretagy.Update(request);

        result.Quality.Value.Should().Be(0);
        result.SellIn.Value.Should().Be(1);
    }

    [Fact]
    public void Update_WithQualityIsAboveMinimumAndSellInIsExpired()
    {
        var request = new ItemMaturingRequest
        {
            Quality = new(1),
            SellIn = new(1),
        };

        var stretagy = new DefaultMaturingStrategy();

        var result = stretagy.Update(request);

        result.Quality.Value.Should().Be(0);
        result.SellIn.Value.Should().Be(0);
    }
}
