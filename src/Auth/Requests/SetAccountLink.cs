using System.Collections.Generic;

namespace Firebase.Auth.Requests
{
    public class SetAccountLinkRequest : Dictionary<string, object>
    {
        public SetAccountLinkRequest()
        {
        }

        public SetAccountLinkRequest(string idToken, bool returnSecureToken = true)
        {
            this[nameof(idToken)] = idToken;
            this[nameof(returnSecureToken)] = returnSecureToken;
        }
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
