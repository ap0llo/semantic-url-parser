using System;
using System.Collections.Generic;
using Grynwald.SemanticUrlParser.GitHub;
using Xunit;

namespace Grynwald.SemanticUrlParser.Test.GitHub
{
    /// <summary>
    /// Tests for <see cref="GitHubIssueUrlParser"/>
    /// </summary>
    public sealed class GitHubIssueUrlParserTest : GitHubUrlParserTest<GitHubIssueInfo>
    {
        protected override GitHubUrlParser<GitHubIssueInfo> CreateInstance() => new GitHubIssueUrlParser();


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

            // invalid issue numbers
            yield return TestCase("https://github.com/owner/repo/issues/not-a-number");
            yield return TestCase("https://github.com/owner/repo/issues/ ");
            yield return TestCase("https://github.com/owner/repo/issues/0");
            yield return TestCase("https://github.com/owner/repo/issues/-23");

            // too many segments
            yield return TestCase("https://github.com/owner/repo/issues/some-name/1");

            // too few segments
            yield return TestCase("https://github.com/owner/repo/1");

            // empty or whitespace owner
            yield return TestCase("https://github.com//repo/issues/1");
            yield return TestCase("https://github.com/ /repo/issues/1");

            // empty or whitespace repository name
            yield return TestCase("https://github.com/owner//issues/1");
            yield return TestCase("https://github.com/owner/ /issues/1");
        }

        public static IEnumerable<object?[]> PositiveTestCases()
        {
            static object?[] TestCase(string url, string host, string owner, string repository, int number)
            {
                return new object?[] { url, host, owner, repository, number };
            }


            yield return TestCase("https://github.com/owner/repo/issues/1", "github.com", "owner", "repo", 1);
            yield return TestCase("http://github.com/owner/repo/issues/123", "github.com", "owner", "repo", 123);
            yield return TestCase("https://github.com/owner/repo/issues/123", "github.com", "owner", "repo", 123);
            yield return TestCase("HTTP://github.com/owner/repo/issues/123", "github.com", "owner", "repo", 123);
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
        public void ParseUrl_returns_the_expected_GitHubIssueInfo(string url, string host, string owner, string repository, int number)
        {
            // ARRANGE
            var sut = CreateInstance();

            // ACT 
            var issueInfo = sut.ParseUrl(url);

            // ASSERT
            Assert.NotNull(issueInfo);
            Assert.Equal(host, issueInfo.Project.Host);
            Assert.Equal(owner, issueInfo.Project.Owner);
            Assert.Equal(repository, issueInfo.Project.Repository);
            Assert.Equal(number, issueInfo.Number);
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
        public void TryParseUrl_returns_the_expected_GitHubIssueInfo(string url, string host, string owner, string repository, int number)
        {
            // ARRANGE
            var sut = CreateInstance();

            // ACT 
            var success = sut.TryParseUrl(url, out var issueInfo);

            // ASSERT
            Assert.True(success);
            Assert.NotNull(issueInfo);
            Assert.Equal(host, issueInfo!.Project.Host);
            Assert.Equal(owner, issueInfo.Project.Owner);
            Assert.Equal(repository, issueInfo.Project.Repository);
            Assert.Equal(number, issueInfo.Number);
        }
    }
}
