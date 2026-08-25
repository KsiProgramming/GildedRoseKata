//-----------------------------------------------------------------------
// <copyright file="SulfurasMaturingStrategy.cs" company="KsiProgramming">
//     Copyright (c) KsiProgramming. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace GildedRoseKata.ProposedCode.MaturingStrategies;

public class SulfurasMaturingStrategy : IMaturingPolicy
{
    public ItemKind SupportedKind => new ItemKind.Sulfuras();

    public ItemState Mature(ItemState state)
    {
        return state;
    }
}