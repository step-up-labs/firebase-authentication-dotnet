using Firebase.Auth.Requests;
using System.Threading.Tasks;

namespace Firebase.Auth.Providers
{
    public class OAuthContinuation
    {
        private readonly FirebaseAuthConfig config;
        private readonly VerifyAssertion verifyAssertion;
        private readonly GetAccountInfo accountInfo;
        private readonly string sessionId;
        private readonly FirebaseProviderType providerType;

        internal OAuthContinuation(FirebaseAuthConfig config, string uri, string sessionId, FirebaseProviderType providerType)
        {
            this.config = config;
            this.verifyAssertion = new VerifyAssertion(config);
            this.accountInfo = new GetAccountInfo(config);
            this.Uri = uri;
            this.sessionId = sessionId;
            this.providerType = providerType;
        }

        public string Uri { get; }

        public async Task<User> ContinueSignInAsync(string redirectUri)
        {
            var assertion = await this.verifyAssertion.ExecuteAsync(new VerifyAssertionRequest
            {
                RequestUri = redirectUri,
                SessionId = this.sessionId,
                ReturnIdpCredential = true,
                ReturnSecureToken = true
            }).ConfigureAwait(false);

            var accountInfo = await this.accountInfo.ExecuteAsync(new IdTokenRequest
            {
                IdToken = assertion.IdToken
            }).ConfigureAwait(false);

            var u = accountInfo.Users[0];
            var userInfo = new UserInfo
            {
                DisplayName = u.DisplayName,
                FirstName = assertion.FirstName,
                LastName = assertion.LastName,
                Email = u.Email ?? assertion.Email,
                IsEmailVerified = u.EmailVerified,
                FederatedId = assertion.FederatedId,
                Uid = u.LocalId,
                PhotoUrl = assertion.PhotoUrl,
                IsAnonymous = false
            };

            var token = new FirebaseCredential
            {
                ExpiresIn = assertion.ExpiresIn,
                RefreshToken = assertion.RefreshToken,
                IdToken = assertion.IdToken,
                ProviderType = this.providerType
            };

            return new User(this.config, userInfo, token);
        }
    }
}
