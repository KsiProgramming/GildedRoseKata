//-----------------------------------------------------------------------
// <copyright file="DefaultMaturingStrategy.cs" company="KsiProgramming">
//     Copyright (c) KsiProgramming. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace GildedRoseKata.ProposedCode.MaturingStrategies
{
    public class DefaultMaturingStrategy : IItemMaturingStrategy
    {
        public ItemMaturingResponse Update(ItemMaturingRequest request)
        {
            var newQuality = request.quality;
            if (newQuality.IsAboveMinimum(0))
            {
                newQuality = newQuality.Decrease();
            }

            var newSellIn = request.sellIn.Decrease();

            if (newSellIn.HasExpired() && newQuality.IsAboveMinimum(0))
            {
                newQuality = newQuality.Decrease();
            }

            return new (newQuality, newSellIn);
        }
    }
}
