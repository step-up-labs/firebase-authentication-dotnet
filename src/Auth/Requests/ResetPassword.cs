namespace Firebase.Auth.Requests
{
    public class ResetPasswordRequest
    {
        public const string PasswordResetType = "PASSWORD_RESET";

        public ResetPasswordRequest()
        {
            this.RequestType = PasswordResetType;
        }

        public string Email { get; set; }

        public string RequestType { get; set; }
    }

    public class ResetPasswordResponse
    {
        public string Email { get; set; }
    }

    /// <summary>
    /// Resets user's password for given email by sending a reset email.
    /// </summary>
    public class ResetPassword : FirebaseRequestBase<ResetPasswordRequest, ResetPasswordResponse>
    {
        public ResetPassword(FirebaseAuthConfig config) : base(config)
        {
        }

        protected override string UrlFormat => Endpoints.GoogleGetConfirmationCodeUrl;
    }
}
