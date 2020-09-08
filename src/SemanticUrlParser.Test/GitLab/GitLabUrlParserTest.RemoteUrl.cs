using System;
using Grynwald.SemanticUrlParser.GitLab;
using Xunit;

namespace Grynwald.SemanticUrlParser.Test.GitLab
{
    public partial class GitLabUrlParserTest
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("\t")]
        [InlineData("  ")]
        [InlineData("not-a-url")]
        [InlineData("ftp://gitlab.com/user/repo.git")]  // unsupported scheme
        [InlineData("http://gitlab.com")]               // missing project path
        [InlineData("http://gitlab.com/user/repo")]     // missing .git suffix
        [InlineData("http://gitlab.com/user")]          // missing project name and .git suffix
        [InlineData("http://gitlab.com/user/.git")]     // missing project name
        [InlineData("http://gitlab.com//repo.git")]     // missing namespace
        public void ParseRemoteUrl_throws_ArgumentException_for_invalid_input(string url)
        {
            var sut = new GitLabUrlParser();
            Assert.ThrowsAny<ArgumentException>(() => sut.ParseRemoteUrl(url));
        }

        [Theory]
        [InlineData("https://gitlab.com/user/repoName.git", "gitlab.com", "user", "reponame")]
        [InlineData("https://gitlab.com/group/subgroup/repoName.git", "gitlab.com", "group/subgroup", "reponame")]
        [InlineData("https://example.com/user/repoName.git", "example.com", "user", "reponame")]
        [InlineData("git@gitlab.com:user/repoName.git", "gitlab.com", "user", "repoName")]
        [InlineData("git@gitlab.com:group/subgroup/repoName.git", "gitlab.com", "group/subgroup", "repoName")]
        [InlineData("git@example.com:user/repoName.git", "example.com", "user", "repoName")]
        [InlineData("git@example.com:group/subgroup/repoName.git", "example.com", "group/subgroup", "repoName")]
        public void ParseRemoteUrl_returns_the_expected_GitLabProjectInfo(string remoteUrl, string host, string @namespace, string proejctName)
        {
            // ARRANGE
            var sut = new GitLabUrlParser();
            var expected = new GitLabProjectInfo(host, @namespace, proejctName);

            // ACT
            var actual = sut.ParseRemoteUrl(remoteUrl);

            // ASSERT
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("\t")]
        [InlineData("  ")]
        [InlineData("not-a-url")]
        [InlineData("ftp://gitlab.com/user/repo.git")]  // unsupported scheme
        [InlineData("http://gitlab.com")]               // missing project path
        [InlineData("http://gitlab.com/user/repo")]     // missing .git suffix
        [InlineData("http://gitlab.com/user")]          // missing project name and .git suffix
        [InlineData("http://gitlab.com/user/.git")]     // missing project name
        [InlineData("http://gitlab.com//repo.git")]     // missing namespace
        public void TryParseRemoteUrl_returns_false_for_invalid_input(string url)
        {
            var sut = new GitLabUrlParser();
            Assert.False(sut.TryParseRemoteUrl(url, out var uri));
        }

        [Theory]
        [InlineData("https://gitlab.com/user/repoName.git", "gitlab.com", "user", "reponame")]
        [InlineData("https://gitlab.com/group/subgroup/repoName.git", "gitlab.com", "group/subgroup", "reponame")]
        [InlineData("https://example.com/user/repoName.git", "example.com", "user", "reponame")]
        [InlineData("git@gitlab.com:user/repoName.git", "gitlab.com", "user", "repoName")]
        [InlineData("git@gitlab.com:group/subgroup/repoName.git", "gitlab.com", "group/subgroup", "repoName")]
        [InlineData("git@example.com:user/repoName.git", "example.com", "user", "repoName")]
        [InlineData("git@example.com:group/subgroup/repoName.git", "example.com", "group/subgroup", "repoName")]
        public void TryParseRemoteUrl_returns_the_expected_GitHubProjectInfo(string url, string host, string @namespace, string projectName)
        {
            // ARRANGE
            var expected = new GitLabProjectInfo(host, @namespace, projectName);
            var sut = new GitLabUrlParser();

            // ACT 
            var success = sut.TryParseRemoteUrl(url, out var projectInfo);

            // ASSERT
            Assert.True(success);
            Assert.Equal(expected, projectInfo);
        }
    }
}
