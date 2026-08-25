//-----------------------------------------------------------------------
// <copyright file="SellIn.cs" company="KsiProgramming">
//     Copyright (c) KsiProgramming. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace GildedRoseKata.ProposedCode;

public readonly record struct SellIn
{
    public SellIn(int value)
    {
        this.Value = value;
    }

    public int Value { get; }

    public bool IsExpired => this.Value < 0;

    public bool IsBelow(int limit)
    {
        return this.Value < limit;
    }

    public SellIn NextDay()
    {
        return new(this.Value - 1);
    }
}
