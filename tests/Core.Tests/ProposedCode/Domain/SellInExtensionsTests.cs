//-----------------------------------------------------------------------
// <copyright file="SellInExtensionsTests.cs" company="KsiProgramming">
//     Copyright (c) KsiProgramming. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace GildedRoseKata.ProposedCode.Tests
{
    using FluentAssertions;

    public class SellInExtensionsTests
    {
        [Theory]
        [InlineData(5, 4)]
        public void Decrease(int sellInValue, int expectedResult)
        {
            var sellIn = new SellIn(sellInValue);

            var result = sellIn.Decrease();

            result.value.Should().Be(expectedResult);
            result.Should().NotBeSameAs(sellIn);
        }

        [Theory]
        [InlineData(1, false)]
        [InlineData(0, true)]
        [InlineData(-1, true)]
        public void HasExpired(int sellInValue, bool expectedResult)
        {
            var sellIn = new SellIn(sellInValue);

            var result = sellIn.HasExpired();

            result.Should().Be(expectedResult);
        }

        [Theory]
        [InlineData(20, 50, true)]
        [InlineData(50, 50, false)]
        [InlineData(60, 50, false)]
        public void IsBelowLimit(int sellInValue, int limit, bool expectedResult)
        {
            var sellIn = new SellIn(sellInValue);

            var result = sellIn.IsBelowLimit(limit);

            result.Should().Be(expectedResult);
        }
    }
}
