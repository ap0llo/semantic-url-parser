using System;
using System.Diagnostics.CodeAnalysis;
using Grynwald.SemanticUrlParser.Utilities;

namespace Grynwald.SemanticUrlParser.GitHub
{
    public partial class GitHubUrlParser
    {
        /// <summary>
        /// Parses the specified git remote url.
        /// Supports both HTTP and SSH urls.
        /// </summary>
        /// <param name="url">The url to parse.</param>
        /// <returns>Returns a <see cref="GitHubProjectInfo"/> with information about the GitHub project the remote url belongs to.</returns>
        /// <exception cref="ArgumentException">Thrown if the specified url could not be parsed.</exception>
        public GitHubProjectInfo ParseRemoteUrl(string url)
        {
            if (TryParseRemoteUrl(url, out var projectInfo, out var errorMessage))
            {
                return projectInfo;
            }
            else
            {
                throw new ArgumentException(errorMessage, nameof(url));
            }
        }

        /// <summary>
        /// Attempts to parse the specified git remote url.
        /// Supports both HTTP and SSH urls.
        /// </summary>
        /// <param name="url">The url to parse.</param>
        /// <param name="projectInfo">When successful, contains a <see cref="GitHubProjectInfo"/> with information about the GitHub project the remote url belongs to.</param>
        /// <returns>Returns <c>true</c> if the specified url could be parsed, otherwise returns <c>false</c>.</returns>
        public bool TryParseRemoteUrl(string url, [NotNullWhen(true)] out GitHubProjectInfo? projectInfo) => TryParseRemoteUrl(url, out projectInfo, out var _);


        private static bool TryParseRemoteUrl(string url, [NotNullWhen(true)] out GitHubProjectInfo? projectInfo, [NotNullWhen(false)] out string? errorMessage)
        {
            projectInfo = null;
            errorMessage = null;

            if (String.IsNullOrWhiteSpace(url))
            {
                errorMessage = "Value must not be null or empty";
                return false;
            }

            if (!GitRemoteUrl.TryGetUri(url, out var uri))
            {
                errorMessage = $"Value '{url}' is not a valid uri";
                return false;
            }

            switch (uri.Scheme.ToLower())
            {
                case "http":
                case "https":
                case "ssh":
                    var path = Uri.UnescapeDataString(uri.AbsolutePath).Trim('/');

                    if (!path.EndsWith(".git", StringComparison.OrdinalIgnoreCase))
                    {
                        errorMessage = $"Cannot parse '{url}' as GitHub url: Expected url to end with '.git'";
                        return false;
                    }

                    path = path.RemoveSuffix(".git", StringComparison.OrdinalIgnoreCase);

                    var ownerAndRepo = path.Split('/');
                    if (ownerAndRepo.Length != 2)
                    {
                        errorMessage = $"Cannot parse '{url}' as GitHub url";
                        return false;
                    }

                    var (owner, repo) = ownerAndRepo;


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

                    projectInfo = new GitHubProjectInfo(uri.Host, owner, repo);
                    return true;

                default:
                    errorMessage = $"Cannot parse '{url}' as GitHub url: Unsupported scheme '{uri.Scheme}'";
                    return false;
            }
        }
    }
}
