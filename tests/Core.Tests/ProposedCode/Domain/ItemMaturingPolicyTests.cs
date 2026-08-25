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

    [Fact]
    public void Mature_WhenKindIsStandardAndQualityIsAtMinimum_KeepsMinimum()
    {
        var state = new ItemState(new Quality(0), new SellIn(2));

        var result = this.policy.Mature(new ItemKind.Standard(), state);

        result.Quality.Value.Should().Be(0);
        result.SellIn.Value.Should().Be(1);
    }

    [Fact]
    public void Mature_WhenKindIsStandardAndSellInBecomesExpired_DecreasesByTwo()
    {
        var state = new ItemState(new Quality(2), new SellIn(0));

        var result = this.policy.Mature(new ItemKind.Standard(), state);

        result.Quality.Value.Should().Be(0);
        result.SellIn.Value.Should().Be(-1);
    }

    [Fact]
    public void Mature_WhenKindIsAgedBrieAndQualityIsAtMaximum_KeepsMaximum()
    {
        var state = new ItemState(new Quality(50), new SellIn(4));

        var result = this.policy.Mature(new ItemKind.AgedBrie(), state);

        result.Quality.Value.Should().Be(50);
        result.SellIn.Value.Should().Be(3);
    }

    [Fact]
    public void Mature_WhenKindIsAgedBrieAndSellInBecomesExpired_IncreasesByTwo()
    {
        var state = new ItemState(new Quality(20), new SellIn(0));

        var result = this.policy.Mature(new ItemKind.AgedBrie(), state);

        result.Quality.Value.Should().Be(22);
        result.SellIn.Value.Should().Be(-1);
    }

    [Fact]
    public void Mature_WhenKindIsBackstageAndSellInStartsAtOne_DoesNotDropToZeroEarly()
    {
        var state = new ItemState(new Quality(20), new SellIn(1));

        var result = this.policy.Mature(new ItemKind.Backstage(), state);

        result.Quality.Value.Should().Be(23);
        result.SellIn.Value.Should().Be(0);
    }

    [Fact]
    public void Mature_WhenKindIsBackstageAndSellInBecomesExpired_DropsToZero()
    {
        var state = new ItemState(new Quality(20), new SellIn(0));

        var result = this.policy.Mature(new ItemKind.Backstage(), state);

        result.Quality.Value.Should().Be(0);
        result.SellIn.Value.Should().Be(-1);
    }

    [Fact]
    public void Mature_WhenKindIsConjuredAndSellInBecomesExpired_DecreasesByFour()
    {
        var state = new ItemState(new Quality(5), new SellIn(0));

        var result = this.policy.Mature(new ItemKind.Conjured(), state);

        result.Quality.Value.Should().Be(1);
        result.SellIn.Value.Should().Be(-1);
    }

    [Fact]
    public void Mature_WhenKindIsSulfuras_PreservesProvidedState()
    {
        var state = ItemState.Create(new ItemKind.Sulfuras(), 80, 0);

        var result = this.policy.Mature(new ItemKind.Sulfuras(), state);

        result.Should().Be(state);
    }
}