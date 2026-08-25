//-----------------------------------------------------------------------
// <copyright file="MaturingItemManager.cs" company="KsiProgramming">
//     Copyright (c) KsiProgramming. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace GildedRoseKata.ProposedCode;

using GildedRoseKata.OriginalCode;

public class MaturingItemManager
{
    private readonly IItemCategoryResolver itemCategoryResolver;
    private readonly IMaturingPolicyCatalog policyCatalog;

    public MaturingItemManager(IEnumerable<IMaturingPolicy> policies)
        : this(new ItemCategoryResolver(), new MaturingPolicyCatalog(policies))
    {
    }

    public MaturingItemManager(IItemCategoryResolver itemCategoryResolver, IMaturingPolicyCatalog policyCatalog)
    {
        this.itemCategoryResolver = itemCategoryResolver;
        this.policyCatalog = policyCatalog;
    }

    public Item Mature(Item item)
    {
        var category = this.itemCategoryResolver.Resolve(item.Name);
        if (category is ItemCategory.Sulfuras)
        {
            return new()
            {
                Name = item.Name,
                Quality = item.Quality,
                SellIn = item.SellIn,
            };
        }

        var strategy = this.policyCatalog.GetFor(category);

        return this.ApplyStrategy(strategy, item);
    }

    private Item ApplyStrategy(IMaturingPolicy strategy, Item item)
    {
        var state = new ItemState(new Quality(item.Quality), new SellIn(item.SellIn));
        var maturedState = strategy.Mature(state);

        return new()
        {
            Name = item.Name,
            Quality = maturedState.Quality.Value,
            SellIn = maturedState.SellIn.Value,
        };
    }
}
