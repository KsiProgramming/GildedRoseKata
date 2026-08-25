//-----------------------------------------------------------------------
// <copyright file="ItemKind.cs" company="KsiProgramming">
//     Copyright (c) KsiProgramming. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace GildedRoseKata.ProposedCode;

public abstract record ItemKind
{
    public sealed record Standard : ItemKind;

    public sealed record AgedBrie : ItemKind;

    public sealed record Backstage : ItemKind;

    public sealed record Conjured : ItemKind;

    public sealed record Sulfuras : ItemKind;
}