using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Grynwald.SemanticUrlParser.GitHub
{
    /// <summary>
    /// Parses GitHub issue URLs and returns a <see cref="GitHubIssueInfo"/>
    /// </summary>
    /// <remarks>
    /// Gets the host, repository owner, repository name and issue number from a GitHub issue web link
    /// </remarks>
    /// <example>
    /// <code language="csharp">
    /// var parser = new GitHubIssueUrlParser();
    /// var issueInfo = parser.ParseUrl("https://github.com/user/my-repo/issues/42");
    ///
    /// Console.WriteLine(issueInfo.Project.Owner);       // Prints 'user'
    /// Console.WriteLine(issueInfo.Project.Repository);  // Prints 'my-repo'
    /// Console.WriteLine(issueInfo.Number);              // Prints '42'
    /// </code>
    /// </example>
    public sealed class GitHubIssueUrlParser : GitHubUrlParser<GitHubIssueInfo>
    {
        protected override IEnumerable<string> SupportedSchemes { get; } = new[] { "http", "https" };


        protected override bool TryParsePath(Uri uri, string path, [NotNullWhen(true)] out GitHubIssueInfo? result, [NotNullWhen(false)] out string? errorMessage)
        {
            // expected path: '<owner>/<repo>/issues/<number>'

            result = default;

            var pathSegments = path.Split('/');
            if (pathSegments.Length != 4)
            {
                errorMessage = $"'{uri}' is not a GitHub issue url";
                return false;
            }

            var (owner, repo, linkType, numberString) = pathSegments;

            if (!TryCreateProjectInfo(uri.Host, owner, repo, out var projectInfo, out errorMessage))
            {
                return false;
            }

            if (!StringComparer.OrdinalIgnoreCase.Equals("issues", linkType))
            {
                errorMessage = $"Expected link type to be 'issue' but found '{linkType}'";
                return false;
            }

            if (!TryParseIssueNumber(numberString, out var issueNumber, out errorMessage))
            {
                return false;
            }


            errorMessage = null;
            result = new GitHubIssueInfo(projectInfo, issueNumber);
            return true;
        }


        private bool TryParseIssueNumber(string input, out int issueNumber, [NotNullWhen(false)] out string? errorMessage)
        {
            if (!Int32.TryParse(input, out issueNumber))
            {
                errorMessage = $"'{input}' is not a valid issue number";
                return false;
            }

            if (issueNumber <= 0)
            {
                errorMessage = $"Issue number must bot be 0 or negative but was '{issueNumber}'";
                return false;
            }

            errorMessage = null;
            return true;
        }
    }
}
