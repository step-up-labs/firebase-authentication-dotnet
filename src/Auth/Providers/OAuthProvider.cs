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

        protected virtual string LocaleParameterName => null;

        protected static AuthCredential GetCredential(FirebaseProviderType providerType, string accessToken, OAuthCredentialTokenType tokenType)
        {
            return new OAuthCredential
            {
                ProviderType = providerType,
                Token = accessToken,
                TokenType = tokenType
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

        internal virtual AuthCredential GetCredential(VerifyAssertionResponse response)
        {
            return GetCredential(
                this.ProviderType, 
                response.PendingToken ?? response.OauthAccessToken, 
                response.PendingToken == null ? OAuthCredentialTokenType.AccessToken : OAuthCredentialTokenType.PendingToken);
        }

        internal virtual async Task<OAuthContinuation> SignInAsync()
        {
            if (this.LocaleParameterName != null && !this.parameters.ContainsKey(this.LocaleParameterName))
            {
                this.parameters[this.LocaleParameterName] = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            }

            var request = new CreateAuthUriRequest
            {
                ContinueUri = this.config.RedirectUri,
                ProviderId = this.ProviderType,
                CustomParameters = this.parameters,
                OauthScope = this.GetParsedOauthScopes(),
            };

            var response = await this.createAuthUri.ExecuteAsync(request).ConfigureAwait(false);

            return new OAuthContinuation(this.config, response.AuthUri, response.SessionId, this.ProviderType);
        }

        protected internal override async Task<UserCredential> SignInWithCredentialAsync(AuthCredential credential)
        {
            var c = (OAuthCredential)credential;
            var (user, response) = await this.verifyAssertion.ExecuteAndParseAsync(credential.ProviderType, new VerifyAssertionRequest
            {
                RequestUri = $"https://{this.config.AuthDomain}",
                PostBody = c.GetPostBodyValue(credential.ProviderType),
                PendingToken = c.GetPendingTokenValue(),
                ReturnIdpCredential = true,
                ReturnSecureToken = true
            }).ConfigureAwait(false);
            
            credential = this.GetCredential(response);

            response.Validate(credential);

            return new UserCredential(user, credential, OperationType.SignIn);
        }

        protected internal override async Task<UserCredential> LinkWithCredentialAsync(string idToken, AuthCredential credential)
        {
            var c = (OAuthCredential)credential;
            var (user, response) = await this.verifyAssertion.ExecuteAndParseAsync(credential.ProviderType, new VerifyAssertionRequest
            {
                IdToken = idToken,
                RequestUri = $"https://{this.config.AuthDomain}",
                PostBody = c.GetPostBodyValue(c.ProviderType),
                PendingToken = c.GetPendingTokenValue(),
                ReturnIdpCredential = true,
                ReturnSecureToken = true
            }).ConfigureAwait(false);

            credential = this.GetCredential(response);

            response.Validate(credential);

            return new UserCredential(user, this.GetCredential(response), OperationType.Link);
        }

        protected string GetParsedOauthScopes()
        {
            if (!this.scopes.Any())
            {
                return null;
            }

            return $"{{ \"{this.ProviderType.ToEnumString()}\": \"{string.Join(",", this.scopes)}\" }}";
        }

        internal class OAuthCredential : AuthCredential
        {
            public string Token { get; set; }

            public OAuthCredentialTokenType TokenType { get; set; }

            internal virtual string GetPostBodyValue(FirebaseProviderType ProviderType)
            {
                var tokenType = this.TokenType switch
                {
                    OAuthCredentialTokenType.IdToken => "id_token",
                    OAuthCredentialTokenType.PendingToken => null,
                    _ => "access_token"
                };

                return tokenType == null 
                    ? null 
                    : $"{tokenType}={this.Token}&providerId={ProviderType.ToEnumString()}";
            }

            internal virtual string GetPendingTokenValue()
            {
                return this.TokenType == OAuthCredentialTokenType.PendingToken
                    ? this.Token
                    : null;
            }
        }
    }

    public enum OAuthCredentialTokenType
    {
        AccessToken = 0,
        IdToken,
        PendingToken
    }
}
