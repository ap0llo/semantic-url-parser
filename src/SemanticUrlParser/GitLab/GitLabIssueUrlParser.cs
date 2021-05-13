using System;
using System.Diagnostics.CodeAnalysis;

namespace Grynwald.SemanticUrlParser.GitLab
{
    /// <summary>
    /// Parses GitLab issue urls
    /// </summary>
    public sealed class GitLabIssueUrlParser : GitLabWebUrlParser<GitLabIssueInfo, int>
    {
        protected override string ExpectedLinkType => "issues";

        protected override GitLabIssueInfo CreateResult(GitLabProjectInfo project, int id) => new GitLabIssueInfo(project, id);

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
