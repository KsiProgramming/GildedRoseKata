//-----------------------------------------------------------------------
// <copyright file="BackstageMaturingStrategy.cs" company="KsiProgramming">
//     Copyright (c) KsiProgramming. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace GildedRoseKata.ProposedCode.MaturingStrategies;

public class BackstageMaturingStrategy : IMaturingPolicy
{
    public ItemCategory SupportedCategory => new ItemCategory.Backstage();

    public ItemState Mature(ItemState state)
    {
        var newQuality = this.UpdateQuality(state.Quality, state.SellIn);

        var newSellIn = state.SellIn.NextDay();
        if (newSellIn.IsExpired)
        {
            newQuality = newQuality.Reset();
        }

        return new(newQuality, newSellIn);
    }

    private Quality UpdateQuality(Quality quality, SellIn sellIn)
    {
        if (sellIn.IsBelow(6))
        {
            return quality.Increase(3);
        }

        if (sellIn.IsBelow(11))
        {
            return quality.Increase(2);
        }

        return quality.Increase();
    }
}
