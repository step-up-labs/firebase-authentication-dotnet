using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Firebase.Auth.Requests
{
    public class VerifyAssertionRequest : IdTokenRequest
    {
        public string RequestUri { get; set; }
        
        public string PostBody { get; set; }

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
        
        public int OauthExpireIn { get; set; }
        
        public string RefreshToken { get; set; }
        
        public int ExpiresIn { get; set; }

        public string OauthIdToken { get; set; }

        public bool NeedConfirmation { get; set; }

        [JsonProperty("verifiedProvider")]
        public FirebaseProviderType[] VerifiedProviders { get; set; }
    }

    /// <summary>
    /// Finishes oauth authentication processing.
    /// </summary>
    public class VerifyAssertion : FirebaseRequestBase<VerifyAssertionRequest, VerifyAssertionResponse>
    {
        private readonly GetAccountInfo accountInfo;

        public VerifyAssertion(FirebaseAuthConfig config) : base(config)
        {
            this.accountInfo = new GetAccountInfo(config);
        }

        public override async Task<VerifyAssertionResponse> ExecuteAsync(VerifyAssertionRequest request)
        {
            var result = await base.ExecuteAsync(request).ConfigureAwait(false);

            if (result.NeedConfirmation)
            {
                throw new FirebaseAuthLinkConflictException(
                    result.Email,
                    result.VerifiedProviders);
            }

            return result;
        }

        public async Task<User> ExecuteWithUserAsync(FirebaseProviderType providerType, VerifyAssertionRequest request)
        {
            var assertion = await this.ExecuteAsync(request).ConfigureAwait(false);
            
            var accountInfo = await this.accountInfo.ExecuteAsync(new IdTokenRequest
            {
                IdToken = assertion.IdToken
            }).ConfigureAwait(false);

            var u = accountInfo.Users[0];
            var userInfo = new UserInfo
            {
                DisplayName = u.DisplayName ?? assertion.DisplayName,
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
                ProviderType = providerType
            };

            return new User(this.config, userInfo, token);
        }

        protected override string UrlFormat => Endpoints.GoogleIdentityUrl;
    }
}
