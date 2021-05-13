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
    public sealed class GitHubCommitUrlParser : GitHubWebUrlParser<GitHubCommitInfo, string>
    {
        protected override string ExpectedLinkType => "commit";

        protected override GitHubCommitInfo CreateResult(GitHubProjectInfo project, string commitId) => new GitHubCommitInfo(project, commitId);

        protected override bool TryParseId(string input, out string parsed, [NotNullWhen(false)] out string? errorMessage)
        {
            if (String.IsNullOrWhiteSpace(input))
            {
                errorMessage = "Commit id must not be null or whitespace";
                parsed = null!;
                return false;
            }

            errorMessage = null;
            parsed = input;
            return true;
        }
    }
}
