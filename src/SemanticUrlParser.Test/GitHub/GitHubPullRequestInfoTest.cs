using System;
using System.Collections.Generic;
using Grynwald.SemanticUrlParser.GitHub;
using Xunit;

namespace Grynwald.SemanticUrlParser.Test.GitHub
{
    /// <summary>
    /// Tests for <see cref="GitHubPullRequestInfo"/>
    /// </summary>
    public class GitHubPullRequestInfoTest : EqualityTest<GitHubPullRequestInfo, GitHubPullRequestInfoTest>, IEqualityTestDataProvider<GitHubPullRequestInfo>
    {
        public IEnumerable<(GitHubPullRequestInfo left, GitHubPullRequestInfo right)> GetEqualTestCases()
        {
            var project1 = new GitHubProjectInfo("example.com", "owner", "repo");
            var project2 = new GitHubProjectInfo("example.com", "owner", "repo");

            yield return (new GitHubPullRequestInfo(project1, 1), new GitHubPullRequestInfo(project2, 1));
        }

        public IEnumerable<(GitHubPullRequestInfo left, GitHubPullRequestInfo right)> GetUnequalTestCases()
        {
            var project1 = new GitHubProjectInfo("example.com", "owner", "repo1");
            var project2 = new GitHubProjectInfo("example.com", "owner", "repo2");

            yield return (new GitHubPullRequestInfo(project1, 1), new GitHubPullRequestInfo(project2, 1));
            yield return (new GitHubPullRequestInfo(project1, 1), new GitHubPullRequestInfo(project1, 23));
        }


        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-10)]
        public void Number_must_not_be_negative_or_zero(int prNumber)
        {
            var project = new GitHubProjectInfo("example.com", "owner", "repo1");
            Assert.Throws<ArgumentOutOfRangeException>(() => new GitHubPullRequestInfo(project, prNumber));
        }

        [Fact]
        public void Project_must_not_be_null()
        {
            Assert.Throws<ArgumentNullException>(() => new GitHubPullRequestInfo(null!, 1));
        }
    }
}
