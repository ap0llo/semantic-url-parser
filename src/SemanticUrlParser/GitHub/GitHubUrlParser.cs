using System;

namespace Grynwald.SemanticUrlParser.GitHub
{
    /// <summary>
    /// Parser for GitHub urls
    /// </summary>
    public abstract class GitHubUrlParser
    {
        protected bool CheckScheme(Uri uri, params string[] supportedSchemes)
        {
            foreach (var scheme in supportedSchemes)
            {
                if (scheme.Equals(uri.Scheme, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
