using Firebase.Auth.Requests;
using System;
using System.Threading.Tasks;

namespace Firebase.Auth.Providers
{
    public class OAuthContinuation
    {
        private readonly VerifyAssertion verifyAssertion;
        private readonly FirebaseAuthConfig config;
        private readonly string sessionId;
        private readonly FirebaseProviderType providerType;

        internal OAuthContinuation(FirebaseAuthConfig config, string uri, string sessionId, FirebaseProviderType providerType)
        {
            this.verifyAssertion = new VerifyAssertion(config);
            this.config = config;
            this.Uri = uri;
            this.sessionId = sessionId;
            this.providerType = providerType;
        }

        public string Uri { get; }

        public Task<UserCredential> ContinueSignInAsync(string redirectUri)
        {
            return this.verifyAssertion.ExecuteWithUserAsync(
                this.providerType, 
                new VerifyAssertionRequest
                {
                    RequestUri = redirectUri,
                    SessionId = this.sessionId,
                    ReturnIdpCredential = true,
                    ReturnSecureToken = true
                }, 
                (user, response) =>
                {
                    var provider = this.config.GetAuthProvider(this.providerType) as OAuthProvider ?? throw new InvalidOperationException($"{this.providerType} is not a OAuthProvider");
                    return new UserCredential(user, provider.GetCredential(response), OperationType.SignIn);
                });
        }
    }
}
