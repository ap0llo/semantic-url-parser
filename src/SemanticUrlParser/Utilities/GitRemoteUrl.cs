using System;
using System.Diagnostics.CodeAnalysis;

namespace Grynwald.SemanticUrlParser.Utilities
{
    internal static class GitRemoteUrl
    {
        private static readonly char[] s_ScpUrlSplitChars = new[] { ':' };

        /// <summary>
        /// Attemps to convert a git remote url into a <see cref="Uri"/>
        /// </summary>
        /// <remarks>
        /// <para>
        /// Supports both HTTP urls and SSH urls in the SCP-format e.g. <c>git@github.com:ap0llo/semantic-url-parser.git</c>
        /// </para>
        /// <para>
        /// Urls in the SCP format are converted to equivalent <c>ssh://</c> uris.
        /// </para>
        /// </remarks>
        /// <param name="url">The remote url to parse.</param>
        /// <param name="uri">When succesful, contains the parsed url as <see cref="Uri"/>.</param>
        /// <returns>
        /// Returns whether the url could be converted into a <see cref="Uri"/>
        /// </returns>
        public static bool TryGetUri(string url, [NotNullWhen(true)] out Uri? uri)
        {
            if (Uri.TryCreate(url, UriKind.Absolute, out uri))
            {
                return true;
            }
            else
            {
                return TryParseScpUrl(url, out uri);
            }
        }


        private static bool TryParseScpUrl(string url, [NotNullWhen(true)] out Uri? sshUri)
        {
            // Parse a scp-format git url: e.g. git@github.com:ap0llo/semantic-url-parser.git

            var fragments = url.Split(s_ScpUrlSplitChars, StringSplitOptions.RemoveEmptyEntries);
            if (fragments.Length != 2)
            {
                sshUri = default;
                return false;
            }

            var userNameAndHost = fragments[0];
            var path = fragments[1].TrimStart('/');

            return Uri.TryCreate($"ssh://{userNameAndHost}/{path}", UriKind.Absolute, out sshUri);
        }
    }
}
