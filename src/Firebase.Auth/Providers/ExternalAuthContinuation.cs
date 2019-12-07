using Firebase.Auth.Requests;
using System.Threading.Tasks;

namespace Firebase.Auth.Providers
{
    public class ExternalAuthContinuation
    {
        private readonly VerifyAssertion verifyAssertion;
        private readonly GetAccountInfo accountInfo;
        private readonly string sessionId;

        internal ExternalAuthContinuation(VerifyAssertion verifyAssertion, GetAccountInfo accountInfo, string uri, string sessionId)
        {
            this.verifyAssertion = verifyAssertion;
            this.Uri = uri;
            this.sessionId = sessionId;
            this.accountInfo = accountInfo;
        }

        public string Uri { get; }

        public async Task<(User, FirebaseAuthToken)> ContinueSignInAsync(string redirectUri)
        {
            var assertion = await this.verifyAssertion.ExecuteAsync(new VerifyAssertionRequest
            {
                RequestUri = redirectUri,
                SessionId = this.sessionId,
                ReturnIdpCredential = true,
                ReturnSecureToken = true
            }).ConfigureAwait(false);

            var accountInfo = await this.accountInfo.ExecuteAsync(new GetAccountInfoRequest
            {
                IdToken = assertion.IdToken
            }).ConfigureAwait(false);

            var u = accountInfo.Users[0];
            var user = new User
            {
                DisplayName = u.DisplayName,
                FirstName = assertion.FirstName,
                LastName = assertion.LastName,
                Email = u.Email ?? assertion.Email,
                IsEmailVerified = u.EmailVerified,
                FederatedId = assertion.FederatedId,
                LocalId = u.LocalId,
                PhotoUrl = u.PhotoUrl
            };

            var token = new FirebaseAuthToken
            {
                ExpiresIn = assertion.ExpiresIn,
                RefreshToken = assertion.RefreshToken,
                IdToken = assertion.IdToken
            };

            return (user, token);
        }
    }
}
