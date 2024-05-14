//-----------------------------------------------------------------------
// <copyright file="ItemMaturingRequest.cs" company="KsiProgramming">
//     Copyright (c) KsiProgramming. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace GildedRoseKata.ProposedCode
{
    public record struct ItemMaturingRequest(Quality quality, SellIn sellIn);
}
