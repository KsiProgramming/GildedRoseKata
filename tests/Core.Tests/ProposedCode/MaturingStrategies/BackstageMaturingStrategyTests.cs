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
            quality = new (55),
            sellIn = new (5),
        };

        var strategy = new BackstageMaturingStrategy();

        var result = strategy.Update(request);

        result.quality.value.Should().Be(55);
        result.sellIn.value.Should().Be(4);
    }

    [Fact]
    public void Update_WithSellInIsExpired()
    {
        var request = new ItemMaturingRequest
        {
            quality = new (55),
            sellIn = new (1),
        };

        var strategy = new BackstageMaturingStrategy();

        var result = strategy.Update(request);

        result.quality.value.Should().Be(0);
        result.sellIn.value.Should().Be(0);
    }

    [Fact]
    public void Update_WithQualityIsBelow50AndSellInIsAbove11()
    {
        var request = new ItemMaturingRequest
        {
            quality = new (20),
            sellIn = new (15),
        };

        var strategy = new BackstageMaturingStrategy();

        var result = strategy.Update(request);

        result.quality.value.Should().Be(21);
        result.sellIn.value.Should().Be(14);
    }

    [Fact]
    public void Update_WithQualityIsBelow50AndSellInIsBetween11And6()
    {
        var request = new ItemMaturingRequest
        {
            quality = new (20),
            sellIn = new (10),
        };

        var strategy = new BackstageMaturingStrategy();

        var result = strategy.Update(request);

        result.quality.value.Should().Be(22);
        result.sellIn.value.Should().Be(09);
    }

    [Fact]
    public void Update_WithQualityIsBelow50AndSellInIsBetween6And2()
    {
        var request = new ItemMaturingRequest
        {
            quality = new (20),
            sellIn = new (4),
        };

        var strategy = new BackstageMaturingStrategy();

        var result = strategy.Update(request);

        result.quality.value.Should().Be(23);
        result.sellIn.value.Should().Be(03);
    }

    [Fact]
    public void Update_WithQualityIsBelow50AndSellInIsExpired()
    {
        var request = new ItemMaturingRequest
        {
            quality = new (20),
            sellIn = new (1),
        };

        var strategy = new BackstageMaturingStrategy();

        var result = strategy.Update(request);

        result.quality.value.Should().Be(0);
        result.sellIn.value.Should().Be(0);
    }
}
