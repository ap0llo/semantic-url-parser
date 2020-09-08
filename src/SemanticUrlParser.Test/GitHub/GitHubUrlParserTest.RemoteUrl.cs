using System;
using System.Collections.Generic;
using Grynwald.SemanticUrlParser.GitHub;
using Xunit;

namespace Grynwald.SemanticUrlParser.Test.GitHub
{
    public partial class GitHubUrlParserTest
    {
        public static IEnumerable<object?[]> NegativeTestCases()
        {
            object?[] TestCase(string? url)
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

            // to many segments in the path
            yield return TestCase("http://github.com/owner/another-name/repo.git");

            // unsupported scheme
            yield return TestCase("ftp://github.com/owner/repo.git");
        }

        public static IEnumerable<object?[]> PositiveTestCases()
        {
            object?[] TestCase(string url, string host, string owner, string repository)
            {
                return new object?[] { url, host, owner, repository };
            }

            yield return TestCase("http://github.com/owner/repo-name.git", "github.com", "owner", "repo-name");
            yield return TestCase("https://github.com/owner/repo-name.git", "github.com", "owner", "repo-name");
            yield return TestCase("git@github.com:owner/repo-name.git", "github.com", "owner", "repo-name");
        }


        [Theory]
        [MemberData(nameof(NegativeTestCases))]
        public void ParseRemoteUrl_throws_ArgumentException_for_invalid_input(string url)
        {
            var sut = new GitHubUrlParser();
            Assert.ThrowsAny<ArgumentException>(() => sut.ParseRemoteUrl(url));
        }

        [Theory]
        [MemberData(nameof(PositiveTestCases))]
        public void ParseRemoteUrl_returns_the_expected_GitHubProjectInfo(string url, string host, string owner, string repository)
        {
            // ARRANGE
            var sut = new GitHubUrlParser();

            // ACT 
            var projectInfo = sut.ParseRemoteUrl(url);

            // ASSERT
            Assert.NotNull(projectInfo);
            Assert.Equal(host, projectInfo.Host);
            Assert.Equal(owner, projectInfo.Owner);
            Assert.Equal(repository, projectInfo.Repository);
        }

        [Theory]
        [MemberData(nameof(NegativeTestCases))]
        public void TryParseRemoteUrl_returns_false_for_invalid_input(string url)
        {
            var sut = new GitHubUrlParser();
            Assert.False(sut.TryParseRemoteUrl(url, out var uri));
        }

        [Theory]
        [MemberData(nameof(PositiveTestCases))]
        public void TryParseRemoteUrl_returns_the_expected_GitHubProjectInfo(string url, string host, string owner, string repository)
        {
            // ARRANGE
            var sut = new GitHubUrlParser();

            // ACT 
            var success = sut.TryParseRemoteUrl(url, out var projectInfo);

            // ASSERT
            Assert.True(success);
            Assert.NotNull(projectInfo);
            Assert.Equal(host, projectInfo!.Host);
            Assert.Equal(owner, projectInfo.Owner);
            Assert.Equal(repository, projectInfo.Repository);
        }
    }
}
