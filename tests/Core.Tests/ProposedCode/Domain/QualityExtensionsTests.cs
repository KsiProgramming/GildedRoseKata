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
    }
}
