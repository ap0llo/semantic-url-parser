using System;
using Xunit;

namespace Grynwald.SemanticUrlParser.Test.Utilities
{
    public class ArrayExtensionsTest
    {
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(3)]
        [InlineData(10)]
        public void Deconstrcut_throws_ArgumentException_if_array_has_incompatible_size(int size)
        {
            var array = new object[size];
            Assert.Throws<ArgumentException>(() => array.Deconstruct(out var a, out var b));
        }

        [Fact]
        public void Deconstrcut_deconstructs_the_array_into_the_expected_items()
        {
            // ARRANGE
            var array = new[] { "item1", "item2" };

            // ACT
            var (a, b) = array;

            // ASSERT
            Assert.Equal("item1", a);
            Assert.Equal("item2", b);
        }
    }
}
