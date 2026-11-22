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
    [InlineData(5, 4)]
    [InlineData(0, -1)]
    [InlineData(-1, -2)]
    public void AC002_DecreasesSellInByOneEachDay(int sellIn, int expectedSellIn)
    {
        var state = new ItemState(new Quality(10), new SellIn(sellIn));

        var result = this.policy.Mature(ItemKind.FromName("Mature Cheese"), state);

        result.SellIn.Value.Should().Be(expectedSellIn);
    }

    [Theory]
    [InlineData(0, 5)]
    [InlineData(1, 5)]
    [InlineData(49, 5)]
    public void AC003_IncreasesQualityByOneBeforeSellBy(int quality, int sellIn)
    {
        var state = new ItemState(new Quality(quality), new SellIn(sellIn));

        var result = this.policy.Mature(ItemKind.FromName("Mature Cheese"), state);

        result.Quality.Value.Should().Be(quality + 1);
        result.SellIn.Value.Should().Be(sellIn - 1);
    }

    [Theory]
    [InlineData(10, 1, 11, 0)]
    [InlineData(10, 0, 12, -1)]
    [InlineData(10, -1, 12, -2)]
    public void AC004_IncreasesQualityByTwoAtOrAfterSellByUsingStartingSellIn(
        int quality,
        int sellIn,
        int expectedQuality,
        int expectedSellIn)
    {
        var state = new ItemState(new Quality(quality), new SellIn(sellIn));

        var result = this.policy.Mature(ItemKind.FromName("Mature Cheese"), state);

        result.Quality.Value.Should().Be(expectedQuality);
        result.SellIn.Value.Should().Be(expectedSellIn);
    }

    [Theory]
    [InlineData(49, 5, 50)]
    [InlineData(49, 0, 50)]
    [InlineData(50, 5, 50)]
    [InlineData(50, 0, 50)]
    public void AC005_CapsQualityAtFifty(int quality, int sellIn, int expectedQuality)
    {
        var state = new ItemState(new Quality(quality), new SellIn(sellIn));

        var result = this.policy.Mature(ItemKind.FromName("Mature Cheese"), state);

        result.Quality.Value.Should().Be(expectedQuality);
    }

    [Fact]
    public void AC010_AppliesNoMaximumAgePenalty()
    {
        var state = new ItemState(new Quality(10), new SellIn(-100));

        var result = this.policy.Mature(ItemKind.FromName("Mature Cheese"), state);

        result.Quality.Value.Should().Be(12);
        result.SellIn.Value.Should().Be(-101);
    }

    [Fact]
    public void AC011_ProducesDeterministicResults()
    {
        var initialState = new ItemState(new Quality(48), new SellIn(1));
        var kind = ItemKind.FromName("Mature Cheese");

        var firstResult = this.policy.Mature(kind, initialState);
        var secondResult = this.policy.Mature(kind, initialState);

        firstResult.Should().Be(secondResult);
        firstResult.Quality.Value.Should().Be(49);
        firstResult.SellIn.Value.Should().Be(0);
    }

    [Fact]
    public void AC012_CoversRequiredMatureCheeseScenarios()
    {
        var kind = ItemKind.FromName("Mature Cheese");
        var scenarios = new (int Quality, int SellIn, int ExpectedQuality, int ExpectedSellIn)[]
        {
            (0, 5, 1, 4),
            (1, 5, 2, 4),
            (49, 5, 50, 4),
            (50, 5, 50, 4),
            (0, 0, 2, -1),
            (1, 0, 3, -1),
            (49, 0, 50, -1),
            (50, 0, 50, -1),
            (10, -1, 12, -2),
        };

        foreach (var scenario in scenarios)
        {
            var state = new ItemState(new Quality(scenario.Quality), new SellIn(scenario.SellIn));
            var result = this.policy.Mature(kind, state);

            result.Quality.Value.Should().Be(scenario.ExpectedQuality);
            result.SellIn.Value.Should().Be(scenario.ExpectedSellIn);
        }

        var countdown = new ItemState(new Quality(0), new SellIn(2));
        var dayOne = this.policy.Mature(kind, countdown);
        var dayTwo = this.policy.Mature(kind, dayOne);
        var dayThree = this.policy.Mature(kind, dayTwo);

        dayOne.Should().Be(new ItemState(new Quality(1), new SellIn(1)));
        dayTwo.Should().Be(new ItemState(new Quality(2), new SellIn(0)));
        dayThree.Should().Be(new ItemState(new Quality(4), new SellIn(-1)));

        this.policy.Mature(new ItemKind.AgedBrie(), new ItemState(new Quality(20), new SellIn(4)))
            .Should().Be(new ItemState(new Quality(21), new SellIn(3)));
        this.policy.Mature(new ItemKind.Backstage(), new ItemState(new Quality(20), new SellIn(5)))
            .Should().Be(new ItemState(new Quality(23), new SellIn(4)));
        this.policy.Mature(new ItemKind.Conjured(), new ItemState(new Quality(5), new SellIn(5)))
            .Should().Be(new ItemState(new Quality(3), new SellIn(4)));

        var sulfuras = ItemState.Create(new ItemKind.Sulfuras(), 80, 0);
        this.policy.Mature(new ItemKind.Sulfuras(), sulfuras).Should().Be(sulfuras);
    }

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
