//-----------------------------------------------------------------------
// <copyright file="MaturingItemManager.cs" company="KsiProgramming">
//     Copyright (c) KsiProgramming. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace GildedRoseKata.ProposedCode;

using GildedRoseKata.OriginalCode;

public class MaturingItemManager
{
    private readonly IEnumerable<IItemMaturingStrategy> strategies;

    public MaturingItemManager(IEnumerable<IItemMaturingStrategy> strategies)
    {
        this.strategies = strategies;
    }

    public Item Mature(Item item)
    {
        var strategy = this.strategies.SingleOrDefault(s => s.ItemName == item.Name);

        return strategy is { } ?
            ApplyStrategy(strategy, item)
            : item;
    }

    private Item ApplyStrategy(IItemMaturingStrategy strategy, Item item)
    {
        var request = new ItemMaturingRequest(new Quality(item.Quality), new SellIn(item.SellIn));
        var response = strategy.Update(request);

        return new()
        {
            Name = item.Name,
            Quality = response.Quality.Value,
            SellIn = response.SellIn.Value,
        };
    }
}
