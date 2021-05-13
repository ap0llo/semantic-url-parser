using System;
using System.Collections.Generic;
using Grynwald.SemanticUrlParser.GitLab;
using Xunit;

namespace Grynwald.SemanticUrlParser.Test.GitLab
{
    /// <summary>
    /// Tests for <see cref="GitLabIssueInfo"/>
    /// </summary>
    public class GitLabIssueInfoTest : EqualityTest<GitLabIssueInfo, GitLabIssueInfoTest>, IEqualityTestDataProvider<GitLabIssueInfo>
    {
        public IEnumerable<(GitLabIssueInfo left, GitLabIssueInfo right)> GetEqualTestCases()
        {
            var project1 = new GitLabProjectInfo("example.com", "user", "project");
            var project2 = new GitLabProjectInfo("example.com", "user", "project");

            yield return (new GitLabIssueInfo(project1, 1), new GitLabIssueInfo(project2, 1));
        }

        public IEnumerable<(GitLabIssueInfo left, GitLabIssueInfo right)> GetUnequalTestCases()
        {
            var project1 = new GitLabProjectInfo("example.com", "user", "project1");
            var project2 = new GitLabProjectInfo("example.com", "user", "project2");

            yield return (new GitLabIssueInfo(project1, 1), new GitLabIssueInfo(project2, 1));
            yield return (new GitLabIssueInfo(project1, 1), new GitLabIssueInfo(project1, 23));
        }


        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-10)]
        public void Number_must_not_be_negative_or_zero(int issueNumber)
        {
            var project = new GitLabProjectInfo("example.com", "owner", "repo1");
            Assert.Throws<ArgumentOutOfRangeException>(() => new GitLabIssueInfo(project, issueNumber));
        }

        [Fact]
        public void Project_must_not_be_null()
        {
            Assert.Throws<ArgumentNullException>(() => new GitLabIssueInfo(null!, 1));
        }
    }
}
