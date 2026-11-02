//-----------------------------------------------------------------------
// <copyright file="UpdateQualityService.cs" company="KsiProgramming">
//     Copyright (c) KsiProgramming. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace GildedRoseKata.OriginalCode;

public class UpdateQualityService(IList<Item> items)
{
#pragma warning disable  
    private const string AgedBrieName = "Aged Brie";
    private const string SulfurasName = "Sulfuras, Hand of Ragnaros";
    private const string BackstagePassesName = "Backstage passes to a TAFKAL80ETC concert";
    private const int MaxQuality = 50;
    private const int MinQuality = 0;

    public void UpdateQuality()
    {
        foreach (var item in items)
        {
            switch (item.Name)
            {
                case AgedBrieName:
                    UpdateAgedBrie(item);
                    break;
                case SulfurasName:
                    UpdateSulfuras(item);
                    break;
                case BackstagePassesName:
                    UpdateBackstagePasses(item);
                    break;
                default:
                    UpdateNormalItem(item);
                    break;
            }
        }
    }

    private static void UpdateNormalItem(Item item)
    {
        DecreaseQuality(item);
        item.SellIn--;

        if (item.SellIn < 0)
        {
            DecreaseQuality(item);
        }
    }

    private static void UpdateAgedBrie(Item item)
    {
        IncreaseQuality(item);
        item.SellIn--;

        if (item.SellIn < 0)
        {
            IncreaseQuality(item);
        }
    }

    private static void UpdateSulfuras(Item item)
    {
        // Sulfuras never has to be sold and never decreases in quality: nothing changes.
    }

    private static void UpdateBackstagePasses(Item item)
    {
        // The tier checks use SellIn as it was before today's decrement.
        IncreaseQuality(item);

        if (item.SellIn < 11)
        {
            IncreaseQuality(item);
        }

        if (item.SellIn < 6)
        {
            IncreaseQuality(item);
        }

        item.SellIn--;

        if (item.SellIn < 0)
        {
            item.Quality = 0;
        }
    }

    private static void IncreaseQuality(Item item)
    {
        if (item.Quality < MaxQuality)
        {
            item.Quality++;
        }
    }

    private static void DecreaseQuality(Item item)
    {
        if (item.Quality > MinQuality)
        {
            item.Quality--;
        }
    }
#pragma warning restore
}
