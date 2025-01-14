//-----------------------------------------------------------------------
// <copyright file="BackstageMaturingStrategy.cs" company="KsiProgramming">
//     Copyright (c) KsiProgramming. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace GildedRoseKata.ProposedCode.MaturingStrategies;

public class BackstageMaturingStrategy : IItemMaturingStrategy
{
    public string ItemName => "Backstage";

    public ItemMaturingResponse Update(ItemMaturingRequest request)
    {
        var newQuality = this.UpdateQuality(request.Quality, request.SellIn);

        var newSellIn = request.SellIn.Decrease();
        if (newSellIn.HasExpired())
        {
            newQuality = new Quality(0);
        }

        return new(newQuality, newSellIn);
    }

    private Quality UpdateQuality(Quality quality, SellIn sellIn)
    {
        if (quality.IsBelowLimit(50) && sellIn.IsBelowLimit(6))
        {
            return quality.IncreaseBy(3);
        }

        if (quality.IsBelowLimit(50) && sellIn.IsBelowLimit(11))
        {
            return quality.IncreaseBy(2);
        }

        if (quality.IsBelowLimit(50))
        {
            return quality.Increase();
        }

        return quality;
    }
}
