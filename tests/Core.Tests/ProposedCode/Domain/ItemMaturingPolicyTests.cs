//-----------------------------------------------------------------------
// <copyright file="ItemMaturingPolicyTests.cs" company="KsiProgramming">
//     Copyright (c) KsiProgramming. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace GildedRoseKata.ProposedCode.Tests;

using FluentAssertions;

public class ItemMaturingPolicyTests
{
    private readonly ItemMaturingPolicy policy = new();

    [Theory]
    [InlineData(0, 2, 0, 1)]
    [InlineData(10, 5, 9, 4)]
    [InlineData(2, 0, 0, -1)]
    [InlineData(0, 0, 0, -1)]
    public void Mature_WhenKindIsStandard_FollowsExpectedRules(int quality, int sellIn, int expectedQuality, int expectedSellIn)
    {
        var state = new ItemState(new Quality(quality), new SellIn(sellIn));

        var result = this.policy.Mature(new ItemKind.Standard(), state);

        result.Quality.Value.Should().Be(expectedQuality);
        result.SellIn.Value.Should().Be(expectedSellIn);
    }

    [Theory]
    [InlineData(20, 4, 21, 3)]
    [InlineData(20, 0, 22, -1)]
    [InlineData(49, 0, 50, -1)]
    [InlineData(50, 4, 50, 3)]
    [InlineData(50, 0, 50, -1)]
    public void Mature_WhenKindIsAgedBrie_FollowsExpectedRules(int quality, int sellIn, int expectedQuality, int expectedSellIn)
    {
        var state = new ItemState(new Quality(quality), new SellIn(sellIn));

        var result = this.policy.Mature(new ItemKind.AgedBrie(), state);

        result.Quality.Value.Should().Be(expectedQuality);
        result.SellIn.Value.Should().Be(expectedSellIn);
    }

    [Theory]
    [InlineData(20, 15, 21, 14)]
    [InlineData(20, 11, 21, 10)]
    [InlineData(20, 10, 22, 9)]
    [InlineData(20, 6, 22, 5)]
    [InlineData(20, 5, 23, 4)]
    [InlineData(20, 1, 23, 0)]
    [InlineData(20, 0, 0, -1)]
    [InlineData(49, 5, 50, 4)]
    [InlineData(49, 10, 50, 9)]
    [InlineData(49, 15, 50, 14)]
    [InlineData(50, 5, 50, 4)]
    public void Mature_WhenKindIsBackstage_FollowsExpectedRules(int quality, int sellIn, int expectedQuality, int expectedSellIn)
    {
        var state = new ItemState(new Quality(quality), new SellIn(sellIn));

        var result = this.policy.Mature(new ItemKind.Backstage(), state);

        result.Quality.Value.Should().Be(expectedQuality);
        result.SellIn.Value.Should().Be(expectedSellIn);
    }

    [Theory]
    [InlineData(0, 5, 0, 4)]
    [InlineData(5, 5, 3, 4)]
    [InlineData(5, 0, 1, -1)]
    [InlineData(1, 0, 0, -1)]
    public void Mature_WhenKindIsConjured_FollowsExpectedRules(int quality, int sellIn, int expectedQuality, int expectedSellIn)
    {
        var state = new ItemState(new Quality(quality), new SellIn(sellIn));

        var result = this.policy.Mature(new ItemKind.Conjured(), state);

        result.Quality.Value.Should().Be(expectedQuality);
        result.SellIn.Value.Should().Be(expectedSellIn);
    }

    [Fact]
    public void Mature_WhenKindIsSulfuras_PreservesProvidedState()
    {
        var state = ItemState.Create(new ItemKind.Sulfuras(), 80, 0);

        var result = this.policy.Mature(new ItemKind.Sulfuras(), state);

        result.Should().Be(state);
    }
}