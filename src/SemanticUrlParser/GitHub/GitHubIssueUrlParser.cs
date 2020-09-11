using System;
using System.Diagnostics.CodeAnalysis;

namespace Grynwald.SemanticUrlParser.GitHub
{
    public sealed class GitHubIssueUrlParser : GitHubUrlParser
    {
        public GitHubIssueInfo ParseIssueUrl(string url)
        {
            if (TryParseIssueUrl(url, out var projectInfo, out var errorMessage))
            {
                return projectInfo;
            }
            else
            {
                throw new ArgumentException(errorMessage, nameof(url));
            }
        }

        public bool TryParseIssueUrl(string url, [NotNullWhen(true)] out GitHubIssueInfo? issueInfo) =>
            TryParseIssueUrl(url, out issueInfo, out var _);

        private bool TryParseIssueUrl(string url, [NotNullWhen(true)] out GitHubIssueInfo? issueInfo, [NotNullWhen(false)] out string? errorMessage)
        {
            issueInfo = null;
            errorMessage = null;

            if (String.IsNullOrWhiteSpace(url))
            {
                errorMessage = "Value must not be null or empty";
                return false;
            }

            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            {
                errorMessage = $"Value '{url}' is not a valid uri";
                return false;
            }

            if (!CheckScheme(uri, "http", "https"))
            {
                errorMessage = $"Cannot parse '{url}' as GitHub url: Unsupported scheme '{uri.Scheme}'";
                return false;
            }


            var path = Uri.UnescapeDataString(uri.AbsolutePath).Trim('/');

            var pathParts = path.Split('/');
            if (pathParts.Length != 4)
            {
                errorMessage = $"Cannot parse '{url}' as GitHub issue url";
                return false;
            }

            var (owner, repo, linkType, numberString) = pathParts;


            if (String.IsNullOrWhiteSpace(owner))
            {
                errorMessage = $"Cannot parse '{url}' as GitHub url: Repository owner cannot be empty or whitespace";
                return false;
            }

            if (String.IsNullOrWhiteSpace(repo))
            {
                errorMessage = $"Cannot parse '{url}' as GitHub url: Repository name cannot be empty or whitespace";
                return false;
            }

            if (!StringComparer.OrdinalIgnoreCase.Equals("issues", linkType))
            {
                errorMessage = $"Cannot parse '{url}' as GitHub url: Expected link type to be 'issue' but found '{linkType}'";
                return false;
            }

            if (!int.TryParse(numberString, out var number))
            {
                errorMessage = $"Cannot parse '{url}' as GitHub url: '{numberString}' is not a valid issue number";
                return false;
            }

            if (number <= 0)
            {
                errorMessage = $"Cannot parse '{url}' as GitHub url: Issue number must bot be 0 or negative but was '{number}'";
                return false;
            }

            issueInfo = new GitHubIssueInfo(new GitHubProjectInfo(uri.Host, owner, repo), number);
            return true;
        }


    }
}
