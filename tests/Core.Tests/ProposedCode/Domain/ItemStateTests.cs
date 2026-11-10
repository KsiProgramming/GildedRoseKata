//-----------------------------------------------------------------------
// <copyright file="ItemStateTests.cs" company="KsiProgramming">
//     Copyright (c) KsiProgramming. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace GildedRoseKata.ProposedCode.Tests;

using FluentAssertions;

public class ItemStateTests
{
	[Theory]
	[InlineData(12)]
	[InlineData(80)]
	public void Create_WhenKindIsSulfuras_UsesLegendaryQuality(int quality)
	{
		var state = ItemState.Create(new ItemKind.Sulfuras(), quality, 0);

		state.Quality.Value.Should().Be(ItemState.LegendaryQuality);
		state.Quality.Value.Should().Be(80);
	}

	[Theory]
	[InlineData(99, 50)]
	[InlineData(-5, 0)]
	[InlineData(20, 20)]
	public void Create_WhenKindIsNotLegendary_ClampsQuality(int quality, int expected)
	{
		var state = ItemState.Create(new ItemKind.Standard(), quality, 5);

		state.Quality.Value.Should().Be(expected);
	}

	[Theory]
	[InlineData(10)]
	[InlineData(0)]
	[InlineData(-3)]
	public void Create_PassesSellInThrough(int sellIn)
	{
		var state = ItemState.Create(new ItemKind.Standard(), 10, sellIn);

		state.SellIn.Value.Should().Be(sellIn);
	}
}
