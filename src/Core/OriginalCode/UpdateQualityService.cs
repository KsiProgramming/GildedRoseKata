//-----------------------------------------------------------------------
// <copyright file="UpdateQualityService.cs" company="KsiProgramming">
//     Copyright (c) KsiProgramming. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace GildedRoseKata.OriginalCode;

public class UpdateQualityService
{
    private readonly IList<Item> items;

    public UpdateQualityService(IList<Item> items)
    {
        this.items = items;
    }

#pragma warning disable S1066, S1764
    public void UpdateQuality()
    {
        for (var i = 0; i < this.items.Count; i++)
        {
            if (this.items[i].Name != "Aged Brie" && this.items[i].Name != "Backstage passes to a TAFKAL80ETC concert")
            {
                if (this.items[i].Quality > 0)
                {
                    if (this.items[i].Name != "Sulfuras, Hand of Ragnaros")
                    {
                        this.items[i].Quality = this.items[i].Quality - 1;
                    }
                }
            }
            else
            {
                if (this.items[i].Quality < 50)
                {
                    this.items[i].Quality = this.items[i].Quality + 1;

                    if (this.items[i].Name == "Backstage passes to a TAFKAL80ETC concert")
                    {
                        if (this.items[i].SellIn < 11)
                        {
                            if (this.items[i].Quality < 50)
                            {
                                this.items[i].Quality = this.items[i].Quality + 1;
                            }
                        }

                        if (this.items[i].SellIn < 6)
                        {
                            if (this.items[i].Quality < 50)
                            {
                                this.items[i].Quality = this.items[i].Quality + 1;
                            }
                        }
                    }
                }
            }

            if (this.items[i].Name != "Sulfuras, Hand of Ragnaros")
            {
                this.items[i].SellIn = this.items[i].SellIn - 1;
            }

            if (this.items[i].SellIn < 0)
            {
                if (this.items[i].Name != "Aged Brie")
                {
                    if (this.items[i].Name != "Backstage passes to a TAFKAL80ETC concert")
                    {
                        if (this.items[i].Quality > 0)
                        {
                            if (this.items[i].Name != "Sulfuras, Hand of Ragnaros")
                            {
                                this.items[i].Quality = this.items[i].Quality - 1;
                            }
                        }
                    }
                    else
                    {
                        this.items[i].Quality = this.items[i].Quality - this.items[i].Quality;
                    }
                }
                else
                {
                    if (this.items[i].Quality < 50)
                    {
                        this.items[i].Quality = this.items[i].Quality + 1;
                    }
                }
            }
        }
    }
#pragma warning restore S1066, S1764
}
