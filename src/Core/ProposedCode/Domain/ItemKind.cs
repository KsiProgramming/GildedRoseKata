//-----------------------------------------------------------------------
// <copyright file="ItemKind.cs" company="KsiProgramming">
//     Copyright (c) KsiProgramming. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace GildedRoseKata.ProposedCode;

public abstract record ItemKind
{
    public const string AgedBrieName = "Aged Brie";

    public const string BackstageName = "Backstage passes to a TAFKAL80ETC concert";

    public const string ConjuredName = "Conjured Mana Cake";

    public const string SulfurasName = "Sulfuras, Hand of Ragnaros";

    // Short aliases, accepted in addition to the canonical names above.
    public const string AgedBrieAlias = "AgedBrie";

    public const string BackstageAlias = "Backstage";

    public const string ConjuredAlias = "Conjured";

    public sealed record Standard : ItemKind;

    public sealed record AgedBrie : ItemKind;

    public sealed record Backstage : ItemKind;

    public sealed record Conjured : ItemKind;

    public sealed record Sulfuras : ItemKind;

    public static ItemKind FromName(string itemName)
    {
        return itemName switch
        {
            AgedBrieName or AgedBrieAlias => new AgedBrie(),
            BackstageName or BackstageAlias => new Backstage(),
            ConjuredName or ConjuredAlias => new Conjured(),
            SulfurasName => new Sulfuras(),
            _ => new Standard(),
        };
    }
}
