using System;
using System.Collections.Generic;
using Grynwald.SemanticUrlParser.GitHub;
using Xunit;

namespace Grynwald.SemanticUrlParser.Test.GitHub
{
    /// <summary>
    /// Base test class for tests of implementations of <see cref="GitHubUrlParser"/>
    /// </summary>
    public abstract class GitHubUrlParserTest<T> where T : class
    {
        public static IEnumerable<object?[]> InvalidUriTestCases()
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
        }


        [Theory]
        [MemberData(nameof(InvalidUriTestCases))]
        public void ParseUrl_throws_ArgumentException_for_inputs_that_are_not_valid_uris(string url)
        {
            var sut = new GitHubIssueUrlParser();
            Assert.ThrowsAny<ArgumentException>(() => sut.ParseUrl(url));
        }

        [Theory]
        [MemberData(nameof(InvalidUriTestCases))]
        public void TryParseUrl_returns_false_for_inputs_that_are_not_valid_uris(string url)
        {
            var sut = new GitHubIssueUrlParser();
            Assert.False(sut.TryParseUrl(url, out var uri));
            Assert.Null(uri);
        }

        protected abstract GitHubUrlParser<T> CreateInstance();
    }
}
