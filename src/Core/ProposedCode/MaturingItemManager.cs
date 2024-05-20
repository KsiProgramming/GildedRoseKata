//-----------------------------------------------------------------------
// <copyright file="MaturingItemManager.cs" company="KsiProgramming">
//     Copyright (c) KsiProgramming. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace GildedRoseKata.ProposedCode
{
    using GildedRoseKata.OriginalCode;

    public class MaturingItemManager
    {
        private readonly IDictionary<string, IItemMaturingStrategy> strategies;

        public MaturingItemManager(IDictionary<string, IItemMaturingStrategy> strategies)
        {
            this.strategies = strategies;
        }

        public Item MatureItem(Item item)
        {
            if (this.strategies.TryGetValue(item.Name, out var strategy))
            {
                var request = new ItemMaturingRequest(new Quality(item.Quality), new SellIn(item.SellIn));
                var response = strategy.Update(request);

                return new ()
                {
                    Name = item.Name,
                    Quality = response.quality.value,
                    SellIn = response.sellIn.value,
                };
            }

            // Handle case where item type doesn't have a strategy
            return item;
        }
    }
}
