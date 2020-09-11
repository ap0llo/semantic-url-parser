using System;
using System.Diagnostics.CodeAnalysis;
using Grynwald.SemanticUrlParser.Utilities;

namespace Grynwald.SemanticUrlParser.GitLab
{
    public sealed partial class GitLabUrlParser
    {
        /// <summary>
        /// Parses the specified git remote url.
        /// Supports both HTTP and SSH urls.
        /// </summary>
        /// <param name="url">The url to parse.</param>
        /// <returns>Returns a <see cref="GitLabProjectInfo"/> with information about the GitLab project the remote url belongs to.</returns>
        /// <exception cref="ArgumentException">Thrown if the specified url could not be parsed.</exception>
        public GitLabProjectInfo ParseRemoteUrl(string url)
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
        /// <param name="projectInfo">When successful, contains a <see cref="GitLabProjectInfo"/> with information about the GitLab project the remote url belongs to.</param>
        /// <returns>Returns <c>true</c> if the specified url could be parsed, otherwise returns <c>false</c>.</returns>
        public bool TryParseRemoteUrl(string url, [NotNullWhen(true)] out GitLabProjectInfo? projectInfo) =>
            TryParseRemoteUrl(url, out projectInfo, out var _);


        private static bool TryParseRemoteUrl(string url, [NotNullWhen(true)] out GitLabProjectInfo? projectInfo, [NotNullWhen(false)] out string? errorMessage)
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
                    var projectPath = Uri.UnescapeDataString(uri.AbsolutePath).Trim('/');

                    if (!projectPath.EndsWith(".git", StringComparison.OrdinalIgnoreCase))
                    {
                        errorMessage = $"Cannot parse '{url}' as GitLab url: Expected url to end with '.git'";
                        return false;
                    }

                    projectPath = projectPath.RemoveSuffix(".git", StringComparison.OrdinalIgnoreCase);

                    if (String.IsNullOrWhiteSpace(projectPath))
                    {
                        errorMessage = $"Cannot parse '{url}' as GitLab url: Project path is empty";
                        return false;
                    }

                    if (!projectPath.Contains("/"))
                    {
                        errorMessage = $"Cannot parse '{url}' as GitLab url: Invalid project path '{projectPath}'";
                        return false;
                    }

                    var splitIndex = projectPath.LastIndexOf('/');
                    var @namespace = projectPath.Substring(0, splitIndex).Trim('/');
                    var projectName = projectPath.Substring(splitIndex).Trim('/');

                    if (String.IsNullOrWhiteSpace(@namespace))
                    {
                        errorMessage = $"Cannot parse '{url}' as GitLab url: Project namespace is empty";
                        return false;
                    }

                    if (String.IsNullOrWhiteSpace(projectName))
                    {
                        errorMessage = $"Cannot parse '{url}' as GitLab url: Project name is empty";
                        return false;
                    }

                    projectInfo = new GitLabProjectInfo(uri.Host, @namespace, projectName);
                    return true;

                default:
                    errorMessage = $"Cannot parse '{url}' as GitLab url: Unsupported scheme '{uri.Scheme}'";
                    return false;
            }
        }
    }
}
