//-----------------------------------------------------------------------
// <copyright file="AgedBrieMaturingStrategy.cs" company="KsiProgramming">
//     Copyright (c) KsiProgramming. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace GildedRoseKata.ProposedCode.MaturingStrategies;

public class AgedBrieMaturingStrategy : IMaturingPolicy
{
    public ItemKind SupportedKind => new ItemKind.AgedBrie();

    public ItemState Mature(ItemState state)
    {
        var newQuality = state.Quality.Increase();
        var newSellIn = state.SellIn.NextDay();

        if (newSellIn.IsExpired)
        {
            newQuality = newQuality.Increase();
        }

        return new(newQuality, newSellIn);
    }
}
