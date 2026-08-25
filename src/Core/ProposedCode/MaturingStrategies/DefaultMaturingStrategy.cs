//-----------------------------------------------------------------------
// <copyright file="DefaultMaturingStrategy.cs" company="KsiProgramming">
//     Copyright (c) KsiProgramming. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace GildedRoseKata.ProposedCode.MaturingStrategies;

public class DefaultMaturingStrategy : IMaturingPolicy
{
    public ItemCategory SupportedCategory => new ItemCategory.Standard();

    public ItemState Mature(ItemState state)
    {
        var newQuality = state.Quality.Decrease();
        var newSellIn = state.SellIn.NextDay();

        if (newSellIn.IsExpired)
        {
            newQuality = newQuality.Decrease();
        }

        return new(newQuality, newSellIn);
    }
}
