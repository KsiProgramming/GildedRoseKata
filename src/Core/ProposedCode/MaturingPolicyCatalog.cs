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
        this.policiesByType = policies.ToDictionary(policy => policy.SupportedKind.GetType());
    }

    public IMaturingPolicy GetFor(ItemKind kind)
    {
        return this.policiesByType.TryGetValue(kind.GetType(), out var policy)
            ? policy
            : this.policiesByType[typeof(ItemKind.Standard)];
    }
}