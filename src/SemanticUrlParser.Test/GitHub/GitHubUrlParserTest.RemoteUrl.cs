using System;
using Grynwald.SemanticUrlParser.GitHub;
using Xunit;

namespace Grynwald.SemanticUrlParser.Test.GitHub
{
    public partial class GitHubUrlParserTest
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("\t")]
        [InlineData("  ")]
        [InlineData("not-a-url")]
        [InlineData("http://github.com/owner/another-name/repo.git")] // to many segments in the path
        [InlineData("ftp://github.com/owner/repo.git")] // unsupported scheme
        public void ParseRemoteUrl_throws_ArgumentException_for_invalid_input(string url)
        {
            var sut = new GitHubUrlParser();
            Assert.ThrowsAny<ArgumentException>(() => sut.ParseRemoteUrl(url));
        }

        [Theory]
        [InlineData("http://github.com/owner/repo-name.git", "github.com", "owner", "repo-name")]
        [InlineData("https://github.com/owner/repo-name.git", "github.com", "owner", "repo-name")]
        [InlineData("git@github.com:owner/repo-name.git", "github.com", "owner", "repo-name")]
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
        [InlineData(null)]
        [InlineData("")]
        [InlineData("\t")]
        [InlineData("  ")]
        [InlineData("not-a-url")]
        [InlineData("http://github.com/owner/another-name/repo.git")] // to many segments in the path
        [InlineData("ftp://github.com/owner/repo.git")] // unsupported scheme
        public void TryParseRemoteUrl_returns_false_for_invalid_input(string url)
        {
            var sut = new GitHubUrlParser();
            Assert.False(sut.TryParseRemoteUrl(url, out var uri));
        }

        [Theory]
        [InlineData("http://github.com/owner/repo-name.git", "github.com", "owner", "repo-name")]
        [InlineData("https://github.com/owner/repo-name.git", "github.com", "owner", "repo-name")]
        [InlineData("git@github.com:owner/repo-name.git", "github.com", "owner", "repo-name")]
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
