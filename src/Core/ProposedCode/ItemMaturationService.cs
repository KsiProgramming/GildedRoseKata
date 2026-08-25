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
    private readonly IMaturingPolicyCatalog policyCatalog;

    public ItemMaturationService(IEnumerable<IMaturingPolicy> policies)
        : this(new ItemKindResolver(), new MaturingPolicyCatalog(policies))
    {
    }

    public ItemMaturationService(IItemKindResolver itemKindResolver, IMaturingPolicyCatalog policyCatalog)
    {
        this.itemKindResolver = itemKindResolver;
        this.policyCatalog = policyCatalog;
    }

    public Item Mature(Item item)
    {
        var kind = this.itemKindResolver.Resolve(item.Name);
        var policy = this.policyCatalog.GetFor(kind);
        var state = ItemState.Create(kind, item.Quality, item.SellIn);
        var maturedState = policy.Mature(state);

        return new()
        {
            Name = item.Name,
            Quality = maturedState.Quality.Value,
            SellIn = maturedState.SellIn.Value,
        };
    }
}