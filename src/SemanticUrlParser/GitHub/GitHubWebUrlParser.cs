using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Grynwald.SemanticUrlParser.GitHub
{
    /// <summary>
    /// Base class for all parsers of GitHub web url
    /// </summary>
    /// <typeparam name="TResult">The type of the result returned by the parser</typeparam>
    /// <typeparam name="TId">The type of the parsed item id, e.g. <c>int</c> for pull request ids.</typeparam>
    public abstract class GitHubWebUrlParser<TResult, TId> : GitHubUrlParser<TResult> where TResult : class
    {
        protected override IEnumerable<string> SupportedSchemes { get; } = new[] { "http", "https" };

        /// <summary>
        /// Gets the expected link type for the current parser.
        /// </summary>
        /// <remarks>
        /// GitHub web urls all folow the format <c><![CDATA[<scheme>://<host>/<owner>/<repo>/<linktype>/<id>]]></c>,
        /// e.g. <c><![CDATA[https://github.com/owner/repo/issues/1]]></c>.
        /// The <see cref="ExpectedLinkType"/> must return the expected link type for the current parser.
        /// </remarks>
        protected abstract string ExpectedLinkType { get; }


        protected override bool TryParsePath(Uri uri, string path, [NotNullWhen(true)] out TResult? result, [NotNullWhen(false)] out string? errorMessage)
        {
            // expected path: '<owner>/<repo>/<linktype>/<id>'

            result = default;

            var pathSegments = path.Split('/');
            if (pathSegments.Length != 4)
            {
                errorMessage = $"'{uri}' is not a GitHub web url";
                return false;
            }

            var (owner, repo, linkType, idString) = pathSegments;

            if (!TryCreateProjectInfo(uri.Host, owner, repo, out var projectInfo, out errorMessage))
            {
                return false;
            }

            if (!StringComparer.OrdinalIgnoreCase.Equals(ExpectedLinkType, linkType))
            {
                errorMessage = $"Expected link type to be '{ExpectedLinkType}' but found '{linkType}'";
                return false;
            }

            if (!TryParseId(idString, out var parsedId, out errorMessage))
            {
                return false;
            }


            errorMessage = null;
            result = CreateResult(projectInfo, parsedId);
            return true;
        }

        protected abstract bool TryParseId(string input, out TId parsed, [NotNullWhen(false)] out string? errorMessage);

        protected abstract TResult CreateResult(GitHubProjectInfo project, TId id);

    }
}
