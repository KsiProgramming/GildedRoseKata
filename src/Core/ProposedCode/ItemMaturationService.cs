//-----------------------------------------------------------------------
// <copyright file="ItemMaturationService.cs" company="KsiProgramming">
//     Copyright (c) KsiProgramming. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace GildedRoseKata.ProposedCode;

using GildedRoseKata.OriginalCode;

public class ItemMaturationService
{
    private readonly IMaturingPolicy maturingPolicy;

    public ItemMaturationService()
        : this(new ItemMaturingPolicy())
    {
    }

    public ItemMaturationService(IMaturingPolicy maturingPolicy)
    {
        this.maturingPolicy = maturingPolicy;
    }

    public Item Mature(Item item)
    {
        var kind = ItemKind.FromName(item.Name);
        var state = ItemState.Create(kind, item.Quality, item.SellIn);
        var maturedState = this.maturingPolicy.Mature(kind, state);

        return new()
        {
            Name = item.Name,
            Quality = maturedState.Quality.Value,
            SellIn = maturedState.SellIn.Value,
        };
    }
}