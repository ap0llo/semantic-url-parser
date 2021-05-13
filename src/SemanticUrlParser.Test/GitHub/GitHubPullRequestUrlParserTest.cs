using System;
using System.Collections.Generic;
using Grynwald.SemanticUrlParser.GitHub;
using Xunit;

namespace Grynwald.SemanticUrlParser.Test.GitHub
{
    /// <summary>
    /// Tests for <see cref="GitHuPullRequestUrlParser"/>
    /// </summary>
    public sealed class GitHubPullRequestUrlParserTest : GitHubUrlParserTest<GitHubPullRequestInfo>
    {
        protected override GitHubUrlParser<GitHubPullRequestInfo> CreateInstance() => new GitHubPullRequestUrlParser();


        public static IEnumerable<object?[]> NegativeTestCases()
        {
            static object?[] TestCase(string? url)
            {
                return new object?[] { url };
            }

            // unsupported scheme
            yield return TestCase("ftp://github.com/owner/repo.git");

            // Issue url
            yield return TestCase("https://github.com/owner/repo/issues/123");

            // invalid PR numbers
            yield return TestCase("https://github.com/owner/repo/pull/not-a-number");
            yield return TestCase("https://github.com/owner/repo/pull/ ");
            yield return TestCase("https://github.com/owner/repo/pull/0");
            yield return TestCase("https://github.com/owner/repo/pull/-23");

            // too many segments
            yield return TestCase("https://github.com/owner/repo/pull/some-name/1");

            // too few segments
            yield return TestCase("https://github.com/owner/repo/1");

            // empty or whitespace owner
            yield return TestCase("https://github.com//repo/pull/1");
            yield return TestCase("https://github.com/ /repo/pull/1");

            // empty or whitespace repository name
            yield return TestCase("https://github.com/owner//pull/1");
            yield return TestCase("https://github.com/owner/ /pull/1");
        }

        public static IEnumerable<object?[]> PositiveTestCases()
        {
            static object?[] TestCase(string url, string host, string owner, string repository, int number)
            {
                return new object?[] { url, host, owner, repository, number };
            }


            yield return TestCase("https://github.com/owner/repo/pull/1", "github.com", "owner", "repo", 1);
            yield return TestCase("http://github.com/owner/repo/pull/123", "github.com", "owner", "repo", 123);
            yield return TestCase("https://github.com/owner/repo/pull/123", "github.com", "owner", "repo", 123);
            yield return TestCase("HTTP://github.com/owner/repo/pull/123", "github.com", "owner", "repo", 123);
        }


        [Theory]
        [MemberData(nameof(NegativeTestCases))]
        public void ParseUrl_throws_ArgumentException_for_invalid_input(string url)
        {
            var sut = CreateInstance();
            Assert.ThrowsAny<ArgumentException>(() => sut.ParseUrl(url));
        }

        [Theory]
        [MemberData(nameof(PositiveTestCases))]
        public void ParseUrl_returns_the_expected_GitHubPullRequestInfo(string url, string host, string owner, string repository, int number)
        {
            // ARRANGE
            var sut = CreateInstance();

            // ACT 
            var prInfo = sut.ParseUrl(url);

            // ASSERT
            Assert.NotNull(prInfo);
            Assert.Equal(host, prInfo.Project.Host);
            Assert.Equal(owner, prInfo.Project.Owner);
            Assert.Equal(repository, prInfo.Project.Repository);
            Assert.Equal(number, prInfo.Number);
        }

        [Theory]
        [MemberData(nameof(NegativeTestCases))]
        public void TryParseUrl_returns_false_for_invalid_input(string url)
        {
            var sut = CreateInstance();
            Assert.False(sut.TryParseUrl(url, out var uri));
            Assert.Null(uri);
        }

        [Theory]
        [MemberData(nameof(PositiveTestCases))]
        public void TryParseUrl_returns_the_expected_GitHubPullRequestInfo(string url, string host, string owner, string repository, int number)
        {
            // ARRANGE
            var sut = CreateInstance();

            // ACT 
            var success = sut.TryParseUrl(url, out var prInfo);

            // ASSERT
            Assert.True(success);
            Assert.NotNull(prInfo);
            Assert.Equal(host, prInfo!.Project.Host);
            Assert.Equal(owner, prInfo.Project.Owner);
            Assert.Equal(repository, prInfo.Project.Repository);
            Assert.Equal(number, prInfo.Number);
        }
    }
}
