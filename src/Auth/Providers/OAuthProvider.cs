using Firebase.Auth.Requests;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Firebase.Auth.Providers
{
    public abstract class OAuthProvider : FirebaseAuthProvider
    {
        protected VerifyAssertion verifyAssertion;
        protected readonly List<string> scopes;
        protected readonly Dictionary<string, string> parameters;

        public OAuthProvider()
        {
            this.scopes = new List<string>();
            this.parameters = new Dictionary<string, string>();
        }

        protected static AuthCredential GetCredential(FirebaseProviderType providerType, string accessToken)
        {
            return new AuthCredential
            {
                ProviderType = providerType,
                Object = new OAuthCredential
                {
                    Token = accessToken
                }
            };
        }

        internal override void Initialize(FirebaseAuthConfig config)
        {
            base.Initialize(config);
            this.verifyAssertion = new VerifyAssertion(config);
        }

        public virtual FirebaseAuthProvider AddScopes(params string[] scopes)
        {
            this.scopes.AddRange(scopes);

            return this;
        }

        public virtual FirebaseAuthProvider AddCustomParameters(params KeyValuePair<string, string>[] parameters)
        {
            parameters.ToList().ForEach(p => this.parameters.Add(p.Key, p.Value));

            return this;
        }

        internal virtual async Task<OAuthContinuation> SignInAsync()
        {
            if (this.LocaleParameterName != null && !this.parameters.ContainsKey(this.LocaleParameterName))
            {
                this.parameters[this.LocaleParameterName] = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            }

            var request = new CreateAuthUriRequest
            {
                ContinueUri = this.GetContinueUri(),
                ProviderId = this.ProviderType,
                CustomParameters = this.parameters,
                OauthScope = this.GetParsedOauthScopes(),
            };

            var response = await this.createAuthUri.ExecuteAsync(request).ConfigureAwait(false);

            return new OAuthContinuation(this.config, response.AuthUri, response.SessionId, this.ProviderType);
        }

        protected internal override Task<User> SignInWithCredentialAsync(AuthCredential credential)
        {
            var c = (OAuthCredential)credential.Object;
            return this.verifyAssertion.ExecuteWithUserAsync(credential.ProviderType, new VerifyAssertionRequest
            {
                RequestUri = $"https://{this.config.AuthDomain}",
                PostBody = $"access_token={c.Token}&providerId={credential.ProviderType.ToEnumString()}",
                ReturnIdpCredential = true,
                ReturnSecureToken = true
            });
        }

        protected virtual string LocaleParameterName => null;

        protected string GetParsedOauthScopes()
        {
            if (!this.scopes.Any())
            {
                return null;
            }

            return $"{{ \"{this.ProviderType.ToEnumString()}\": \"{string.Join(",", this.scopes)}\" }}";
        }

        internal class OAuthCredential
        {
            public string Token { get; set; }
        }
    }
}
