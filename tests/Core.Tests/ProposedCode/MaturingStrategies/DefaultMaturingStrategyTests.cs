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
            quality = new (0),
            sellIn = new (2),
        };

        var stretagy = new DefaultMaturingStrategy();

        var result = stretagy.Update(request);

        result.quality.value.Should().Be(0);
        result.sellIn.value.Should().Be(1);
    }

    [Fact]
    public void Update_WithQualityIsAboveMinimum()
    {
        var request = new ItemMaturingRequest
        {
            quality = new (1),
            sellIn = new (2),
        };

        var stretagy = new DefaultMaturingStrategy();

        var result = stretagy.Update(request);

        result.quality.value.Should().Be(0);
        result.sellIn.value.Should().Be(1);
    }

    [Fact]
    public void Update_WithQualityIsAboveMinimumAndSellInIsExpired()
    {
        var request = new ItemMaturingRequest
        {
            quality = new (1),
            sellIn = new (1),
        };

        var stretagy = new DefaultMaturingStrategy();

        var result = stretagy.Update(request);

        result.quality.value.Should().Be(0);
        result.sellIn.value.Should().Be(0);
    }
}
