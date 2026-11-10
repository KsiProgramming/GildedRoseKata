//-----------------------------------------------------------------------
// <copyright file="ItemMaturingPolicy.cs" company="KsiProgramming">
//     Copyright (c) KsiProgramming. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace GildedRoseKata.ProposedCode;

using System;

public class ItemMaturingPolicy : IMaturingPolicy
{
    public ItemState Mature(ItemKind kind, ItemState state)
    {
        return kind switch
        {
            ItemKind.AgedBrie => Age(state, s => s.Quality.Increase(), q => q.Increase()),
            ItemKind.Backstage => Age(state, BackstageDailyChange, q => q.Reset()),
            ItemKind.Conjured => Age(state, s => s.Quality.Decrease(2), q => q.Decrease(2)),
            ItemKind.Sulfuras => state,
            _ => Age(state, s => s.Quality.Decrease(), q => q.Decrease()),
        };
    }

    private static ItemState Age(
        ItemState state,
        Func<ItemState, Quality> dailyChange,
        Func<Quality, Quality> expiredChange)
    {
        var newQuality = dailyChange(state);
        var newSellIn = state.SellIn.NextDay();

        if (newSellIn.IsExpired)
        {
            newQuality = expiredChange(newQuality);
        }

        return new(newQuality, newSellIn);
    }

    private static Quality BackstageDailyChange(ItemState state)
    {
        return state.SellIn switch
        {
            _ when state.SellIn.IsBelow(6) => state.Quality.Increase(3),
            _ when state.SellIn.IsBelow(11) => state.Quality.Increase(2),
            _ => state.Quality.Increase(),
        };
    }
}
