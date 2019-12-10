using Firebase.Auth.Providers;

namespace Firebase.Auth.UI
{
    public class FirebaseAuthConfigUI : FirebaseAuthConfig
    {
        public FirebaseAuthConfigUI()
        {
        }

        public FirebaseAuthConfigUI(string apiKey, string authDomain, IFirebaseTokenRepository tokenRepository, FirebaseAuthProvider[] providers, string termsOfServiceUrl, string privacyPolicyUrl) 
            : base(apiKey, authDomain, tokenRepository, providers)
        {
            this.TermsOfServiceUrl = termsOfServiceUrl;
            this.PrivacyPolicyUrl = privacyPolicyUrl;
        }

        /// <summary>
        /// Url pointing to your ToS.
        /// </summary>
        public string TermsOfServiceUrl { get; set; }

        /// <summary>
        /// Url pointing to your privacy policy.
        /// </summary>
        public string PrivacyPolicyUrl { get; set; }
    }
}
