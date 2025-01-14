//-----------------------------------------------------------------------
// <copyright file="AgedBrieMaturingStrategy.cs" company="KsiProgramming">
//     Copyright (c) KsiProgramming. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace GildedRoseKata.ProposedCode.MaturingStrategies;

public class AgedBrieMaturingStrategy : IItemMaturingStrategy
{
    public ItemMaturingResponse Update(ItemMaturingRequest request)
    {
        var newQuality = request.Quality;
        if (newQuality.IsBelowLimit(50))
        {
            newQuality = newQuality.Increase();
        }

        var newSellIn = request.SellIn.Decrease();

        if (newQuality.IsBelowLimit(50) && newSellIn.HasExpired())
        {
            newQuality = newQuality.Increase();
        }

        return new(newQuality, newSellIn);
    }
}
