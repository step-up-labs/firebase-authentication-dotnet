namespace Firebase.Auth.Requests
{
    public class VerifyAssertionRequest
    {
        public string RequestUri { get; set; }

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
    }

    public class VerifyAssertion : FirebaseRequestBase<VerifyAssertionRequest, VerifyAssertionResponse>
    {
        public VerifyAssertion(FirebaseAuthConfig config) : base(config)
        {
        }

        protected override string UrlFormat => Endpoints.GoogleIdentityUrl;
    }
}
