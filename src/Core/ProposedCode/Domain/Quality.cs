//-----------------------------------------------------------------------
// <copyright file="Quality.cs" company="KsiProgramming">
//     Copyright (c) KsiProgramming. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace GildedRoseKata.ProposedCode;

using System;

public readonly record struct Quality
{
    public const int Minimum = 0;
    public const int Maximum = 50;

    public Quality(int value)
    {
        this.Value = Math.Clamp(value, Minimum, Maximum);
    }

    public int Value { get; }

    public Quality Increase(int amount = 1)
    {
        return new(this.Value + ValidateAmount(amount));
    }

    public Quality Decrease(int amount = 1)
    {
        return new(this.Value - ValidateAmount(amount));
    }

    public Quality Reset()
    {
        return new(0);
    }

    internal static Quality Unclamped(int value)
    {
        return new Quality(value, clamp: false);
    }

    private static int ValidateAmount(int amount)
    {
        return amount < 0
            ? throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be non-negative.")
            : amount;
    }

    private Quality(int value, bool clamp)
    {
        this.Value = clamp
            ? Math.Clamp(value, Minimum, Maximum)
            : value;
    }
}
