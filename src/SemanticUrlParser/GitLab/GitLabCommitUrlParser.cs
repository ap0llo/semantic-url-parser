using System;
using System.Diagnostics.CodeAnalysis;

namespace Grynwald.SemanticUrlParser.GitLab
{
    /// <summary>
    /// Parses GitLab commit urls
    /// </summary>
    public sealed class GitLabCommitUrlParser : GitLabWebUrlParser<GitLabCommitInfo, string>
    {
        protected override string ExpectedLinkType => "commit";

        protected override GitLabCommitInfo CreateResult(GitLabProjectInfo project, string id) => new GitLabCommitInfo(project, id);

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
