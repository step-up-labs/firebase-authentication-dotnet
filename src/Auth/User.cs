using Firebase.Auth.Requests;
using System.Threading.Tasks;

namespace Firebase.Auth
{
    public class User
    {
        private const string TokenGrantType = "refresh_token";

        private readonly DeleteAccount deleteAccount;
        private readonly RefreshToken token;
        private readonly FirebaseAuthConfig config;

        internal User(FirebaseAuthConfig config, UserInfo userInfo, FirebaseCredential credential)
        {
            this.config = config;
            this.Info = userInfo;
            this.Credential = credential;
            this.deleteAccount = new DeleteAccount(config);
            this.token = new RefreshToken(config);
        }

        public string Uid => this.Info.Uid;

        public UserInfo Info { get; private set; }

        public FirebaseCredential Credential { get; private set; }

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
            }

            return this.Credential.IdToken;
        }

        public async Task DeleteAsync()
        {
            var token = await this.GetIdTokenAsync().ConfigureAwait(false);

            await this.deleteAccount.ExecuteAsync(new IdTokenRequest { IdToken = token });
        }
    }
}
