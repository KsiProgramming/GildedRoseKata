//-----------------------------------------------------------------------
// <copyright file="ItemCategory.cs" company="KsiProgramming">
//     Copyright (c) KsiProgramming. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace GildedRoseKata.ProposedCode;

public abstract record ItemCategory
{
    public sealed record Standard : ItemCategory;

    public sealed record AgedBrie : ItemCategory;

    public sealed record Backstage : ItemCategory;

    public sealed record Conjured : ItemCategory;

    public sealed record Sulfuras : ItemCategory;
}