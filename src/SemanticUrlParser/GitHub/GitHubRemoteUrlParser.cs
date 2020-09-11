using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Grynwald.SemanticUrlParser.Utilities;

namespace Grynwald.SemanticUrlParser.GitHub
{
    /// <summary>
    /// Parses GitHub git remote URLs and returns a <see cref="GitHubProjectInfo"/>.
    /// </summary>
    /// <remarks>
    /// Supports both HTTP and SSH git urls.
    /// </remarks>
    /// <example>
    /// Get the name of a GitHub project from a SSH url.
    /// <code language="csharp">
    /// var parser = new GitHubRemoteUrlParser();
    /// var projectInfo = parser.ParseUrl("git@github.com:user/my-repo.git");
    ///
    /// Console.WriteLine(projectInfo.Owner);       // Prints 'user'
    /// Console.WriteLine(projectInfo.Repository);  // Prints 'my-repo'
    /// </code>
    /// </example>
    public sealed partial class GitHubRemoteUrlParser : GitHubUrlParser<GitHubProjectInfo>
    {
        protected override IEnumerable<string> SupportedSchemes { get; } = new[] { "http", "https", "ssh" };


        protected override bool TryCreateUri(string url, [NotNullWhen(true)] out Uri? result) => GitRemoteUrl.TryGetUri(url, out result);

        protected override bool TryParsePath(Uri uri, string path, [NotNullWhen(true)] out GitHubProjectInfo? result, [NotNullWhen(false)] out string? errorMessage)
        {
            // expected path: '<owner>/<repo>/.git'

            result = default;
            errorMessage = default;

            if (!path.EndsWith(".git", StringComparison.OrdinalIgnoreCase))
            {
                errorMessage = $"Expected url to end with '.git'";
                return false;
            }

            path = path.RemoveSuffix(".git", StringComparison.OrdinalIgnoreCase);

            var pathSegments = path.Split('/');
            if (pathSegments.Length != 2)
            {
                errorMessage = $"'{uri}' is not a GitHub remote url";
                return false;
            }

            var (owner, repo) = pathSegments;

            return TryCreateProjectInfo(uri.Host, owner, repo, out result, out errorMessage);
        }
    }
}
