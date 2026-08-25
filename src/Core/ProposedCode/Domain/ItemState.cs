//-----------------------------------------------------------------------
// <copyright file="ItemState.cs" company="KsiProgramming">
//     Copyright (c) KsiProgramming. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace GildedRoseKata.ProposedCode;

public readonly record struct ItemState
{
    public ItemState(Quality quality, SellIn sellIn)
    {
        this.Quality = quality;
        this.SellIn = sellIn;
    }

    public Quality Quality { get; }

    public SellIn SellIn { get; }
}