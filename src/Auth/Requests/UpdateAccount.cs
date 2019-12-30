namespace Firebase.Auth.Requests
{
    public class UpdateAccountRequest : IdTokenRequest
    {
        public string Password { get; set; }
        
        public bool ReturnSecureToken { get; set; }
    }

    public class UpdateAccountResponse
    {
        public string LocalId { get; set; }

        public string Email { get; set; }
        
        public string PasswordHash { get; set; }
        
        public string IdToken { get; set; }

        public string RefreshToken { get; set; }

        public int ExpiresIn { get; set; }
    }

    public class UpdateAccount : FirebaseRequestBase<UpdateAccountRequest, UpdateAccountResponse>
    {
        public UpdateAccount(FirebaseAuthConfig config) : base(config)
        {
        }

        protected override string UrlFormat => Endpoints.GoogleUpdateUserPassword;
    }
}
