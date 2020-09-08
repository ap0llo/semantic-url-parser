using System;

namespace Grynwald.SemanticUrlParser
{
    internal static class StringExtensions
    {
        internal static string RemoveSuffix(this string value, string suffix)
        {
            return value.EndsWith(suffix)
#if NETSTANDARD2_0
                ? value.Substring(0, value.Length - suffix.Length)
#else
                ? value[..^suffix.Length]
#endif
                : value;
        }

    }
}
