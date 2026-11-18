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
    [InlineData(20, 5, 21, 4)]
    [InlineData(20, 1, 21, 0)]
    [InlineData(20, 0, 22, -1)]
    [InlineData(30, -5, 32, -6)]
    [InlineData(20, -1, 22, -2)]
    [InlineData(49, 5, 50, 4)]
    [InlineData(49, 0, 50, -1)]
    [InlineData(48, 0, 50, -1)]
    [InlineData(48, 5, 49, 4)]
    [InlineData(50, 5, 50, 4)]
    [InlineData(50, 0, 50, -1)]
    [InlineData(0, 5, 1, 4)]
    [InlineData(0, 0, 2, -1)]
    public void Mature_WhenKindIsCellarAgedMead_FollowsExpectedRules(int quality, int sellIn, int expectedQuality, int expectedSellIn)
    {
        var state = new ItemState(new Quality(quality), new SellIn(sellIn));

        var result = this.policy.Mature(new ItemKind.CellarAgedMead(), state);

        result.Quality.Value.Should().Be(expectedQuality);
        result.SellIn.Value.Should().Be(expectedSellIn);
    }

    [Fact]
    public void Mature_WhenCellarAgedMeadIsUpdatedRepeatedly_AppliesEachDayInSequence()
    {
        var state = new ItemState(new Quality(20), new SellIn(2));

        for (var day = 0; day < 3; day++)
        {
            state = this.policy.Mature(new ItemKind.CellarAgedMead(), state);
        }

        state.Quality.Value.Should().Be(24);
        state.SellIn.Value.Should().Be(-1);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(48)]
    public void Mature_WhenCellarAgedMeadIsUpdatedRepeatedly_CapsQualityAtFifty(int startQuality)
    {
        var state = new ItemState(new Quality(startQuality), new SellIn(0));

        for (var day = 0; day < 30; day++)
        {
            state = this.policy.Mature(new ItemKind.CellarAgedMead(), state);
        }

        state.Quality.Value.Should().Be(50);
        state.SellIn.Value.Should().Be(-30);
    }

    [Fact]
    public void Mature_WhenCellarAgedMeadIsUpdatedTwiceFromSameState_ReturnsEqualResults()
    {
        var state = new ItemState(new Quality(30), new SellIn(1));

        var first = this.policy.Mature(new ItemKind.CellarAgedMead(), state);
        var second = this.policy.Mature(new ItemKind.CellarAgedMead(), state);

        second.Quality.Value.Should().Be(first.Quality.Value);
        second.SellIn.Value.Should().Be(first.SellIn.Value);
    }

    [Fact]
    public void Mature_WhenCellarAgedMeadIsUpdatedForManyDays_KeepsQualityWithinBounds()
    {
        var state = new ItemState(new Quality(0), new SellIn(10));

        for (var day = 0; day < 200; day++)
        {
            state = this.policy.Mature(new ItemKind.CellarAgedMead(), state);

            state.Quality.Value.Should().BeInRange(0, 50);
        }
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-5)]
    [InlineData(-100)]
    public void Mature_WhenCellarAgedMeadHasNegativeSellIn_NeverLowersQuality(int sellIn)
    {
        var state = new ItemState(new Quality(30), new SellIn(sellIn));

        var result = this.policy.Mature(new ItemKind.CellarAgedMead(), state);

        result.Quality.Value.Should().BeGreaterThan(30);
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
