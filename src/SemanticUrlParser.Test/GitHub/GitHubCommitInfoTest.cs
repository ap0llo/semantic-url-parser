using System;
using System.Collections.Generic;
using Grynwald.SemanticUrlParser.GitHub;
using Xunit;

namespace Grynwald.SemanticUrlParser.Test.GitHub
{
    /// <summary>
    /// Tests for <see cref="GitHubCommitInfo"/>
    /// </summary>
    public class GitHubCommitInfoTest : EqualityTest<GitHubCommitInfo, GitHubCommitInfoTest>, IEqualityTestDataProvider<GitHubCommitInfo>
    {
        public IEnumerable<(GitHubCommitInfo left, GitHubCommitInfo right)> GetEqualTestCases()
        {
            var project1 = new GitHubProjectInfo("example.com", "owner", "repo");
            var project2 = new GitHubProjectInfo("example.com", "owner", "repo");

            yield return (new GitHubCommitInfo(project1, "abc123"), new GitHubCommitInfo(project2, "abc123"));
            yield return (new GitHubCommitInfo(project1, "abc123"), new GitHubCommitInfo(project2, "ABC123"));
        }

        public IEnumerable<(GitHubCommitInfo left, GitHubCommitInfo right)> GetUnequalTestCases()
        {
            var project1 = new GitHubProjectInfo("example.com", "owner", "repo1");
            var project2 = new GitHubProjectInfo("example.com", "owner", "repo2");

            yield return (new GitHubCommitInfo(project1, "abc123"), new GitHubCommitInfo(project1, "def467"));
            yield return (new GitHubCommitInfo(project1, "abc123"), new GitHubCommitInfo(project2, "abc123"));
        }


        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("\t")]
        [InlineData("  ")]
        public void CommitId_must_not_be_null_or_whitespace(string commitId)
        {
            // ARRANGE
            var project = new GitHubProjectInfo("example.com", "owner", "repo");
            Action act = () => new GitHubCommitInfo(project, commitId: commitId);

            // ACT 
            var ex = Record.Exception(act);

            // ASSERT
            var argumentException = Assert.IsType<ArgumentException>(ex);
            Assert.Equal("commitId", argumentException.ParamName);
        }

        [Fact]
        public void Project_must_not_be_null()
        {
            // ARRANGE
            Action act = () => new GitHubCommitInfo(project: null!, "abc123");

            // ACT 
            var ex = Record.Exception(act);

            // ASSERT
            var argumentNullException = Assert.IsType<ArgumentNullException>(ex);
            Assert.Equal("project", argumentNullException.ParamName);
        }
    }
}
