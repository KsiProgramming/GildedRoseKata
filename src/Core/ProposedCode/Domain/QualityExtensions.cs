﻿//-----------------------------------------------------------------------
// <copyright file="QualityExtensions.cs" company="KsiProgramming">
//     Copyright (c) KsiProgramming. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace GildedRoseKata.ProposedCode;

public static class QualityExtensions
{
    public static bool IsBelowLimit(this Quality quality, int limit)
    {
        return quality.Value < limit;
    }

    public static Quality Decrease(this Quality quality)
    {
        return DecreaseBy(quality, 1);
    }

    public static Quality DecreaseBy(this Quality quality, int value)
    {
        return quality with { Value = quality.Value - value };
    }

    public static Quality Increase(this Quality quality)
    {
        return IncreaseBy(quality, 1);
    }

    public static Quality IncreaseBy(this Quality quality, int value)
    {
        return quality with { Value = quality.Value + value };
    }

    public static bool IsAboveMinimum(this Quality quality, int minimum)
    {
        return quality.Value > minimum;
    }
}
