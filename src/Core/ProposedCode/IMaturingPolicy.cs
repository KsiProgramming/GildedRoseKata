//-----------------------------------------------------------------------
// <copyright file="IMaturingPolicy.cs" company="KsiProgramming">
//     Copyright (c) KsiProgramming. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace GildedRoseKata.ProposedCode;

public interface IMaturingPolicy
{
    ItemKind SupportedKind { get; }

    ItemState Mature(ItemState state);
}