using Firebase.Auth.Providers;
using System;
using System.Threading.Tasks;

namespace Firebase.Auth.UI
{
    /// <summary>
    /// Configuration options for FirebaseUI. Extends <see cref="FirebaseAuthConfig"/>.
    /// </summary>
    public class FirebaseUIConfig : FirebaseAuthConfig
    {
        /// <summary>
        /// Url pointing to your ToS.
        /// </summary>
        public string TermsOfServiceUrl { get; set; }

        /// <summary>
        /// Url pointing to your privacy policy.
        /// </summary>
        public string PrivacyPolicyUrl { get; set; }

        /// <summary>
        /// Specifies whether anonymous signin is allowed.
        /// </summary>
        public bool IsAnonymousAllowed { get; set; }

        /// <summary>
        /// Specifies whether signed in anonymous users should be linked to newly created users.
        /// </summary>
        public bool AutoUpgradeAnonymousUsers { get; set; }

        public Func<FirebaseUpgradeConflict, Task<UserCredential>> AnonymousUpgradeConflict;

        internal Task<UserCredential> RaiseUpgradeConflictAsync(FirebaseAuthClient client, AuthCredential credential)
        {
            if (this.AnonymousUpgradeConflict == null)
            {
                new InvalidOperationException($"{nameof(AnonymousUpgradeConflict)} must be set when {nameof(AutoUpgradeAnonymousUsers)} is set");
            }

            return this.AnonymousUpgradeConflict(new FirebaseUpgradeConflict(client, credential));
        }
    }
}
