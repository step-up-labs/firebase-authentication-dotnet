using Firebase.Auth.Requests;
using System;
using System.Threading.Tasks;

namespace Firebase.Auth.Providers
{
    public class TwitterProvider : OAuthProvider
    {
        public static AuthCredential GetCredential(string token, string secret)
        {
            return new AuthCredential
            {
                ProviderType = FirebaseProviderType.Twitter,
                Object = new TwitterCredential
                {
                    Token = token,
                    Secret = secret
                }
            };
        }

        public override FirebaseProviderType ProviderType => FirebaseProviderType.Twitter;

        protected override string LocaleParameterName => "lang";

        protected internal override Task<User> SignInWithCredentialAsync(AuthCredential credential)
        {
            var c = (TwitterCredential)credential.Object;
            return this.verifyAssertion.ExecuteWithUserAsync(credential.ProviderType, new VerifyAssertionRequest
            {
                RequestUri = $"https://{this.config.AuthDomain}",
                PostBody = $"access_token={c.Token}&oauth_token_secret={c.Secret}&providerId=twitter.com",
                ReturnIdpCredential = true,
                ReturnSecureToken = true
            });
        }

        internal class TwitterCredential : OAuthCredential
        {
            public string Secret { get; set; }
        }
    }
}
