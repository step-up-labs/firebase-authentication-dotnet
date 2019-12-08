namespace Firebase.Auth.Requests
{
    public class SignupNewUserRequest
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public bool ReturnSecureToken { get; set; }
    }

    public class SignupNewUserResponse
    {
        public string IdToken { get; set; }

        public string Email { get; set; }

        public string RefreshToken { get; set; }

        public int ExpiresIn { get; set; }

        public string LocalId { get; set; }
    }

    /// <summary>
    /// Creates a new user account.
    /// </summary>
    public class SignupNewUser : FirebaseRequestBase<SignupNewUserRequest, SignupNewUserResponse>
    {
        public SignupNewUser(FirebaseAuthConfig config) : base(config)
        {
        }

        protected override string UrlFormat => Endpoints.GoogleSignUpUrl;
    }
}
