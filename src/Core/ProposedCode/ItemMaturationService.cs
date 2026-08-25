//-----------------------------------------------------------------------
// <copyright file="ItemMaturationService.cs" company="KsiProgramming">
//     Copyright (c) KsiProgramming. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace GildedRoseKata.ProposedCode;

using GildedRoseKata.OriginalCode;

public class ItemMaturationService
{
    private readonly IItemKindResolver itemKindResolver;
    private readonly IMaturingPolicy maturingPolicy;

    public ItemMaturationService()
        : this(new ItemKindResolver(), new ItemMaturingPolicy())
    {
    }

    public ItemMaturationService(IItemKindResolver itemKindResolver, IMaturingPolicy maturingPolicy)
    {
        this.itemKindResolver = itemKindResolver;
        this.maturingPolicy = maturingPolicy;
    }

    public Item Mature(Item item)
    {
        var kind = this.itemKindResolver.Resolve(item.Name);
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