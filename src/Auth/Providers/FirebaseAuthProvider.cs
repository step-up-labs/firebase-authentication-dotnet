using Firebase.Auth.Requests;

namespace Firebase.Auth.Providers
{
    /// <summary>
    /// Base class for Firebase auth providers.
    /// </summary>
    public abstract class FirebaseAuthProvider
    {
        protected FirebaseAuthConfig config;
        protected CreateAuthUri createAuthUri;

        public abstract FirebaseProviderType ProviderType { get; }

        internal virtual void Initialize(FirebaseAuthConfig config)
        {
            this.config = config;
            this.createAuthUri = new CreateAuthUri(config);
        }

        protected string GetContinueUri()
        {
            return $"https://{this.config.AuthDomain}/__/auth/handler";
        }
    }
}
