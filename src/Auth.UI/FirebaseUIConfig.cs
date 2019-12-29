using Firebase.Auth.Providers;

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
    }
}
