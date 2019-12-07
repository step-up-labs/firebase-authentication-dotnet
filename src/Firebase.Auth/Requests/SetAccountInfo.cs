namespace Firebase.Auth.Requests
{
    public class SetAccountInfoRequest
    {
        public string IdToken { get; set; }

        public string DisplayName { get; set; }

        public bool ReturnSecureToken { get; set; }
    }

    public class SetAccountInfoResponse
    {
        public string LocalId { get; set; }
        
        public string Email { get; set; }
        
        public string DisplayName { get; set; }
        
        public ProviderUserInfo[] ProviderUserInfo { get; set; }
        
        public string PasswordHash { get; set; }

        public bool EmailVerified { get; set; }
    }

    public class SetAccountInfo : FirebaseRequestBase<SetAccountInfoRequest, SetAccountInfoResponse>
    {
        public SetAccountInfo(FirebaseAuthConfig config) : base(config)
        {
        }

        protected override string UrlFormat => Endpoints.GoogleSetAccountUrl;
    }
}
