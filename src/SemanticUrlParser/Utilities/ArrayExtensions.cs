using System;

namespace Grynwald.SemanticUrlParser
{
    internal static class ArrayExtensions
    {
        internal static void Deconstruct<T>(this T[] array, out T t1, out T t2)
        {
            if (array.Length != 2)
            {
                throw new ArgumentException($"Cannot deconstruct array of length {array.Length} into two items");
            }

            t1 = array[0];
            t2 = array[1];
        }

        internal static void Deconstruct<T>(this T[] array, out T t1, out T t2, out T t3, out T t4)
        {
            if (array.Length != 4)
            {
                throw new ArgumentException($"Cannot deconstruct array of length {array.Length} into four items");
            }

            t1 = array[0];
            t2 = array[1];
            t3 = array[2];
            t4 = array[3];
        }
    }
}
