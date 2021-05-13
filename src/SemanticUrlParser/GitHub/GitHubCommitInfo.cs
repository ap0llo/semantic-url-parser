using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Grynwald.SemanticUrlParser.GitHub
{
    /// <summary>
    /// Encapsulates information about a git commit on GitHub
    /// </summary>
    public sealed class GitHubCommitInfo : IEquatable<GitHubCommitInfo>
    {
        /// <summary>
        /// Gets the GitHub project the pull request belongs to.
        /// </summary>
        public GitHubProjectInfo Project { get; }

        /// <summary>
        /// Gets the git commit SHA
        /// </summary>
        public string CommitId { get; }


        /// <summary>
        /// Initializes a new instance of <see cref="GitHubCommitInfo"/>
        /// </summary>
        /// <param name="project">The GitHub project the pull request belongs to.</param>
        /// <param name="number">The commit's SHA</param>
        public GitHubCommitInfo(GitHubProjectInfo project, string commitId)
        {
            if (String.IsNullOrWhiteSpace(commitId))
                throw new ArgumentException("Value must not be null or whitespace", nameof(commitId));

            Project = project ?? throw new ArgumentNullException(nameof(project));
            CommitId = commitId;
        }


        /// <inheritdoc />
        public override bool Equals(object? obj) => Equals(obj as GitHubCommitInfo);

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hash = Project.GetHashCode() * 397;
                hash ^= StringComparer.OrdinalIgnoreCase.GetHashCode(CommitId);
                return hash;
            }
        }

        /// <inheritdoc />
        public bool Equals([AllowNull] GitHubCommitInfo other) =>
            other != null &&
            Project.Equals(other.Project) &&
            StringComparer.OrdinalIgnoreCase.Equals(CommitId, other.CommitId);
    }
}
