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
            quality = new (0),
            sellIn = new (5),
        };

        var strategy = new ConjuredMaturingStrategy();

        var result = strategy.Update(request);

        result.quality.value.Should().Be(0);
        result.sellIn.value.Should().Be(4);
    }

    [Fact]
    public void Update_WithQualityIsAboveMinimum()
    {
        var request = new ItemMaturingRequest
        {
            quality = new (5),
            sellIn = new (5),
        };

        var strategy = new ConjuredMaturingStrategy();

        var result = strategy.Update(request);

        result.quality.value.Should().Be(3);
        result.sellIn.value.Should().Be(4);
    }

    [Fact]
    public void Update_WithQualityIsAboveMinimumAndSellInIsExpired()
    {
        var request = new ItemMaturingRequest
        {
            quality = new (5),
            sellIn = new (1),
        };

        var strategy = new ConjuredMaturingStrategy();

        var result = strategy.Update(request);

        result.quality.value.Should().Be(1);
        result.sellIn.value.Should().Be(0);
    }
}
