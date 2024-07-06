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
            quality = new (55),
            sellIn = new (4),
        };

        var strategy = new AgedBrieMaturingStrategy();

        var result = strategy.Update(request);

        result.quality.value.Should().Be(55);
        result.sellIn.value.Should().Be(3);
    }

    [Fact]
    public void Update_WithQualityBelowLimit50()
    {
        var request = new ItemMaturingRequest
        {
            quality = new (20),
            sellIn = new (4),
        };

        var strategy = new AgedBrieMaturingStrategy();

        var result = strategy.Update(request);

        result.quality.value.Should().Be(21);
        result.sellIn.value.Should().Be(3);
    }

    [Fact]
    public void Update_WithQualityBelowLimit50AndSellInIsExpired()
    {
        var request = new ItemMaturingRequest
        {
            quality = new (20),
            sellIn = new (0),
        };

        var strategy = new AgedBrieMaturingStrategy();

        var result = strategy.Update(request);

        result.quality.value.Should().Be(22);
        result.sellIn.value.Should().Be(-1);
    }
}
