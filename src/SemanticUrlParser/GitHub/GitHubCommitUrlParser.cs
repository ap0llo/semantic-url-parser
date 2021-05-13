using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Grynwald.SemanticUrlParser.GitHub
{
    /// <summary>
    /// Parses GitHub commit URLs and returns a <see cref="GitHubCommitInfo"/>
    /// </summary>
    /// <remarks>
    /// Gets the host, repository owner, repository name and the commit id from a GitHub commit web link
    /// </remarks>
    /// <example>
    /// <code language="csharp">
    /// var parser = new GitHubCommitUrlParser();
    /// var commitInfo = parser.ParseUrl("https://github.com/user/my-repo/commit/abc123");
    ///
    /// Console.WriteLine(commitInfo.Project.Owner);       // Prints 'user'
    /// Console.WriteLine(commitInfo.Project.Repository);  // Prints 'my-repo'
    /// Console.WriteLine(commitInfo.CommitId);            // Prints 'abc123'
    /// </code>
    /// </example>
    public sealed class GitHubCommitUrlParser : GitHubUrlParser<GitHubCommitInfo>
    {
        protected override IEnumerable<string> SupportedSchemes { get; } = new[] { "http", "https" };

        protected override bool TryParsePath(Uri uri, string path, [NotNullWhen(true)] out GitHubCommitInfo? result, [NotNullWhen(false)] out string? errorMessage)
        {
            // expected path: '<owner>/<repo>/commit/<commitid>'

            result = default;

            var pathSegments = path.Split('/');
            if (pathSegments.Length != 4)
            {
                errorMessage = $"'{uri}' is not a GitHub commit url";
                return false;
            }

            var (owner, repo, linkType, commitId) = pathSegments;

            if (!TryCreateProjectInfo(uri.Host, owner, repo, out var projectInfo, out errorMessage))
            {
                return false;
            }

            if (!StringComparer.OrdinalIgnoreCase.Equals("commit", linkType))
            {
                errorMessage = $"Expected link type to be 'commit' but found '{linkType}'";
                return false;
            }

            if (String.IsNullOrWhiteSpace(commitId))
            {
                errorMessage = "Commit id must not be null or whitespace";
                return false;
            }

            errorMessage = null;
            result = new GitHubCommitInfo(projectInfo, commitId);
            return true;
        }
    }
}
