//-----------------------------------------------------------------------
// <copyright file="IItemMaturingStrategy.cs" company="KsiProgramming">
//     Copyright (c) KsiProgramming. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace GildedRoseKata.ProposedCode
{
    public interface IItemMaturingStrategy
    {
        ItemMaturingResponse Update(ItemMaturingRequest request);
    }
}
