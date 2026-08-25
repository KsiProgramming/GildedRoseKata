//-----------------------------------------------------------------------
// <copyright file="IMaturingPolicyCatalog.cs" company="KsiProgramming">
//     Copyright (c) KsiProgramming. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace GildedRoseKata.ProposedCode;

public interface IMaturingPolicyCatalog
{
	IMaturingPolicy GetFor(ItemCategory category);
}