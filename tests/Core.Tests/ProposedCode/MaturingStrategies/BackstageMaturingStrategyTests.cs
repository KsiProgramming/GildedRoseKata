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
    public void Update()
    {
        var request = new ItemMaturingRequest
        {
            Quality = new(55),
            SellIn = new(5),
        };

        var strategy = new BackstageMaturingStrategy();

        var result = strategy.Update(request);

        result.Quality.Value.Should().Be(55);
        result.SellIn.Value.Should().Be(4);
    }

    [Fact]
    public void Update_WithSellInIsExpired()
    {
        var request = new ItemMaturingRequest
        {
            Quality = new(55),
            SellIn = new(1),
        };

        var strategy = new BackstageMaturingStrategy();

        var result = strategy.Update(request);

        result.Quality.Value.Should().Be(0);
        result.SellIn.Value.Should().Be(0);
    }

    [Fact]
    public void Update_WithQualityIsBelow50AndSellInIsAbove11()
    {
        var request = new ItemMaturingRequest
        {
            Quality = new(20),
            SellIn = new(15),
        };

        var strategy = new BackstageMaturingStrategy();

        var result = strategy.Update(request);

        result.Quality.Value.Should().Be(21);
        result.SellIn.Value.Should().Be(14);
    }

    [Fact]
    public void Update_WithQualityIsBelow50AndSellInIsBetween11And6()
    {
        var request = new ItemMaturingRequest
        {
            Quality = new(20),
            SellIn = new(10),
        };

        var strategy = new BackstageMaturingStrategy();

        var result = strategy.Update(request);

        result.Quality.Value.Should().Be(22);
        result.SellIn.Value.Should().Be(09);
    }

    [Fact]
    public void Update_WithQualityIsBelow50AndSellInIsBetween6And2()
    {
        var request = new ItemMaturingRequest
        {
            Quality = new(20),
            SellIn = new(4),
        };

        var strategy = new BackstageMaturingStrategy();

        var result = strategy.Update(request);

        result.Quality.Value.Should().Be(23);
        result.SellIn.Value.Should().Be(03);
    }

    [Fact]
    public void Update_WithQualityIsBelow50AndSellInIsExpired()
    {
        var request = new ItemMaturingRequest
        {
            Quality = new(20),
            SellIn = new(1),
        };

        var strategy = new BackstageMaturingStrategy();

        var result = strategy.Update(request);

        result.Quality.Value.Should().Be(0);
        result.SellIn.Value.Should().Be(0);
    }
}
