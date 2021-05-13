using System;
using System.Diagnostics.CodeAnalysis;

namespace Grynwald.SemanticUrlParser.GitHub
{
    //TODO: Rename to GitHubRepositoryInfo??
    /// <summary>
    /// Encapsulates information about a GitHub project
    /// </summary>
    public sealed class GitHubProjectInfo : IEquatable<GitHubProjectInfo>
    {
        /// <summary>
        /// The GitHub server's host name (typically <c>github.com</c>, but might be different for GitHub Enterprise installations).
        /// </summary>
        public string Host { get; }

        /// <summary>
        /// The owner (user or group) of the repository.
        /// </summary>
        public string Owner { get; }

        /// <summary>
        /// The name of the GitHub repository.
        /// </summary>
        public string Repository { get; }


        /// <summary>
        /// Initializes a new instance of <see cref="GitHubProjectInfo"/>
        /// </summary>
        /// <param name="host">The GitHub server's host name (typically <c>github.com</c>, but might be different for GitHub Enterprise installations).</param>
        /// <param name="owner">The owner (user or group) of the repository.</param>
        /// <param name="repository">The name of the GitHub repository.</param>
        public GitHubProjectInfo(string host, string owner, string repository)
        {
            if (String.IsNullOrWhiteSpace(host))
                throw new ArgumentException("Value must not be null or whitespace", nameof(host));

            if (String.IsNullOrWhiteSpace(owner))
                throw new ArgumentException("Value must not be null or whitespace", nameof(owner));

            if (String.IsNullOrWhiteSpace(repository))
                throw new ArgumentException("Value must not be null or whitespace", nameof(repository));

            Host = host;
            Owner = owner;
            Repository = repository;
        }


        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hash = StringComparer.OrdinalIgnoreCase.GetHashCode(Host) * 397;
                hash ^= StringComparer.OrdinalIgnoreCase.GetHashCode(Owner);
                hash ^= StringComparer.OrdinalIgnoreCase.GetHashCode(Repository);
                return hash;
            }
        }

        /// <inheritdoc />
        public override bool Equals(object? obj) => Equals(obj as GitHubProjectInfo);

        /// <inheritdoc />
        public bool Equals([AllowNull] GitHubProjectInfo other)
        {
            return other != null &&
                StringComparer.OrdinalIgnoreCase.Equals(Host, other.Host) &&
                StringComparer.OrdinalIgnoreCase.Equals(Owner, other.Owner) &&
                StringComparer.OrdinalIgnoreCase.Equals(Repository, other.Repository);
        }
    }
}
