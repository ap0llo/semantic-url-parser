using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Grynwald.SemanticUrlParser.GitHub
{
    /// <summary>
    /// Base class for GitHub url parsers
    /// </summary>
    /// <typeparam name="T">The model type parsed from the url</typeparam>
    public abstract class GitHubUrlParser<T> : UrlParser<T> where T : class
    {
        protected bool TryCreateProjectInfo(string host, string owner, string repo, [NotNullWhen(true)] out GitHubProjectInfo? result, [NotNullWhen(false)] out string? errorMessage)
        {
            result = default;

            if (String.IsNullOrWhiteSpace(owner))
            {
                errorMessage = "Repository owner must not be null or whitespace";
                return false;
            }

            if (String.IsNullOrWhiteSpace(repo))
            {
                errorMessage = "Repository name must not be null or whitespace";
                return false;
            }

            errorMessage = null;
            result = new GitHubProjectInfo(host, owner, repo);
            return true;
        }
    }
}
