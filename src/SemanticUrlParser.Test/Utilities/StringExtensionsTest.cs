using System;
using Xunit;

namespace Grynwald.SemanticUrlParser.Test.Utilities
{
    /// <summary>
    /// Tests for <see cref="StringExtensions"/>
    /// </summary>
    public class StringExtensionsTest
    {
        [Theory]
        [InlineData("some-string", "", "some-string")]
        [InlineData("some-string", "-string", "some")]
        [InlineData("some-string", "suffix", "some-string")]
        public void RemoteSuffix_returns_expected_string(string input, string suffix, string expectedResult)
        {
            // ARRANGE

            // ACT 
            var actualResult = input.RemoveSuffix(suffix);

            // ASSERT
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData("some-string", "-STRING", StringComparison.OrdinalIgnoreCase, "some")]
        [InlineData("some-string", "-STRING", StringComparison.Ordinal, "some-string")]
        public void RemoteSuffix_returns_expected_string_and_uses_the_specified_comparison_type(string input, string suffix, StringComparison comparisonType, string expectedResult)
        {
            // ARRANGE

            // ACT 
            var actualResult = input.RemoveSuffix(suffix, comparisonType);

            // ASSERT
            Assert.Equal(expectedResult, actualResult);
        }
    }
}
