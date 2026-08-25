//-----------------------------------------------------------------------
// <copyright file="ItemKindResolver.cs" company="KsiProgramming">
//     Copyright (c) KsiProgramming. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace GildedRoseKata.ProposedCode;

public class ItemKindResolver : IItemKindResolver
{
    public ItemKind Resolve(string itemName)
    {
        return itemName switch
        {
            "Aged Brie" => new ItemKind.AgedBrie(),
            "AgedBrie" => new ItemKind.AgedBrie(),
            "Backstage passes to a TAFKAL80ETC concert" => new ItemKind.Backstage(),
            "Backstage" => new ItemKind.Backstage(),
            "Conjured Mana Cake" => new ItemKind.Conjured(),
            "Conjured" => new ItemKind.Conjured(),
            "Sulfuras, Hand of Ragnaros" => new ItemKind.Sulfuras(),
            _ => new ItemKind.Standard(),
        };
    }
}