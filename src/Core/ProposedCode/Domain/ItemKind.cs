//-----------------------------------------------------------------------
// <copyright file="ItemKind.cs" company="KsiProgramming">
//     Copyright (c) KsiProgramming. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace GildedRoseKata.ProposedCode;

public abstract record ItemKind
{
    public sealed record Standard : ItemKind;

    public sealed record AgedBrie : ItemKind;

    public sealed record Backstage : ItemKind;

    public sealed record Conjured : ItemKind;

    public sealed record Sulfuras : ItemKind;

    public static ItemKind FromName(string itemName)
    {
        return itemName switch
        {
            "Aged Brie" => new AgedBrie(),
            "AgedBrie" => new AgedBrie(),
            "Backstage passes to a TAFKAL80ETC concert" => new Backstage(),
            "Backstage" => new Backstage(),
            "Conjured Mana Cake" => new Conjured(),
            "Conjured" => new Conjured(),
            "Sulfuras, Hand of Ragnaros" => new Sulfuras(),
            _ => new Standard(),
        };
    }
}