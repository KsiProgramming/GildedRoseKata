//-----------------------------------------------------------------------
// <copyright file="ItemCategoryResolver.cs" company="KsiProgramming">
//     Copyright (c) KsiProgramming. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace GildedRoseKata.ProposedCode;

public class ItemCategoryResolver : IItemCategoryResolver
{
	public ItemCategory Resolve(string itemName)
	{
		return itemName switch
		{
			"Aged Brie" => new ItemCategory.AgedBrie(),
			"AgedBrie" => new ItemCategory.AgedBrie(),
			"Backstage passes to a TAFKAL80ETC concert" => new ItemCategory.Backstage(),
			"Backstage" => new ItemCategory.Backstage(),
			"Conjured Mana Cake" => new ItemCategory.Conjured(),
			"Conjured" => new ItemCategory.Conjured(),
			"Sulfuras, Hand of Ragnaros" => new ItemCategory.Sulfuras(),
			_ => new ItemCategory.Standard(),
		};
	}
}