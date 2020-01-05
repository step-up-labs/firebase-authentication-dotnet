using Firebase.Auth.Requests;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Threading.Tasks;
using static Firebase.Auth.Providers.EmailProvider;
using static Firebase.Auth.Providers.OAuthProvider;

namespace Firebase.Auth
{
    public class User
    {
        private const string TokenGrantType = "refresh_token";

        private readonly DeleteAccount deleteAccount;
        private readonly RefreshToken token;
        private readonly UpdateAccount updateAccount;
        private readonly VerifyAssertion verifyAssertion;
        private readonly GetAccountInfo getAccount;
        private readonly FirebaseAuthConfig config;

        internal User(FirebaseAuthConfig config, UserInfo userInfo, FirebaseCredential credential)
        {
            this.config = config;
            this.Info = userInfo;
            this.Credential = credential;
            this.deleteAccount = new DeleteAccount(config);
            this.token = new RefreshToken(config);
            this.updateAccount = new UpdateAccount(config);
            this.getAccount = new GetAccountInfo(config);
            this.verifyAssertion = new VerifyAssertion(config);
        }

        public string Uid => this.Info.Uid;

        public UserInfo Info { get; private set; }

        public FirebaseCredential Credential { get; private set; }

        /// <summary>
        /// Get fresh firebase id token.
        /// </summary>
        /// <param name="forceRefresh"> Specifies whether the token should be refreshed even if it's not expired. </param>
        public async Task<string> GetIdTokenAsync(bool forceRefresh = false)
        {
            if (forceRefresh || this.Credential.IsExpired())
            {
                var refresh = await this.token.ExecuteAsync(new RefreshTokenRequest
                {
                    GrantType = TokenGrantType,
                    RefreshToken = this.Credential.RefreshToken
                });

                this.Credential = new FirebaseCredential
                {
                    ExpiresIn = refresh.ExpiresIn,
                    IdToken = refresh.IdToken,
                    ProviderType = this.Credential.ProviderType,
                    RefreshToken = refresh.RefreshToken
                };

                await this.config.UserRepository.SaveUserAsync(this).ConfigureAwait(false);
            }

            return this.Credential.IdToken;
        }

        /// <summary>
        /// Delete user's account.
        /// </summary>
        public async Task DeleteAsync()
        {
            var token = await this.GetIdTokenAsync().ConfigureAwait(false);

            await this.deleteAccount.ExecuteAsync(new IdTokenRequest { IdToken = token }).ConfigureAwait(false);

            await this.config.UserRepository.SaveUserAsync(null).ConfigureAwait(false);
        }

        /// <summary>
        /// Change a user's password.
        /// </summary>
        /// <param name="password"> The new password. </param>
        public async Task ChangePasswordAsync(string password)
        {
            var token = await this.GetIdTokenAsync().ConfigureAwait(false);
            var result = await this.updateAccount.ExecuteAsync(new UpdateAccountRequest 
            { 
                IdToken = token,
                Password = password,
                ReturnSecureToken = true
            }).ConfigureAwait(false);

            this.Credential = new FirebaseCredential
            {
                ExpiresIn = result.ExpiresIn,
                IdToken = result.IdToken,
                ProviderType = this.Credential.ProviderType,
                RefreshToken = result.RefreshToken
            };

            await this.config.UserRepository.SaveUserAsync(this).ConfigureAwait(false);
        }

        public async Task<User> LinkWithCredentialAsync(AuthCredential credential)
        {
            var provider = this.config.Providers.FirstOrDefault(p => p.ProviderType == credential.ProviderType);

            if (provider == null)
            {
                throw new InvalidOperationException($"Provider {credential.ProviderType} is not configured");
            }

            var token = await this.GetIdTokenAsync().ConfigureAwait(false);
            var user = await provider.SignInWithCredentialAsync(credential).ConfigureAwait(false);

            this.Credential = user.Credential;
            this.Info = user.Info;

            await this.config.UserRepository.SaveUserAsync(this).ConfigureAwait(false);

            return user;
        }
    }
}
