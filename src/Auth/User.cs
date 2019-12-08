using Firebase.Auth.Requests;
using System.Threading.Tasks;

namespace Firebase.Auth
{
    public class User
    {
        private readonly DeleteAccount deleteAccount;
        private readonly FirebaseAuthConfig config;

        internal User(FirebaseAuthConfig config, UserInfo userInfo, FirebaseCredential credential)
        {
            this.config = config;
            this.Info = userInfo;
            this.Credential = credential;
            this.deleteAccount = new DeleteAccount(config);
        }

        public UserInfo Info { get; }

        public FirebaseCredential Credential { get; }

        public async Task<string> GetIdTokenAsync(bool forceRefresh = false)
        {
            if (forceRefresh || this.Credential.IsExpired())
            {
                // refresh, set
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
