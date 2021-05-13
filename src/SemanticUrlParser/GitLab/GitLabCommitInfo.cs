using System;

namespace Grynwald.SemanticUrlParser.GitLab
{
    public sealed class GitLabCommitInfo : IEquatable<GitLabCommitInfo>
    {
        /// <summary>
        /// Gets the GitLab project the commitbelongs to.
        /// </summary>
        public GitLabProjectInfo Project { get; }

        /// <summary>
        /// Gets the git commit SHA
        /// </summary>
        public string CommitId { get; }


        /// <summary>
        /// Initializes a new instance of <see cref="GitLabCommitInfo"/>
        /// </summary>
        /// <param name="project">The GitLab project the pull request belongs to.</param>
        /// <param name="number">The commit's SHA</param>
        public GitLabCommitInfo(GitLabProjectInfo project, string commitId)
        {
            if (String.IsNullOrWhiteSpace(commitId))
                throw new ArgumentException("Value must not be null or whitespace", nameof(commitId));

            Project = project ?? throw new ArgumentNullException(nameof(project));
            CommitId = commitId;
        }


        /// <inheritdoc />
        public override bool Equals(object? obj) => Equals(obj as GitLabCommitInfo);

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
        public bool Equals(GitLabCommitInfo? other) =>
            other != null &&
            Project.Equals(other.Project) &&
            StringComparer.OrdinalIgnoreCase.Equals(CommitId, other.CommitId);
    }

}
