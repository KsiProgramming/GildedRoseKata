//-----------------------------------------------------------------------
// <copyright file="ItemState.cs" company="KsiProgramming">
//     Copyright (c) KsiProgramming. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace GildedRoseKata.ProposedCode;

public readonly record struct ItemState
{
    public const int LegendaryQuality = 80;

    public ItemState(Quality quality, SellIn sellIn)
    {
        this.Quality = quality;
        this.SellIn = sellIn;
    }

    public Quality Quality { get; }

    public SellIn SellIn { get; }

    public static ItemState Create(ItemKind kind, int quality, int sellIn)
    {
        var initialQuality = kind is ItemKind.Sulfuras
            ? Quality.Unclamped(LegendaryQuality)
            : new Quality(quality);

        return new ItemState(initialQuality, new SellIn(sellIn));
    }
}
