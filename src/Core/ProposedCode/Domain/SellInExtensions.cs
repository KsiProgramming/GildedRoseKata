//-----------------------------------------------------------------------
// <copyright file="SellInExtensions.cs" company="KsiProgramming">
//     Copyright (c) KsiProgramming. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace GildedRoseKata.ProposedCode;

public static class SellInExtensions
{
    public static SellIn Decrease(this SellIn sellIn)
    {
        return sellIn with { Value = sellIn.Value - 1 };
    }

    public static bool HasExpired(this SellIn sellIn)
    {
        return sellIn.Value <= 0;
    }

    public static bool IsBelowLimit(this SellIn sellIn, int limit)
    {
        return sellIn.Value < limit;
    }
}
