using System;
using System.Diagnostics.CodeAnalysis;

namespace Grynwald.SemanticUrlParser.GitHub
{
    /// <summary>
    /// Encapsulates information about a GitHub Pull Request.
    /// </summary>
    public sealed class GitHubPullRequestInfo : IEquatable<GitHubPullRequestInfo>
    {
        /// <summary>
        /// Gets the GitHub project the pull request belongs to.
        /// </summary>
        public GitHubProjectInfo Project { get; }

        /// <summary>
        /// Gets the PR's number
        /// </summary>
        public int Number { get; }


        /// <summary>
        /// Initializes a new instance of <see cref="GitHubPullRequestInfo"/>
        /// </summary>
        /// <param name="project">The GitHub project the pull request belongs to.</param>
        /// <param name="number">The PR's number.</param>
        public GitHubPullRequestInfo(GitHubProjectInfo project, int number)
        {
            if (number <= 0)
                throw new ArgumentOutOfRangeException(nameof(number));

            Project = project ?? throw new ArgumentNullException(nameof(project));
            Number = number;
        }


        /// <inheritdoc />
        public override bool Equals(object? obj) => Equals(obj as GitHubPullRequestInfo);

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
        public bool Equals([AllowNull] GitHubPullRequestInfo other) =>
            other != null &&
            Project.Equals(other.Project) &&
            Number == other.Number;
    }
}
