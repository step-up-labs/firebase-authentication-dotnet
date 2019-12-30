using Firebase.Auth.Requests;
using System.Threading.Tasks;

namespace Firebase.Auth
{
    public class User
    {
        private const string TokenGrantType = "refresh_token";

        private readonly DeleteAccount deleteAccount;
        private readonly RefreshToken token;
        private readonly UpdateAccount updateAccount;
        private readonly FirebaseAuthConfig config;

        internal User(FirebaseAuthConfig config, UserInfo userInfo, FirebaseCredential credential)
        {
            this.config = config;
            this.Info = userInfo;
            this.Credential = credential;
            this.deleteAccount = new DeleteAccount(config);
            this.token = new RefreshToken(config);
            this.updateAccount = new UpdateAccount(config);
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
    }
}
