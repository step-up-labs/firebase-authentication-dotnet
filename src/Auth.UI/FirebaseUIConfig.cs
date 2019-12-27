using Firebase.Auth.Providers;

namespace Firebase.Auth.UI
{
    public class FirebaseUIConfig : FirebaseAuthConfig
    {
        public FirebaseUIConfig()
        {
        }

        public FirebaseUIConfig(string apiKey, string authDomain, IFirebaseTokenRepository tokenRepository, FirebaseAuthProvider[] providers, string termsOfServiceUrl, string privacyPolicyUrl, bool allowAnonymous = false) 
            : base(apiKey, authDomain, tokenRepository, providers)
        {
            this.TermsOfServiceUrl = termsOfServiceUrl;
            this.PrivacyPolicyUrl = privacyPolicyUrl;
            this.IsAnonymousAllowed = allowAnonymous;
        }

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
