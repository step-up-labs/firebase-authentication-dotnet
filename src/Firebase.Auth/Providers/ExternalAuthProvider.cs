using Firebase.Auth.Requests;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Firebase.Auth.Providers
{
    public abstract class ExternalAuthProvider : FirebaseAuthProvider
    {
        protected VerifyAssertion verifyAssertion;
        protected readonly List<string> scopes;

        public ExternalAuthProvider()
        {
            this.scopes = new List<string>();
        }

        public virtual FirebaseAuthProvider AddScopes(params string[] scopes)
        {
            this.scopes.AddRange(scopes);

            return this;
        }

        internal override void Initialize(FirebaseAuthConfig config)
        {
            base.Initialize(config);
            this.verifyAssertion = new VerifyAssertion(config);
        }

        internal virtual async Task<ExternalAuthContinuation> SignInAsync()
        {
            var request = new CreateAuthUriRequest
            {
                ContinueUri = this.GetContinueUri(),
                ProviderId = this.AuthType,
                CustomParameters = new Dictionary<string, string>
                {
                    ["hl"] = CultureInfo.CurrentCulture.TwoLetterISOLanguageName
                },
                OauthScope = this.GetParsedOauthScopes(),
            };

            var response = await this.createAuthUri.ExecuteAsync(request).ConfigureAwait(false);

            return new ExternalAuthContinuation(this.verifyAssertion, this.accountInfo, response.AuthUri, response.SessionId);
        }

        protected string GetParsedOauthScopes()
        {
            if (!this.scopes.Any())
            {
                return null;
            }

            return $"{{ \"{this.AuthType.ToEnumString()}\": \"{string.Join(",", this.scopes)}\" }}";
        }
    }
}
