//-----------------------------------------------------------------------
// <copyright file="Item.cs" company="KsiProgramming">
//     Copyright (c) KsiProgramming. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace GildedRoseKata.OriginalCode
{
    public class Item
    {
#pragma warning disable 8618
        public string Name { get; set; }
#pragma warning restore 8618

        public int SellIn { get; set; }

        public int Quality { get; set; }
    }
}
