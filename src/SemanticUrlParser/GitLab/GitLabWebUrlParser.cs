using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Grynwald.SemanticUrlParser.GitLab
{
    /// <summary>
    /// Base class for all parsers of GitLab web urls
    /// </summary>
    /// <typeparam name="TResult">The type of the result returned by the parser</typeparam>
    /// <typeparam name="TId">The type of the parsed item id, e.g. <c>int</c> for pull request ids.</typeparam>
    public abstract class GitLabWebUrlParser<TResult, TId> : GitLabUrlParser<TResult> where TResult : class
    {
        protected override IEnumerable<string> SupportedSchemes { get; } = new[] { "http", "https" };

        /// <summary>
        /// Gets the expected link type for the current parser.
        /// </summary>
        /// <remarks>
        /// GitLab web urls all folow the format <c><![CDATA[<scheme>://<host>/<namespace>/<project>/-/<linktype>/<id>]]></c>,
        /// e.g. <c><![CDATA[https://gitlab.com/group/subgroup/myproject/-/issues/1]]></c>.
        /// The <see cref="ExpectedLinkType"/> must return the expected link type for the current parser.
        /// </remarks>
        protected abstract string ExpectedLinkType { get; }


        protected override bool TryParsePath(Uri uri, string path, [NotNullWhen(true)] out TResult? result, [NotNullWhen(false)] out string? errorMessage)
        {
            // expected path: '<namespace>/<project>/-/<linktype>/<id>'

            result = default;

            var pathSegments = path.Split('/');
            if (pathSegments.Length < 5)
            {
                errorMessage = $"'{uri}' is not a GitLab web url";
                return false;
            }

            var dashIndex = Array.IndexOf(pathSegments, "-");

            // there must be at least a user or group name and a project name before the '-'
            if (dashIndex < 2)
            {
                errorMessage = $"'{uri}' is not a GitLab web url";
                return false;
            }

            // there must be at least the link type and the id after the '-'
            if (dashIndex != pathSegments.Length - 3)
            {
                errorMessage = $"'{uri}' is not a GitLab web url";
                return false;
            }

            var @namespace = String.Join("/", pathSegments.Take(dashIndex - 1));
            var projectName = pathSegments[dashIndex - 1];

            if (!TryCreateProjectInfo(uri.Host, @namespace, projectName, out var projectInfo, out errorMessage))
            {
                return false;
            }

            var linkType = pathSegments[dashIndex + 1];
            var idString = pathSegments[dashIndex + 2];

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

        protected abstract TResult CreateResult(GitLabProjectInfo project, TId id);

    }
}
