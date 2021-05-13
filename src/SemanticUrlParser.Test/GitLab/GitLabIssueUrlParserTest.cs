using System;
using System.Collections.Generic;
using Grynwald.SemanticUrlParser.GitLab;
using Xunit;

namespace Grynwald.SemanticUrlParser.Test.GitLab
{
    /// <summary>
    /// Tests for <see cref="GitLabIssueUrlParser"/>
    /// </summary>
    public class GitLabIssueUrlParserTest : UrlParserTest<GitLabIssueInfo>
    {

        protected override GitLabUrlParser<GitLabIssueInfo> CreateInstance() => new GitLabIssueUrlParser();

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

            // commit url
            yield return TestCase("https://gitlab.com/namespace/project/-/commit/1");

            // Missing or misplaced '-'
            yield return TestCase("https://gitlab.com/namespace/project/issues/1");
            yield return TestCase("https://gitlab.com/namespace/-/project/issues/1");

            // too many segments
            yield return TestCase("https://gitlab.com/namespace/project/-/issues/1/some-value");

            // too few segments
            yield return TestCase("https://gitlab.com/namespace/project/-/1");
            yield return TestCase("https://gitlab.com/namespace/project/-/issues");
            yield return TestCase("https://gitlab.com/project/-/issues/1");

            // empty or whitespace namesapce
            yield return TestCase("https://gitlab.com//project/-/issues/1");
            yield return TestCase("https://gitlab.com/ /project/-/issues/1");

            // empty or whitespace project name
            yield return TestCase("https://gitlab.com/namespace//-/issues/1");
            yield return TestCase("https://gitlab.com/namespace/ /-/issues/1");

            // empty or whitespace issue id
            yield return TestCase("https://gitlab.com/namespace/project/-/issues/ /");

            // negative id
            yield return TestCase("https://gitlab.com/namespace/project/-/issues/-1");
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
            static object?[] TestCase(string url, string host, string @namespace, string projectName, int number)
            {
                return new object?[] { url, host, @namespace, projectName, number };
            }

            yield return TestCase("https://gitlab.com/user/project/-/issues/23", "gitlab.com", "user", "project", 23);
            yield return TestCase("https://gitlab.com/group/subgroup/project/-/issues/42", "gitlab.com", "group/subgroup", "project", 42);

            // Trailing slashes are ignored
            yield return TestCase("https://gitlab.com/group/subgroup/project/-/issues/42/", "gitlab.com", "group/subgroup", "project", 42);

        }


        [Theory]
        [MemberData(nameof(PositiveTestCases))]
        public void ParseUrl_returns_the_expected_GitLabCommitInfo(string url, string host, string @namespace, string projectName, int issueNumber)
        {
            // ARRANGE
            var sut = CreateInstance();

            // ACT 
            var issueInfo = sut.ParseUrl(url);

            // ASSERT
            Assert.NotNull(issueInfo);
            Assert.Equal(host, issueInfo.Project.Host);
            Assert.Equal(@namespace, issueInfo.Project.Namespace);
            Assert.Equal(projectName, issueInfo.Project.Project);
            Assert.Equal(issueNumber, issueInfo.Number);
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
        public void TryParseUrl_returns_the_expected_GitLabCommitInfo(string url, string host, string @namespace, string projectName, int issueNumber)
        {
            // ARRANGE
            var sut = CreateInstance();

            // ACT 
            var success = sut.TryParseUrl(url, out var issueInfo);

            // ASSERT
            Assert.True(success);
            Assert.NotNull(issueInfo);
            Assert.Equal(host, issueInfo!.Project.Host);
            Assert.Equal(@namespace, issueInfo.Project.Namespace);
            Assert.Equal(projectName, issueInfo.Project.Project);
            Assert.Equal(issueNumber, issueInfo.Number);
        }
    }
}
