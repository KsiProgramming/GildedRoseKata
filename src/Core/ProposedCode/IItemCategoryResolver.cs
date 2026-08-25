//-----------------------------------------------------------------------
// <copyright file="IItemCategoryResolver.cs" company="KsiProgramming">
//     Copyright (c) KsiProgramming. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace GildedRoseKata.ProposedCode;

public interface IItemCategoryResolver
{
	ItemCategory Resolve(string itemName);
}