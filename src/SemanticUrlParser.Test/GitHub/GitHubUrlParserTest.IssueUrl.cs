using System;
using System.Collections.Generic;
using Grynwald.SemanticUrlParser.GitHub;
using Xunit;

namespace Grynwald.SemanticUrlParser.Test.GitHub
{
    public partial class GitHubUrlParserTest
    {
        public static IEnumerable<object?[]> IssueUrlNegativeTestCases()
        {
            static object?[] TestCase(string? url)
            {
                return new object?[] { url };
            }

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

        public static IEnumerable<object?[]> IssueUrlPositiveTestCases()
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
        [MemberData(nameof(IssueUrlNegativeTestCases))]
        [MemberData(nameof(CommonNegativeTestCases))]
        public void ParseIssueUrl_throws_ArgumentException_for_invalid_input(string url)
        {
            var sut = new GitHubUrlParser();
            Assert.ThrowsAny<ArgumentException>(() => sut.ParseIssueUrl(url));
        }

        [Theory]
        [MemberData(nameof(IssueUrlPositiveTestCases))]
        public void ParseIssueUrl_returns_the_expected_GitHubIssueInfo(string url, string host, string owner, string repository, int number)
        {
            // ARRANGE
            var sut = new GitHubUrlParser();

            // ACT 
            var issueInfo = sut.ParseIssueUrl(url);

            // ASSERT
            Assert.NotNull(issueInfo);
            Assert.Equal(host, issueInfo.Project.Host);
            Assert.Equal(owner, issueInfo.Project.Owner);
            Assert.Equal(repository, issueInfo.Project.Repository);
            Assert.Equal(number, issueInfo.Number);
        }

        [Theory]
        [MemberData(nameof(IssueUrlNegativeTestCases))]
        [MemberData(nameof(CommonNegativeTestCases))]
        public void TryParseIssueUrl_returns_false_for_invalid_input(string url)
        {
            var sut = new GitHubUrlParser();
            Assert.False(sut.TryParseIssueUrl(url, out var uri));
            Assert.Null(uri);
        }

        [Theory]
        [MemberData(nameof(IssueUrlPositiveTestCases))]
        public void TryParseIssueUrl_returns_the_expected_GitHubIssueInfo(string url, string host, string owner, string repository, int number)
        {
            // ARRANGE
            var sut = new GitHubUrlParser();

            // ACT 
            var success = sut.TryParseIssueUrl(url, out var issueInfo);

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
