using System;
using System.Collections.Generic;
using Grynwald.SemanticUrlParser.GitHub;
using Xunit;

namespace Grynwald.SemanticUrlParser.Test.GitHub
{
    /// <summary>
    /// Tests for <see cref="GitHubCommitUrlParser"/>
    /// </summary>
    public sealed class GitHubCommitUrlParserTest : GitHubUrlParserTest<GitHubCommitInfo>
    {
        protected override GitHubUrlParser<GitHubCommitInfo> CreateInstance() => new GitHubCommitUrlParser();


        public static IEnumerable<object?[]> NegativeTestCases()
        {
            static object?[] TestCase(string? url)
            {
                return new object?[] { url };
            }

            // unsupported scheme
            yield return TestCase("ftp://github.com/owner/repo.git");

            // PR url
            yield return TestCase("https://github.com/owner/repo/pull/123");

            // Issue url
            yield return TestCase("https://github.com/owner/repo/issues/123");

            // invalid issue numbers
            yield return TestCase("https://github.com/owner/repo/issues/not-a-number");
            yield return TestCase("https://github.com/owner/repo/issues/ ");
            yield return TestCase("https://github.com/owner/repo/issues/0");
            yield return TestCase("https://github.com/owner/repo/issues/-23");

            // too many segments
            yield return TestCase("https://github.com/owner/repo/commit/abc123/1");

            // too few segments
            yield return TestCase("https://github.com/owner/repo/abc123");

            // empty or whitespace owner
            yield return TestCase("https://github.com//repo/commit/abc123");
            yield return TestCase("https://github.com/ /repo/commit/abc123");

            // empty or whitespace repository name
            yield return TestCase("https://github.com/owner//commit/abc123");
            yield return TestCase("https://github.com/owner/ /commit/abc123");
        }

        public static IEnumerable<object?[]> PositiveTestCases()
        {
            static object?[] TestCase(string url, string host, string owner, string repository, string commitId)
            {
                return new object?[] { url, host, owner, repository, commitId };
            }

            yield return TestCase("https://github.com/owner/repo/commit/abc123", "github.com", "owner", "repo", "abc123");
            yield return TestCase("http://github.com/owner/repo/commit/abc123", "github.com", "owner", "repo", "abc123");
            yield return TestCase("http://github.com/owner/repo/commit/f9f35c3bd67e1b26186cd28d01043ed1c2cd6c70", "github.com", "owner", "repo", "f9f35c3bd67e1b26186cd28d01043ed1c2cd6c70");
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
        public void ParseUrl_returns_the_expected_GitHubCommitInfo(string url, string host, string owner, string repository, string commitId)
        {
            // ARRANGE
            var sut = CreateInstance();

            // ACT 
            var commitInfo = sut.ParseUrl(url);

            // ASSERT
            Assert.NotNull(commitInfo);
            Assert.Equal(host, commitInfo.Project.Host);
            Assert.Equal(owner, commitInfo.Project.Owner);
            Assert.Equal(repository, commitInfo.Project.Repository);
            Assert.Equal(commitId, commitInfo.CommitId);
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
        public void TryParseUrl_returns_the_expected_GitHubCommitInfo(string url, string host, string owner, string repository, string commitId)
        {
            // ARRANGE
            var sut = CreateInstance();

            // ACT 
            var success = sut.TryParseUrl(url, out var commitInfo);

            // ASSERT
            Assert.True(success);
            Assert.NotNull(commitInfo);
            Assert.Equal(host, commitInfo!.Project.Host);
            Assert.Equal(owner, commitInfo.Project.Owner);
            Assert.Equal(repository, commitInfo.Project.Repository);
            Assert.Equal(commitId, commitInfo.CommitId);
        }
    }
}
