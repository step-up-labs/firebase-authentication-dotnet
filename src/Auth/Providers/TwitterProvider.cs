using Firebase.Auth.Requests;

namespace Firebase.Auth.Providers
{
    public class TwitterProvider : OAuthProvider
    {
        public static AuthCredential GetCredential(string token, string secret)
        {
            return new TwitterCredential
            {
                ProviderType = FirebaseProviderType.Twitter,
                Token = token,
                Secret = secret
            };
        }

        public override FirebaseProviderType ProviderType => FirebaseProviderType.Twitter;

        protected override string LocaleParameterName => "lang";

        internal override AuthCredential GetCredential(VerifyAssertionResponse response)
        {
            return GetCredential(response.OauthAccessToken, response.OauthTokenSecret);
        }

        internal class TwitterCredential : OAuthCredential
        {
            public string Secret { get; set; }

            internal override string GetPostBodyValue(FirebaseProviderType ProviderType)
            {
                var value = base.GetPostBodyValue(ProviderType);

                if (value == null)
                {
                    return value;
                }

                return $"{value}&oauth_token_secret={this.Secret}";
            }
        }
    }
}
