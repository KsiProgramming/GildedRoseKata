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
        public void Decrease(int qualityValue, int expectedResult)
        {
            var quality = new Quality(qualityValue);

            var result = quality.Decrease();

            result.value.Should().Be(expectedResult);
            result.Should().NotBeSameAs(quality);
        }
    }
}
