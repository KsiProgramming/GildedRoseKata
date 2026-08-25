//-----------------------------------------------------------------------
// <copyright file="MaturingPolicyCatalog.cs" company="KsiProgramming">
//     Copyright (c) KsiProgramming. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace GildedRoseKata.ProposedCode;

using System;

public class MaturingPolicyCatalog : IMaturingPolicyCatalog
{
	private readonly Dictionary<Type, IMaturingPolicy> policiesByType;

	public MaturingPolicyCatalog(IEnumerable<IMaturingPolicy> policies)
	{
		this.policiesByType = policies.ToDictionary(policy => policy.SupportedCategory.GetType());
	}

	public IMaturingPolicy GetFor(ItemCategory category)
	{
		return this.policiesByType.TryGetValue(category.GetType(), out var policy)
			? policy
			: this.policiesByType[typeof(ItemCategory.Standard)];
	}
}