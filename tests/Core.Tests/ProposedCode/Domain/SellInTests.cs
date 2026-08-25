//-----------------------------------------------------------------------
// <copyright file="SellInTests.cs" company="KsiProgramming">
//     Copyright (c) KsiProgramming. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace GildedRoseKata.ProposedCode.Tests;

using FluentAssertions;

public class SellInTests
{
	[Theory]
	[InlineData(1, false)]
	[InlineData(0, false)]
	[InlineData(-1, true)]
	public void IsExpired_UsesStrictlyNegativeBoundary(int value, bool expected)
	{
		var sellIn = new SellIn(value);

		sellIn.IsExpired.Should().Be(expected);
	}

	[Theory]
	[InlineData(20, 50, true)]
	[InlineData(50, 50, false)]
	[InlineData(60, 50, false)]
	public void IsBelow_ReturnsWhetherValueIsBelowLimit(int value, int limit, bool expected)
	{
		var sellIn = new SellIn(value);

		var result = sellIn.IsBelow(limit);

		result.Should().Be(expected);
	}

	[Fact]
	public void NextDay_DecrementsByOne()
	{
		var sellIn = new SellIn(5);

		var result = sellIn.NextDay();

		result.Value.Should().Be(4);
	}
}