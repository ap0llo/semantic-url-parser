using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Grynwald.SemanticUrlParser.Utilities;

namespace Grynwald.SemanticUrlParser.GitLab
{
    /// <summary>
    /// Parses GitLab git remote URLs and returns a <see cref="GitLabProjectInfo"/>.
    /// </summary>
    /// <remarks>
    /// Supports both HTTP and SSH git urls.
    /// </remarks>
    /// <example>
    /// Get the name of a GitLab project from a SSH url.
    /// <code language="csharp">
    /// var parser = new GitLabRemoteUrlParser();
    /// var projectInfo = parser.ParseUrl("git@gitlab.com:user/my-repo.git");
    ///
    /// Console.WriteLine(projectInfo.Owner);       // Prints 'user'
    /// Console.WriteLine(projectInfo.Repository);  // Prints 'my-repo'
    /// </code>
    /// </example>
    public sealed class GitLabRemoteUrlParser : GitLabUrlParser<GitLabProjectInfo>
    {
        protected override IEnumerable<string> SupportedSchemes { get; } = new[] { "http", "https", "ssh" };


        protected override bool TryCreateUri(string url, [NotNullWhen(true)] out Uri? result) => GitRemoteUrl.TryGetUri(url, out result);

        protected override bool TryParsePath(Uri uri, string path, [NotNullWhen(true)] out GitLabProjectInfo? result, [NotNullWhen(false)] out string? errorMessage)
        {
            var projectPath = Uri.UnescapeDataString(uri.AbsolutePath).Trim('/');

            if (!projectPath.EndsWith(".git", StringComparison.OrdinalIgnoreCase))
            {
                result = null;
                errorMessage = $"Cannot parse '{uri}' as GitLab url: Expected url to end with '.git'";
                return false;
            }

            projectPath = projectPath.RemoveSuffix(".git", StringComparison.OrdinalIgnoreCase);

            if (String.IsNullOrWhiteSpace(projectPath))
            {
                result = null;
                errorMessage = $"Cannot parse '{uri}' as GitLab url: Project path is empty";
                return false;
            }

            if (!projectPath.Contains("/"))
            {
                result = null;
                errorMessage = $"Cannot parse '{uri}' as GitLab url: Invalid project path '{projectPath}'";
                return false;
            }

            var splitIndex = projectPath.LastIndexOf('/');
            var @namespace = projectPath.Substring(0, splitIndex).Trim('/');
            var projectName = projectPath.Substring(splitIndex).Trim('/');

            return TryCreateProjectInfo(uri.Host, @namespace, projectName, out result, out errorMessage);
        }
    }
}
