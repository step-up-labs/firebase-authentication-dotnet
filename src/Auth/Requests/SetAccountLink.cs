namespace Firebase.Auth.Requests
{
    public class SetAccountLinkRequest : SetAccountInfoRequest
    {
        public string Email { get; set; }

        public string Password { get; set; }
    }

    public class SetAccountLinkResponse : SetAccountInfoResponse
    {
        public string IdToken { get; set; }

        public string RefreshToken { get; set; }

        public int ExpiresIn { get; set; }
    }

    /// <summary>
    /// Link two accounts.
    /// </summary>
    public class SetAccountLink : FirebaseRequestBase<SetAccountLinkRequest, SetAccountLinkResponse>
    {
        public SetAccountLink(FirebaseAuthConfig config) : base(config)
        {
        }

        protected override string UrlFormat => Endpoints.GoogleSetAccountUrl;
    }
}
