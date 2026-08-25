//-----------------------------------------------------------------------
// <copyright file="IItemKindResolver.cs" company="KsiProgramming">
//     Copyright (c) KsiProgramming. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace GildedRoseKata.ProposedCode;

public interface IItemKindResolver
{
    ItemKind Resolve(string itemName);
}