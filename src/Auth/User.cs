using Firebase.Auth.Providers;
using Firebase.Auth.Requests;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Firebase.Auth
{
    /// <summary>
    /// Represents a signed-in Firebase user. 
    /// </summary>
    [DebuggerDisplay("{Uid} | {Info.Email} | {Info.DisplayName}")]
    public class User
    {
        private const string TokenGrantType = "refresh_token";

        private readonly DeleteAccount deleteAccount;
        private readonly RefreshToken token;
        private readonly UpdateAccount updateAccount;
        private readonly SetAccountUnlink unlinkAccount;
        private readonly SetAccountInfo setAccountInfo;
        private readonly FirebaseAuthConfig config;

        internal User(FirebaseAuthConfig config, UserInfo userInfo, FirebaseCredential credential)
        {
            this.config = config;
            this.Info = userInfo;
            this.Credential = credential;
            this.deleteAccount = new DeleteAccount(config);
            this.token = new RefreshToken(config);
            this.updateAccount = new UpdateAccount(config);
            this.unlinkAccount = new SetAccountUnlink(config);
            this.setAccountInfo = new SetAccountInfo(config);
        }

        /// <summary>
        /// Firebase user ID.
        /// </summary>
        public string Uid => this.Info.Uid;

        /// <summary>
        /// Specifies whether this user is anonymous.
        /// </summary>
        public bool IsAnonymous => this.Info.IsAnonymous;

        /// <summary>
        /// More information about current user.
        /// </summary>
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

                this.config.UserManager.UpdateExistingUser(this);
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

            this.config.UserManager.DeleteExistingUser(this.Uid);
        }

        /// <summary>
        /// Change user's password.
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

            this.config.UserManager.UpdateExistingUser(this);
        }

        /// <summary>
        /// Change user's display name.
        /// </summary>
        /// <param name="displayName"> The new display name. </param>
        public async Task ChangeDisplayNameAsync(string displayName)
        {
            var token = await this.GetIdTokenAsync().ConfigureAwait(false);
            var result = await this.setAccountInfo.ExecuteAsync(new SetAccountDisplayName
            {
                IdToken = token,
                DisplayName = displayName,
                ReturnSecureToken = true
            }).ConfigureAwait(false);

            this.Info.DisplayName = result.DisplayName;

            this.config.UserManager.UpdateExistingUser(this);
        }

        /// <summary>
        /// Link this user with another credential. The user represented by the <paramref name="credential"/> object must not already exist in Firebase.
        /// </summary>
        /// <param name="credential"> Provider-specifc credentials. </param>
        public async Task<UserCredential> LinkWithCredentialAsync(AuthCredential credential)
        {
            var provider = this.config.Providers.FirstOrDefault(p => p.ProviderType == credential.ProviderType);

            if (provider == null)
            {
                throw new InvalidOperationException($"Provider {credential.ProviderType} is not configured");
            }

            var token = await this.GetIdTokenAsync().ConfigureAwait(false);
            var userCredential = await provider.LinkWithCredentialAsync(token, credential).ConfigureAwait(false);

            this.Credential = userCredential.User.Credential;
            this.Info = userCredential.User.Info;

            this.config.UserManager.UpdateExistingUser(userCredential.User);

            return userCredential;
        }

        public async Task<UserCredential> LinkWithRedirectAsync(FirebaseProviderType providerType, SignInRedirectDelegate redirectDelegate)
        {
            var provider = this.config.Providers.FirstOrDefault(p => p.ProviderType == providerType);

            if (!(provider is OAuthProvider oauthProvider))
            {
                throw new InvalidOperationException("You cannot sign in with this provider using this method.");
            }

            var continuation = await oauthProvider.SignInAsync();
            var redirectUri = await redirectDelegate(continuation.Uri).ConfigureAwait(false);

            if (string.IsNullOrEmpty(redirectUri))
            {
                return null;
            }

            var token = await this.GetIdTokenAsync().ConfigureAwait(false);
            var userCredential = await continuation.ContinueSignInAsync(redirectUri, token).ConfigureAwait(false);

            this.Credential = userCredential.User.Credential;
            this.Info = userCredential.User.Info;

            this.config.UserManager.UpdateExistingUser(userCredential.User);

            return userCredential;
        }

        /// <summary>
        /// Unlinks a provider from a user account.
        /// </summary>
        public async Task<User> UnlinkAsync(FirebaseProviderType providerType)
        {
            var token = await this.GetIdTokenAsync().ConfigureAwait(false);
            await this.unlinkAccount.ExecuteAsync(new SetAccountUnlinkRequest
            {
                IdToken = token,
                DeleteProviders = new[]
                {
                    providerType
                }
            });

            return this;
        }
    }
}
