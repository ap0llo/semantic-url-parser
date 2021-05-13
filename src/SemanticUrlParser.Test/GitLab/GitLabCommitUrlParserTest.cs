using System;
using System.Collections.Generic;
using Grynwald.SemanticUrlParser.GitLab;
using Xunit;

namespace Grynwald.SemanticUrlParser.Test.GitLab
{
    /// <summary>
    /// Tests for <see cref="GitLabCommitUrlParser"/>
    /// </summary>
    public class GitLabCommitUrlParserTest : UrlParserTest<GitLabCommitInfo>
    {

        protected override GitLabUrlParser<GitLabCommitInfo> CreateInstance() => new GitLabCommitUrlParser();

        public static IEnumerable<object?[]> NegativeTestCases()
        {
            static object?[] TestCase(string? url)
            {
                return new object?[] { url };
            }

            // unsupported scheme
            yield return TestCase("ftp://gitlab.com/user/project.git");

            // Merge Request url
            yield return TestCase("https://gitlab.com/namespace/project/-/merge_requests/1");

            // Issue url
            yield return TestCase("https://gitlab.com/namespace/project/-/issues/1");

            // Missing or misplaced '-'
            yield return TestCase("https://gitlab.com/namespace/project/commit/abc123");
            yield return TestCase("https://gitlab.com/namespace/-/project/commit/abc123");

            // too many segments
            yield return TestCase("https://gitlab.com/namespace/project/-/commit/abc123/some-value");

            // too few segments
            yield return TestCase("https://gitlab.com/namespace/project/-/abc123");
            yield return TestCase("https://gitlab.com/namespace/project/-/commit");
            yield return TestCase("https://gitlab.com/project/-/commit/abc123");

            // empty or whitespace namesapce
            yield return TestCase("https://gitlab.com//project/-/commit/abc123");
            yield return TestCase("https://gitlab.com/ /project/-/commit/abc123");

            // empty or whitespace project name
            yield return TestCase("https://gitlab.com/namespace//-/commit/abc123");
            yield return TestCase("https://gitlab.com/namespace/ /-/commit/abc123");

            // empty or whitespace commit id
            yield return TestCase("https://gitlab.com/namespace/project/-/commit/ /");
        }


        [Theory]
        [MemberData(nameof(NegativeTestCases))]
        public void ParseUrl_throws_ArgumentException_for_invalid_input(string url)
        {
            var sut = CreateInstance();
            Assert.ThrowsAny<ArgumentException>(() => sut.ParseUrl(url));
        }


        public static IEnumerable<object?[]> PositiveTestCases()
        {
            static object?[] TestCase(string url, string host, string @namespace, string projectName, string commitId)
            {
                return new object?[] { url, host, @namespace, projectName, commitId };
            }

            yield return TestCase("https://gitlab.com/user/project/-/commit/abc123", "gitlab.com", "user", "project", "abc123");
            yield return TestCase("https://gitlab.com/group/subgroup/project/-/commit/abc123", "gitlab.com", "group/subgroup", "project", "abc123");

            // Trailing slashes are ignored
            yield return TestCase("https://gitlab.com/group/subgroup/project/-/commit/abc123/", "gitlab.com", "group/subgroup", "project", "abc123");

        }


        [Theory]
        [MemberData(nameof(PositiveTestCases))]
        public void ParseUrl_returns_the_expected_GitLabCommitInfo(string url, string host, string @namespace, string projectName, string commitId)
        {
            // ARRANGE
            var sut = CreateInstance();

            // ACT 
            var commitInfo = sut.ParseUrl(url);

            // ASSERT
            Assert.NotNull(commitInfo);
            Assert.Equal(host, commitInfo.Project.Host);
            Assert.Equal(@namespace, commitInfo.Project.Namespace);
            Assert.Equal(projectName, commitInfo.Project.Project);
            Assert.Equal(commitId, commitInfo.CommitId);
        }

        [Theory]
        [MemberData(nameof(NegativeTestCases))]
        public void TryParseUrl_returns_false_for_invalid_input(string url)
        {
            var sut = CreateInstance();
            Assert.False(sut.TryParseUrl(url, out var uri));
            Assert.Null(uri);
        }

        [Theory]
        [MemberData(nameof(PositiveTestCases))]
        public void TryParseUrl_returns_the_expected_GitLabCommitInfo(string url, string host, string @namespace, string projectName, string commitId)
        {
            // ARRANGE
            var sut = CreateInstance();

            // ACT 
            var success = sut.TryParseUrl(url, out var commitInfo);

            // ASSERT
            Assert.True(success);
            Assert.NotNull(commitInfo);
            Assert.Equal(host, commitInfo!.Project.Host);
            Assert.Equal(@namespace, commitInfo.Project.Namespace);
            Assert.Equal(projectName, commitInfo.Project.Project);
            Assert.Equal(commitId, commitInfo.CommitId);
        }
    }
}
