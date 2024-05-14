//-----------------------------------------------------------------------
// <copyright file="AgedBrieMaturingStrategy.cs" company="KsiProgramming">
//     Copyright (c) KsiProgramming. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace GildedRoseKata.ProposedCode.MaturingStrategies
{
    internal class AgedBrieMaturingStrategy : IItemMaturingStrategy
    {
        public ItemMaturingResponse UpdateQuality(ItemMaturingRequest request)
        {
            var newQuality = request.quality;
            if (newQuality.IsBelowLimit(50))
            {
                newQuality = newQuality.Increase();
            }

            var newSellIn = request.sellIn.Decrease();

            if (newQuality.IsBelowLimit(50) && newSellIn.HasExpired())
            {
                newQuality = newQuality.Increase();
            }

            return new (newQuality, newSellIn);
        }
    }
}
