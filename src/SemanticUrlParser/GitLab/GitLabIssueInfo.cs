using System;
using System.Diagnostics.CodeAnalysis;

namespace Grynwald.SemanticUrlParser.GitLab
{
    /// <summary>
    /// Encapsulates information about a GitLab issue.
    /// </summary>
    public sealed class GitLabIssueInfo : IEquatable<GitLabIssueInfo>
    {
        /// <summary>
        /// Gets the GitLab project the issue belongs to.
        /// </summary>
        public GitLabProjectInfo Project { get; }

        /// <summary>
        /// Gets the issue's number
        /// </summary>
        public int Number { get; }


        /// <summary>
        /// Initializes a new instance of <see cref="GitLabIssueInfo"/>
        /// </summary>
        /// <param name="project">The GitLab project the issue belongs to.</param>
        /// <param name="number">The issue's number.</param>
        public GitLabIssueInfo(GitLabProjectInfo project, int number)
        {
            if (number <= 0)
                throw new ArgumentOutOfRangeException(nameof(number));

            Project = project ?? throw new ArgumentNullException(nameof(project));
            Number = number;
        }


        /// <inheritdoc />
        public override bool Equals(object? obj) => Equals(obj as GitLabIssueInfo);

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hash = Project.GetHashCode() * 397;
                hash ^= Number.GetHashCode();
                return hash;
            }
        }

        /// <inheritdoc />
        public bool Equals([AllowNull] GitLabIssueInfo other) =>
            other != null &&
            Project.Equals(other.Project) &&
            Number == other.Number;
    }
}
