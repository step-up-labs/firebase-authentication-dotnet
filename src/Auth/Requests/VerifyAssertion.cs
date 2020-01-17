using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Firebase.Auth.Requests
{
    public class VerifyAssertionRequest : IdTokenRequest
    {
        public string RequestUri { get; set; }
        
        public string PostBody { get; set; }
        
        public string PendingToken { get; set; }

        public string SessionId { get; set; }

        public bool ReturnIdpCredential { get; set; }

        public bool ReturnSecureToken { get; set; }
    }

    public class VerifyAssertionResponse
    {
        public string FederatedId { get; set; }
        
        public FirebaseProviderType ProviderId { get; set; }
        
        public string Email { get; set; }
        
        public bool EmailVerified { get; set; }
        
        public string FirstName { get; set; }
        
        public string FullName { get; set; }
        
        public string LastName { get; set; }
        
        public string PhotoUrl { get; set; }
        
        public string LocalId { get; set; }
        
        public string DisplayName { get; set; }
        
        public string IdToken { get; set; }
        
        public string Context { get; set; }
        
        public string OauthAccessToken { get; set; }
        
        public string OauthTokenSecret { get; set; }
        
        public int OauthExpireIn { get; set; }
        
        public string RefreshToken { get; set; }
        
        public int ExpiresIn { get; set; }

        public string OauthIdToken { get; set; }

        public string PendingToken { get; set; }

        public bool NeedConfirmation { get; set; }

        [JsonProperty("verifiedProvider")]
        public FirebaseProviderType[] VerifiedProviders { get; set; }

        public string ErrorMessage { get; set; }

        public void Validate(AuthCredential credential) => VerifyAssertion.ValidateAssertionResponse(this, credential);
    }

    /// <summary>
    /// Finishes oauth authentication processing.
    /// </summary>
    public class VerifyAssertion : FirebaseRequestBase<VerifyAssertionRequest, VerifyAssertionResponse>
    {
        public VerifyAssertion(FirebaseAuthConfig config) : base(config)
        {
        }
        
        public static void ValidateAssertionResponse(VerifyAssertionResponse response, AuthCredential credential)
        {
            if (response.NeedConfirmation)
            {
                throw new FirebaseAuthLinkConflictException(
                    response.Email,
                    response.VerifiedProviders);
            }

            if (response.ErrorMessage == "FEDERATED_USER_ID_ALREADY_LINKED")
            {
                throw new FirebaseAuthWithCredentialException("This credential is already associated with a different user account", credential, AuthErrorReason.AlreadyLinked);
            }

            if (response.ErrorMessage == "EMAIL_EXISTS")
            {
                // trying to link OAuth account to email which already exists
                throw new FirebaseAuthWithCredentialException("This email is already associated with another account", response.Email, credential, AuthErrorReason.EmailExists);
            }
        }

        public async Task<(User, VerifyAssertionResponse)> ExecuteAndParseAsync(FirebaseProviderType providerType, VerifyAssertionRequest request)
        {
            var assertion = await this.ExecuteAsync(request).ConfigureAwait(false);

            var userInfo = new UserInfo
            {
                DisplayName = assertion.DisplayName,
                FirstName = assertion.FirstName,
                LastName = assertion.LastName,
                Email = assertion.Email,
                IsEmailVerified = assertion.EmailVerified,
                FederatedId = assertion.FederatedId,
                Uid = assertion.LocalId,
                PhotoUrl = assertion.PhotoUrl,
                IsAnonymous = false
            };

            var token = new FirebaseCredential
            {
                ExpiresIn = assertion.ExpiresIn,
                RefreshToken = assertion.RefreshToken,
                IdToken = assertion.IdToken,
                ProviderType = providerType
            };

            return (new User(this.config, userInfo, token), assertion);
        }

        protected override string UrlFormat => Endpoints.GoogleIdentityUrl;
    }
}
