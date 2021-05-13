using System;
using System.Collections.Generic;
using Grynwald.SemanticUrlParser.GitHub;
using Xunit;

namespace Grynwald.SemanticUrlParser.Test.GitHub
{
    /// <summary>
    /// Tests for <see cref="GitHubIssueInfo"/>
    /// </summary>
    public class GitHubIssueInfoTest : EqualityTest<GitHubIssueInfo, GitHubIssueInfoTest>, IEqualityTestDataProvider<GitHubIssueInfo>
    {
        public IEnumerable<(GitHubIssueInfo left, GitHubIssueInfo right)> GetEqualTestCases()
        {
            var project1 = new GitHubProjectInfo("example.com", "owner", "repo");
            var project2 = new GitHubProjectInfo("example.com", "owner", "repo");

            yield return (new GitHubIssueInfo(project1, 1), new GitHubIssueInfo(project2, 1));
        }

        public IEnumerable<(GitHubIssueInfo left, GitHubIssueInfo right)> GetUnequalTestCases()
        {
            var project1 = new GitHubProjectInfo("example.com", "owner", "repo1");
            var project2 = new GitHubProjectInfo("example.com", "owner", "repo2");

            yield return (new GitHubIssueInfo(project1, 1), new GitHubIssueInfo(project2, 1));
            yield return (new GitHubIssueInfo(project1, 1), new GitHubIssueInfo(project1, 23));
        }


        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-10)]
        public void Number_must_not_be_negative_or_zero(int issueNumber)
        {
            var project = new GitHubProjectInfo("example.com", "owner", "repo1");
            Assert.Throws<ArgumentOutOfRangeException>(() => new GitHubIssueInfo(project, issueNumber));
        }

        [Fact]
        public void Project_must_not_be_null()
        {
            Assert.Throws<ArgumentNullException>(() => new GitHubIssueInfo(null!, 1));
        }
    }
}
