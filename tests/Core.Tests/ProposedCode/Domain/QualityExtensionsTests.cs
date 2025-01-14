//-----------------------------------------------------------------------
// <copyright file="QualityExtensionsTests.cs" company="KsiProgramming">
//     Copyright (c) KsiProgramming. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace GildedRoseKata.ProposedCode.Tests
{
    using FluentAssertions;

    public class QualityExtensionsTests
    {
        [Theory]
        [InlineData(20, 50, true)]
        [InlineData(50, 50, false)]
        [InlineData(60, 50, false)]
        public void IsBelowLimit(int qualityValue, int limit, bool expectedResult)
        {
            var quality = new Quality(qualityValue);

            var result = quality.IsBelowLimit(limit);

            result.Should().Be(expectedResult);
        }

        [Theory]
        [InlineData(5, 0, true)]
        [InlineData(0, 0, false)]
        [InlineData(-5, 0, false)]
        public void IsAboveMinimum(int qualityValue, int minimum, bool expectedResult)
        {
            var quality = new Quality(qualityValue);

            var result = quality.IsAboveMinimum(minimum);

            result.Should().Be(expectedResult);
        }

        [Theory]
        [InlineData(5, 4)]
        public void Decrease(int qualityValue, int expectedResult)
        {
            var quality = new Quality(qualityValue);

            var result = quality.Decrease();

            result.Value.Should().Be(expectedResult);
            result.Should().NotBeSameAs(quality);
        }

        [Theory]
        [InlineData(5, 6)]
        public void Increase(int qualityValue, int expectedResult)
        {
            var quality = new Quality(qualityValue);

            var result = quality.Increase();

            result.Value.Should().Be(expectedResult);
            result.Should().NotBeSameAs(quality);
        }

        [Theory]
        [InlineData(5, 1, 4)]
        [InlineData(5, 0, 5)]
        [InlineData(5, -1, 6)]
        public void DecreaseBy(int qualityValue, int step, int expectedResult)
        {
            var quality = new Quality(qualityValue);

            var result = quality.DecreaseBy(step);

            result.Value.Should().Be(expectedResult);
            result.Should().NotBeSameAs(quality);
        }

        [Theory]
        [InlineData(5, 1, 6)]
        [InlineData(5, 0, 5)]
        [InlineData(5, -1, 4)]
        public void IncreaseBy(int qualityValue, int step, int expectedResult)
        {
            var quality = new Quality(qualityValue);

            var result = quality.IncreaseBy(step);

            result.Value.Should().Be(expectedResult);
            result.Should().NotBeSameAs(quality);
        }
    }
}
