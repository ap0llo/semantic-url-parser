using System;
using System.Collections.Generic;
using Grynwald.SemanticUrlParser.GitHub;
using Xunit;

namespace Grynwald.SemanticUrlParser.Test.GitHub
{
    /// <summary>
    /// Tests for <see cref="GitHubRemoteUrlParser"/>
    /// </summary>
    public sealed class GitHubRemoteUrlParserTest
    {
        public static IEnumerable<object?[]> RemoteUrlNegativeTestCases()
        {
            static object?[] TestCase(string? url)
            {
                return new object?[] { url };
            }

            // to many segments in the path
            yield return TestCase("http://github.com/owner/another-name/repo.git");

            // empty or whitespace project path
            yield return TestCase("http://github.com/");
            yield return TestCase("http://github.com/ ");
            yield return TestCase("http://github.com/.git");
            yield return TestCase("http://github.com/ .git");

            // empty or whitespace owner
            yield return TestCase("http://github.com//repo.git");
            yield return TestCase("http://github.com/ /repo.git");

            // empty or whitespace repository name
            yield return TestCase("http://github.com/owner/.git");
            yield return TestCase("http://github.com/owner/ .git");

            // missing ".git" suffix
            yield return TestCase("http://github.com/owner/repo");

        }

        public static IEnumerable<object?[]> RemoteUrlPositiveTestCases()
        {
            static object?[] TestCase(string url, string host, string owner, string repository)
            {
                return new object?[] { url, host, owner, repository };
            }

            yield return TestCase("http://github.com/owner/repo-name.git", "github.com", "owner", "repo-name");
            yield return TestCase("http://github.com/owner/repo-name.GIT", "github.com", "owner", "repo-name");
            yield return TestCase("https://github.com/owner/repo-name.git", "github.com", "owner", "repo-name");
            yield return TestCase("https://github.com/owner/repo-name.GIT", "github.com", "owner", "repo-name");
            yield return TestCase("git@github.com:owner/repo-name.git", "github.com", "owner", "repo-name");
            yield return TestCase("git@github.com:owner/repo-name.GIT", "github.com", "owner", "repo-name");
            yield return TestCase("ssh://git@github.com/owner/repo-name.git", "github.com", "owner", "repo-name");
            yield return TestCase("ssh://git@github.com/owner/repo-name.GIT", "github.com", "owner", "repo-name");
        }


        [Theory]
        [MemberData(nameof(RemoteUrlNegativeTestCases))]
        [MemberData(nameof(GitHubUrlParserTest.CommonNegativeTestCases), MemberType = typeof(GitHubUrlParserTest))]
        public void ParseRemoteUrl_throws_ArgumentException_for_invalid_input(string url)
        {
            var sut = new GitHubRemoteUrlParser();
            Assert.ThrowsAny<ArgumentException>(() => sut.ParseRemoteUrl(url));
        }

        [Theory]
        [MemberData(nameof(RemoteUrlPositiveTestCases))]
        public void ParseRemoteUrl_returns_the_expected_GitHubProjectInfo(string url, string host, string owner, string repository)
        {
            // ARRANGE
            var sut = new GitHubRemoteUrlParser();

            // ACT 
            var projectInfo = sut.ParseRemoteUrl(url);

            // ASSERT
            Assert.NotNull(projectInfo);
            Assert.Equal(host, projectInfo.Host);
            Assert.Equal(owner, projectInfo.Owner);
            Assert.Equal(repository, projectInfo.Repository);
        }

        [Theory]
        [MemberData(nameof(RemoteUrlNegativeTestCases))]
        [MemberData(nameof(GitHubUrlParserTest.CommonNegativeTestCases), MemberType = typeof(GitHubUrlParserTest))]
        public void TryParseRemoteUrl_returns_false_for_invalid_input(string url)
        {
            var sut = new GitHubRemoteUrlParser();
            Assert.False(sut.TryParseRemoteUrl(url, out var uri));
            Assert.Null(uri);
        }

        [Theory]
        [MemberData(nameof(RemoteUrlPositiveTestCases))]
        public void TryParseRemoteUrl_returns_the_expected_GitHubProjectInfo(string url, string host, string owner, string repository)
        {
            // ARRANGE
            var sut = new GitHubRemoteUrlParser();

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
