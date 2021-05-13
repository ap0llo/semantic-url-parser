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
    public sealed class GitHubPullRequestUrlParser : GitHubWebUrlParser<GitHubPullRequestInfo, int>
    {
        protected override string ExpectedLinkType => "pull";

        protected override GitHubPullRequestInfo CreateResult(GitHubProjectInfo project, int id) => new GitHubPullRequestInfo(project, id);

        protected override bool TryParseId(string input, out int prNumber, [NotNullWhen(false)] out string? errorMessage)
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
