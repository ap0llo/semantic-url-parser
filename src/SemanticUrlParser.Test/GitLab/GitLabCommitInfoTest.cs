using System;
using System.Collections.Generic;
using Grynwald.SemanticUrlParser.GitLab;
using Xunit;

namespace Grynwald.SemanticUrlParser.Test.GitLab
{
    /// <summary>
    /// Tests for <see cref="GitLabCommitInfo"/>
    /// </summary>
    public class GitLabCommitInfoTest : EqualityTest<GitLabCommitInfo, GitLabCommitInfoTest>, IEqualityTestDataProvider<GitLabCommitInfo>
    {
        public IEnumerable<(GitLabCommitInfo left, GitLabCommitInfo right)> GetEqualTestCases()
        {
            var project1 = new GitLabProjectInfo("example.com", "namespace", "project");
            var project2 = new GitLabProjectInfo("example.com", "namespace", "project");

            yield return (new GitLabCommitInfo(project1, "abc123"), new GitLabCommitInfo(project2, "abc123"));
            yield return (new GitLabCommitInfo(project1, "abc123"), new GitLabCommitInfo(project2, "ABC123"));
        }

        public IEnumerable<(GitLabCommitInfo left, GitLabCommitInfo right)> GetUnequalTestCases()
        {
            var project1 = new GitLabProjectInfo("example.com", "namespace", "project1");
            var project2 = new GitLabProjectInfo("example.com", "namespace", "project2");

            yield return (new GitLabCommitInfo(project1, "abc123"), new GitLabCommitInfo(project1, "def467"));
            yield return (new GitLabCommitInfo(project1, "abc123"), new GitLabCommitInfo(project2, "abc123"));
        }


        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("\t")]
        [InlineData("  ")]
        public void CommitId_must_not_be_null_or_whitespace(string commitId)
        {
            // ARRANGE
            var project = new GitLabProjectInfo("example.com", "owner", "repo");
            Action act = () => new GitLabCommitInfo(project, commitId: commitId);

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
            Action act = () => new GitLabCommitInfo(project: null!, "abc123");

            // ACT 
            var ex = Record.Exception(act);

            // ASSERT
            var argumentNullException = Assert.IsType<ArgumentNullException>(ex);
            Assert.Equal("project", argumentNullException.ParamName);
        }
    }
}
