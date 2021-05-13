using System;
using System.Collections.Generic;
using Grynwald.SemanticUrlParser.GitLab;
using Xunit;

namespace Grynwald.SemanticUrlParser.Test.GitLab
{
    /// <summary>
    /// Tests for <see cref="GitLabRemoteUrlParser"/>
    /// </summary>
    public class GitLabRemoteUrlParserTest
    {
        public static IEnumerable<object?[]> NegativeTestCases()
        {
            static object?[] TestCase(string? url)
            {
                return new object?[] { url };
            }

            // null or whitespace urls
            yield return TestCase(null);
            yield return TestCase("");
            yield return TestCase("\t");
            yield return TestCase("  ");

            // invalid URIs
            yield return TestCase("not-a-url");

            // unsupported scheme
            yield return TestCase("ftp://gitlab.com/user/repo.git");

            // missing project path
            yield return TestCase("http://gitlab.com");

            // missing .git suffix
            yield return TestCase("http://gitlab.com/user/repo");

            // missing project name and .git suffix
            yield return TestCase("http://gitlab.com/user");

            // empty or whitespace project path
            yield return TestCase("http://gitlab.com/");
            yield return TestCase("http://gitlab.com/.git");
            yield return TestCase("http://gitlab.com/ ");
            yield return TestCase("http://gitlab.com/ .git");

            // empty or whitespace namespace
            yield return TestCase("http://gitlab.com//repo.git");
            yield return TestCase("http://gitlab.com/ /repo.git");

            // empty of whitespace name
            yield return TestCase("http://gitlab.com/user/.git");
            yield return TestCase("http://gitlab.com/user/ .git");
        }

        public static IEnumerable<object?[]> PositiveTestCases()
        {
            static object?[] TestCase(string url, string host, string @namespace, string projectName)
            {
                return new object?[] { url, host, @namespace, projectName };
            }

            yield return TestCase("https://gitlab.com/user/repoName.git", "gitlab.com", "user", "reponame");
            yield return TestCase("https://gitlab.com/user/repoName.GIT", "gitlab.com", "user", "reponame");
            yield return TestCase("https://gitlab.com/group/subgroup/repoName.git", "gitlab.com", "group/subgroup", "reponame");
            yield return TestCase("https://gitlab.com/group/subgroup/repoName.GIT", "gitlab.com", "group/subgroup", "reponame");
            yield return TestCase("https://example.com/user/repoName.git", "example.com", "user", "reponame");
            yield return TestCase("https://example.com/user/repoName.GIT", "example.com", "user", "reponame");
            yield return TestCase("git@gitlab.com:user/repoName.git", "gitlab.com", "user", "repoName");
            yield return TestCase("git@gitlab.com:user/repoName.GIT", "gitlab.com", "user", "repoName");
            yield return TestCase("git@gitlab.com:group/subgroup/repoName.git", "gitlab.com", "group/subgroup", "repoName");
            yield return TestCase("git@gitlab.com:group/subgroup/repoName.GIT", "gitlab.com", "group/subgroup", "repoName");
            yield return TestCase("git@example.com:user/repoName.git", "example.com", "user", "repoName");
            yield return TestCase("git@example.com:user/repoName.GIT", "example.com", "user", "repoName");
            yield return TestCase("git@example.com:group/subgroup/repoName.git", "example.com", "group/subgroup", "repoName");
            yield return TestCase("git@example.com:group/subgroup/repoName.GIT", "example.com", "group/subgroup", "repoName");
        }


        [Theory]
        [MemberData(nameof(NegativeTestCases))]
        public void ParseRemoteUrl_throws_ArgumentException_for_invalid_input(string url)
        {
            var sut = new GitLabRemoteUrlParser();
            Assert.ThrowsAny<ArgumentException>(() => sut.ParseUrl(url));
        }

        [Theory]
        [MemberData(nameof(PositiveTestCases))]
        public void ParseRemoteUrl_returns_the_expected_GitLabProjectInfo(string url, string host, string @namespace, string projectName)
        {
            // ARRANGE
            var sut = new GitLabRemoteUrlParser();
            var expected = new GitLabProjectInfo(host, @namespace, projectName);

            // ACT
            var actual = sut.ParseUrl(url);

            // ASSERT
            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(NegativeTestCases))]
        public void TryParseRemoteUrl_returns_false_for_invalid_input(string url)
        {
            var sut = new GitLabRemoteUrlParser();
            Assert.False(sut.TryParseUrl(url, out var uri));
            Assert.Null(uri);
        }

        [Theory]
        [MemberData(nameof(PositiveTestCases))]
        public void TryParseRemoteUrl_returns_the_expected_GitHubProjectInfo(string url, string host, string @namespace, string projectName)
        {
            // ARRANGE
            var expected = new GitLabProjectInfo(host, @namespace, projectName);
            var sut = new GitLabRemoteUrlParser();

            // ACT 
            var success = sut.TryParseUrl(url, out var projectInfo);

            // ASSERT
            Assert.True(success);
            Assert.Equal(expected, projectInfo);
        }
    }
}
