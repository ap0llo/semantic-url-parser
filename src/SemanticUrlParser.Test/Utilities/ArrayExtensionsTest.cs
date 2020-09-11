using System;
using Xunit;

namespace Grynwald.SemanticUrlParser.Test.Utilities
{
    public class ArrayExtensionsTest
    {
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(10)]
        public void Deconstruct_throws_ArgumentException_if_array_has_incompatible_size_01(int size)
        {
            var array = new object[size];
            Assert.Throws<ArgumentException>(() => array.Deconstruct(out var a, out var b));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(5)]
        [InlineData(10)]
        public void Deconstruct_throws_ArgumentException_if_array_has_incompatible_size_02(int size)
        {
            var array = new object[size];
            Assert.Throws<ArgumentException>(() => array.Deconstruct(out var a, out var b, out var c, out var d));
        }

        [Fact]
        public void Deconstruct_deconstructs_array_with_length_two_into_the_expected_items()
        {
            // ARRANGE
            var array = new[] { "item1", "item2" };

            // ACT
            var (a, b) = array;

            // ASSERT
            Assert.Equal("item1", a);
            Assert.Equal("item2", b);
        }

        [Fact]
        public void Deconstruct_deconstructs_the_array_of_length_4_into_the_expected_items()
        {
            // ARRANGE
            var array = new[] { "item1", "item2", "item3", "item4" };

            // ACT
            var (a, b, c, d) = array;

            // ASSERT
            Assert.Equal("item1", a);
            Assert.Equal("item2", b);
            Assert.Equal("item3", c);
            Assert.Equal("item4", d);
        }
    }
}
