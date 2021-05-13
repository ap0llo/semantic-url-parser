using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Grynwald.SemanticUrlParser
{
    /// <summary>
    /// Base class for all url parsers
    /// </summary>
    public abstract class UrlParser<T> where T : class
    {
        protected abstract IEnumerable<string> SupportedSchemes { get; }


        /// <summary>
        /// Parses the specified url.
        /// </summary>
        /// <param name="url">The url to parse.</param>
        /// <returns>Returns a instance of <see cref="T"/> with information parsed from the url.</returns>
        /// <exception cref="ArgumentException">Thrown if the specified url could not be parsed.</exception>
        public T ParseUrl(string url)
        {
            if (TryParseUrl(url, out var result, out var errorMessage))
            {
                return result;
            }
            else
            {
                throw new ArgumentException(errorMessage, nameof(url));
            }
        }

        /// <summary>
        /// Attempts to parse the specified url.
        /// </summary>
        /// <param name="url">The url to parse.</param>
        /// <param name="projectInfo">When successful, contains a instance of <see cref="T"/> with information parsed from the url.</param>
        /// <returns>Returns <c>true</c> if the specified url could be parsed, otherwise returns <c>false</c>.</returns>
        public bool TryParseUrl(string url, [NotNullWhen(true)] out T? result) =>
            TryParseUrl(url, out result, out var _);


        protected virtual bool TryParseUrl(string url, [NotNullWhen(true)] out T? result, [NotNullWhen(false)] out string? errorMessage)
        {
            result = default;

            if (String.IsNullOrWhiteSpace(url))
            {
                errorMessage = "Url must not be null or whitsapce";
                return false;
            }

            if (!TryCreateUri(url, out var uri))
            {
                errorMessage = $"'{url}' is not a valid uri";
                return false;
            }

            if (!CheckScheme(uri))
            {
                errorMessage = $"Cannot parse '{url}': Unsupported scheme '{uri.Scheme}'";
                return false;
            }

            var path = uri.GetComponents(UriComponents.Path, UriFormat.Unescaped).Trim('/');

            return TryParsePath(uri, path, out result, out errorMessage);
        }

        protected virtual bool TryCreateUri(string url, [NotNullWhen(true)] out Uri? result) => Uri.TryCreate(url, UriKind.Absolute, out result);

        protected virtual bool CheckScheme(Uri uri)
        {
            foreach (var scheme in SupportedSchemes)
            {
                if (scheme.Equals(uri.Scheme, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        protected abstract bool TryParsePath(Uri uri, string path, [NotNullWhen(true)] out T? result, [NotNullWhen(false)] out string? errorMessage);
    }
}
