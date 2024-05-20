//-----------------------------------------------------------------------
// <copyright file="ConjuredMaturingStrategy.cs" company="KsiProgramming">
//     Copyright (c) KsiProgramming. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace GildedRoseKata.ProposedCode.MaturingStrategies
{
    public class ConjuredMaturingStrategy : IItemMaturingStrategy
    {
        public ItemMaturingResponse Update(ItemMaturingRequest request)
        {
            var newQuality = request.quality;
            if (newQuality.IsAboveMinimum(0))
            {
                newQuality = newQuality.DecreaseBy(2);
            }

            var newSellIn = request.sellIn.Decrease();

            if (newSellIn.HasExpired() && newQuality.IsAboveMinimum(0))
            {
                newQuality = newQuality.DecreaseBy(2);
            }

            return new (newQuality, newSellIn);
        }
    }
}
