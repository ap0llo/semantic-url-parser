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
    public sealed class GitHubIssueUrlParser : GitHubWebUrlParser<GitHubIssueInfo, int>
    {
        protected override string ExpectedLinkType => "issues";

        protected override GitHubIssueInfo CreateResult(GitHubProjectInfo project, int id) => new GitHubIssueInfo(project, id);

        protected override bool TryParseId(string input, out int issueNumber, [NotNullWhen(false)] out string? errorMessage)
        {
            if (!Int32.TryParse(input, out issueNumber))
            {
                errorMessage = $"'{input}' is not a valid issue number";
                return false;
            }

            if (issueNumber <= 0)
            {
                errorMessage = $"Issue number must not be 0 or negative but was '{issueNumber}'";
                return false;
            }

            errorMessage = null;
            return true;
        }
    }
}
