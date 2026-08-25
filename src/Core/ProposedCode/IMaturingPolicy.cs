//-----------------------------------------------------------------------
// <copyright file="IMaturingPolicy.cs" company="KsiProgramming">
//     Copyright (c) KsiProgramming. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace GildedRoseKata.ProposedCode;

public interface IMaturingPolicy
{
    ItemState Mature(ItemKind kind, ItemState state);
}