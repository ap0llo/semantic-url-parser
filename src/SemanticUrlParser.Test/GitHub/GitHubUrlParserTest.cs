using System.Collections.Generic;
using Grynwald.SemanticUrlParser.GitHub;

namespace Grynwald.SemanticUrlParser.Test.GitHub
{
    /// <summary>
    /// Base test class for tests of implementations of <see cref="GitHubUrlParser"/>
    /// </summary>
    public abstract class GitHubUrlParserTest
    {
        public static IEnumerable<object?[]> CommonNegativeTestCases()
        {
            static object?[] TestCase(string? url)
            {
                return new object?[] { url };
            }

            // null or whitespace
            yield return TestCase(null);
            yield return TestCase("");
            yield return TestCase("\t");
            yield return TestCase("  ");

            // invalid URIs
            yield return TestCase("not-a-url");

            // unsupported scheme
            yield return TestCase("ftp://github.com/owner/repo.git");
        }
    }
}
