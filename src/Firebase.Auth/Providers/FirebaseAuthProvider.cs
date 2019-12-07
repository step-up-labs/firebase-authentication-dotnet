using Firebase.Auth.Requests;
using System.Linq;

namespace Firebase.Auth.Providers
{
    /// <summary>
    /// Base class for Firebase auth providers.
    /// </summary>
    public abstract class FirebaseAuthProvider
    {
        protected FirebaseAuthConfig config;
        protected CreateAuthUri createAuthUri;
        protected GetAccountInfo accountInfo;

        public abstract FirebaseProviderType AuthType { get; }

        internal virtual void Initialize(FirebaseAuthConfig config)
        {
            this.config = config;
            this.createAuthUri = new CreateAuthUri(config);
            this.accountInfo = new GetAccountInfo(config);
        }

        protected string GetContinueUri()
        {
            return $"https://{this.config.AuthDomain}/__/auth/handler";
        }
    }
}
