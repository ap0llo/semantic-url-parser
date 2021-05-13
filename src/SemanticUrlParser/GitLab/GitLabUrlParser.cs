using System;
using System.Diagnostics.CodeAnalysis;

namespace Grynwald.SemanticUrlParser.GitLab
{
    /// <summary>
    /// Base class for GitLab url parsers
    /// </summary>
    public abstract class GitLabUrlParser<T> : UrlParser<T> where T : class
    {
        protected bool TryCreateProjectInfo(string host, string @namespace, string projectName, [NotNullWhen(true)] out GitLabProjectInfo? result, [NotNullWhen(false)] out string? errorMessage)
        {
            result = default;

            if (String.IsNullOrWhiteSpace(@namespace))
            {
                result = null;
                errorMessage = $"Project namespace must not be empty or whitespace";
                return false;
            }

            if (String.IsNullOrWhiteSpace(projectName))
            {
                result = null;
                errorMessage = $"Project name must not be empty or whitespace";
                return false;
            }

            errorMessage = null;
            result = new GitLabProjectInfo(host, @namespace, projectName);
            return true;
        }

    }
}
