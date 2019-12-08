namespace Firebase.Auth.Requests
{
    public class VerifyPasswordRequest
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public bool ReturnSecureToken { get; set; }
    }

    public class VerifyPasswordResponse
    {
        public string LocalId { get; set; }
     
        public string Email { get; set; }
        
        public string DisplayName { get; set; }
        
        public string IdToken { get; set; }
        
        public bool Registered { get; set; }
        
        public string RefreshToken { get; set; }
        
        public int ExpiresIn { get; set; }
    }

    /// <summary>
    /// Verifies specified password matches the user's actual password.
    /// </summary>
    public class VerifyPassword : FirebaseRequestBase<VerifyPasswordRequest, VerifyPasswordResponse>
    {
        public VerifyPassword(FirebaseAuthConfig config) : base(config)
        {
        }

        protected override string UrlFormat => Endpoints.GooglePasswordUrl;
    }
}
