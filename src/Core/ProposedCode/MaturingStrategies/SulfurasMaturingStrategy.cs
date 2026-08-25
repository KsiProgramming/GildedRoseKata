//-----------------------------------------------------------------------
// <copyright file="SulfurasMaturingStrategy.cs" company="KsiProgramming">
//     Copyright (c) KsiProgramming. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace GildedRoseKata.ProposedCode.MaturingStrategies;

public class SulfurasMaturingStrategy : IMaturingPolicy
{
    public ItemCategory SupportedCategory => new ItemCategory.Sulfuras();

    public ItemState Mature(ItemState state)
    {
        return state;
    }
}