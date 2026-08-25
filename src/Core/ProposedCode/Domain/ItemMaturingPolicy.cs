//-----------------------------------------------------------------------
// <copyright file="ItemMaturingPolicy.cs" company="KsiProgramming">
//     Copyright (c) KsiProgramming. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace GildedRoseKata.ProposedCode;

public class ItemMaturingPolicy : IMaturingPolicy
{
    public ItemState Mature(ItemKind kind, ItemState state)
    {
        return kind switch
        {
            ItemKind.AgedBrie => this.MatureAgedBrie(state),
            ItemKind.Backstage => this.MatureBackstage(state),
            ItemKind.Conjured => this.MatureConjured(state),
            ItemKind.Sulfuras => state,
            _ => this.MatureStandard(state),
        };
    }

    private ItemState MatureStandard(ItemState state)
    {
        var newQuality = state.Quality.Decrease();
        var newSellIn = state.SellIn.NextDay();

        if (newSellIn.IsExpired)
        {
            newQuality = newQuality.Decrease();
        }

        return new(newQuality, newSellIn);
    }

    private ItemState MatureAgedBrie(ItemState state)
    {
        var newQuality = state.Quality.Increase();
        var newSellIn = state.SellIn.NextDay();

        if (newSellIn.IsExpired)
        {
            newQuality = newQuality.Increase();
        }

        return new(newQuality, newSellIn);
    }

    private ItemState MatureBackstage(ItemState state)
    {
        var newQuality = state.SellIn switch
        {
            _ when state.SellIn.IsBelow(6) => state.Quality.Increase(3),
            _ when state.SellIn.IsBelow(11) => state.Quality.Increase(2),
            _ => state.Quality.Increase(),
        };

        var newSellIn = state.SellIn.NextDay();

        if (newSellIn.IsExpired)
        {
            newQuality = newQuality.Reset();
        }

        return new(newQuality, newSellIn);
    }

    private ItemState MatureConjured(ItemState state)
    {
        var newQuality = state.Quality.Decrease(2);
        var newSellIn = state.SellIn.NextDay();

        if (newSellIn.IsExpired)
        {
            newQuality = newQuality.Decrease(2);
        }

        return new(newQuality, newSellIn);
    }
}