using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Grynwald.SemanticUrlParser.GitHub
{
    /// <summary>
    /// Encapsulates information about a GitHub issue.
    /// </summary>
    public sealed class GitHubIssueInfo : IEquatable<GitHubIssueInfo>
    {
        /// <summary>
        /// Gets the GitHub project the issue belongs to.
        /// </summary>
        public GitHubProjectInfo Project { get; }

        /// <summary>
        /// Gets the issue's number
        /// </summary>
        public int Number { get; }


        /// <summary>
        /// Initializes a new instance of <see cref="GitHubIssueInfo"/>
        /// </summary>
        /// <param name="project">The GitHub project the issue belongs to.</param>
        /// <param name="number">The issue's number.</param>
        public GitHubIssueInfo(GitHubProjectInfo project, int number)
        {
            if (number <= 0)
                throw new ArgumentOutOfRangeException(nameof(number));

            Project = project ?? throw new ArgumentNullException(nameof(project));
            Number = number;
        }


        /// <inheritdoc />
        public override bool Equals(object obj) => Equals(obj as GitHubIssueInfo);

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
        public bool Equals([AllowNull] GitHubIssueInfo other) =>
            other != null &&
            Project.Equals(other.Project) &&
            Number == other.Number;
    }
}
