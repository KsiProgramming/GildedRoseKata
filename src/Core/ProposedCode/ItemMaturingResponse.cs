//-----------------------------------------------------------------------
// <copyright file="ItemMaturingResponse.cs" company="KsiProgramming">
//     Copyright (c) KsiProgramming. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace GildedRoseKata.ProposedCode
{
    public record struct ItemMaturingResponse(Quality quality, SellIn sellIn);
}
