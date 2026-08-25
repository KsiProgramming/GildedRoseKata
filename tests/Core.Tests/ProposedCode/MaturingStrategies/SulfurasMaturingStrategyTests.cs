//-----------------------------------------------------------------------
// <copyright file="SulfurasMaturingStrategyTests.cs" company="KsiProgramming">
//     Copyright (c) KsiProgramming. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace GildedRoseKata.ProposedCode.MaturingStrategies.Tests;

using FluentAssertions;

public class SulfurasMaturingStrategyTests
{
    [Fact]
    public void Mature_PreservesProvidedState()
    {
        var state = new ItemState(new Quality(80), new SellIn(0));

        var strategy = new SulfurasMaturingStrategy();

        var result = strategy.Mature(state);

        result.Should().Be(state);
    }
}