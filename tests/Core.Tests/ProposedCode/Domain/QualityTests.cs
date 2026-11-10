//-----------------------------------------------------------------------
// <copyright file="QualityTests.cs" company="KsiProgramming">
//     Copyright (c) KsiProgramming. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace GildedRoseKata.ProposedCode.Tests;

using System;
using FluentAssertions;

public class QualityTests
{
	[Theory]
	[InlineData(-10, 0)]
	[InlineData(0, 0)]
	[InlineData(25, 25)]
	[InlineData(50, 50)]
	[InlineData(99, 50)]
	public void Constructor_ClampsIntoDomainRange(int value, int expected)
	{
		var quality = new Quality(value);

		quality.Value.Should().Be(expected);
	}

	[Theory]
	[InlineData(5, 1, 6)]
	[InlineData(49, 3, 50)]
	[InlineData(50, 1, 50)]
	public void Increase_RespectsMaximum(int initialValue, int amount, int expected)
	{
		var quality = new Quality(initialValue);

		var result = quality.Increase(amount);

		result.Value.Should().Be(expected);
	}

	[Theory]
	[InlineData(5, 1, 4)]
	[InlineData(1, 3, 0)]
	[InlineData(0, 1, 0)]
	public void Decrease_RespectsMinimum(int initialValue, int amount, int expected)
	{
		var quality = new Quality(initialValue);

		var result = quality.Decrease(amount);

		result.Value.Should().Be(expected);
	}

	[Fact]
	public void Increase_WhenAmountIsNegative_Throws()
	{
		var quality = new Quality(10);

		var act = () => quality.Increase(-1);

		act.Should().Throw<ArgumentOutOfRangeException>();
	}

	[Fact]
	public void Decrease_WhenAmountIsNegative_Throws()
	{
		var quality = new Quality(10);

		var act = () => quality.Decrease(-1);

		act.Should().Throw<ArgumentOutOfRangeException>();
	}

	[Fact]
	public void Reset_ReturnsZero()
	{
		var result = new Quality(10).Reset();

		result.Value.Should().Be(0);
	}
}
