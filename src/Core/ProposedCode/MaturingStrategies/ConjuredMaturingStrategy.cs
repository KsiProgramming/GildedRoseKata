//-----------------------------------------------------------------------
// <copyright file="ConjuredMaturingStrategy.cs" company="KsiProgramming">
//     Copyright (c) KsiProgramming. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace GildedRoseKata.ProposedCode.MaturingStrategies;

public class ConjuredMaturingStrategy : IMaturingPolicy
{
    public ItemCategory SupportedCategory => new ItemCategory.Conjured();

    public ItemState Mature(ItemState state)
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
