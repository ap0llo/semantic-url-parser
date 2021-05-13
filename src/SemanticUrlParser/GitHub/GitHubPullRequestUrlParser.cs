using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Grynwald.SemanticUrlParser.GitHub
{
    /// <summary>
    /// Parses GitHub pull request URLs and returns a <see cref="GitHubPullRequestInfo"/>
    /// </summary>
    /// <remarks>
    /// Gets the host, repository owner, repository name and issue number from a GitHub pull request web link
    /// </remarks>
    /// <example>
    /// <code language="csharp">
    /// var parser = new GitHubPullRequestUrlParser();
    /// var issueInfo = parser.ParseUrl("https://github.com/user/my-repo/pull/42");
    ///
    /// Console.WriteLine(issueInfo.Project.Owner);       // Prints 'user'
    /// Console.WriteLine(issueInfo.Project.Repository);  // Prints 'my-repo'
    /// Console.WriteLine(issueInfo.Number);              // Prints '42'
    /// </code>
    /// </example>
    public sealed class GitHubPullRequestUrlParser : GitHubUrlParser<GitHubPullRequestInfo>
    {
        protected override IEnumerable<string> SupportedSchemes { get; } = new[] { "http", "https" };


        protected override bool TryParsePath(Uri uri, string path, [NotNullWhen(true)] out GitHubPullRequestInfo? result, [NotNullWhen(false)] out string? errorMessage)
        {
            // expected path: '<owner>/<repo>/pull/<number>'

            result = default;

            var pathSegments = path.Split('/');
            if (pathSegments.Length != 4)
            {
                errorMessage = $"'{uri}' is not a GitHub pull request url";
                return false;
            }

            var (owner, repo, linkType, numberString) = pathSegments;

            if (!TryCreateProjectInfo(uri.Host, owner, repo, out var projectInfo, out errorMessage))
            {
                return false;
            }

            if (!StringComparer.OrdinalIgnoreCase.Equals("pull", linkType))
            {
                errorMessage = $"Expected link type to be 'pull' but found '{linkType}'";
                return false;
            }

            if (!TryParsePullRequestNumber(numberString, out var prNumber, out errorMessage))
            {
                return false;
            }

            errorMessage = null;
            result = new GitHubPullRequestInfo(projectInfo, prNumber);
            return true;
        }


        private bool TryParsePullRequestNumber(string input, out int prNumber, [NotNullWhen(false)] out string? errorMessage)
        {
            if (!Int32.TryParse(input, out prNumber))
            {
                errorMessage = $"'{input}' is not a valid pull request number";
                return false;
            }

            if (prNumber <= 0)
            {
                errorMessage = $"Pull Request number must bot be 0 or negative but was '{prNumber}'";
                return false;
            }

            errorMessage = null;
            return true;
        }
    }
}
