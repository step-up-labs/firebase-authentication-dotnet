using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Threading.Tasks;
using Windows.Storage;

namespace Firebase.Auth.Repository
{
    public class StorageRepository : IUserRepository
    {
        private const string UserStorageKey = "FirebaseUser";
        private const string CredentialStorageKey = "FirebaseCredential";

        private readonly ApplicationDataContainer settings;
        private readonly JsonSerializerSettings options;

        public StorageRepository()
        {
            this.settings = ApplicationData.Current.LocalSettings;
            this.options = new JsonSerializerSettings();
            this.options.Converters.Add(new StringEnumConverter());
        }

        public Task DeleteUserAsync()
        {
            this.settings.Values[UserStorageKey] = null;
            this.settings.Values[CredentialStorageKey] = null;

            return Task.CompletedTask;
        }

        public Task<(UserInfo userInfo, FirebaseCredential credential)> ReadUserAsync()
        {
            var info = JsonConvert.DeserializeObject<UserInfo>(this.settings.Values[UserStorageKey].ToString(), this.options);
            var credential = JsonConvert.DeserializeObject<FirebaseCredential>(this.settings.Values[CredentialStorageKey].ToString(), this.options);

            return Task.FromResult((info, credential)); 
        }

        public Task SaveUserAsync(User user)
        {
            this.settings.Values[UserStorageKey] = JsonConvert.SerializeObject(user.Info, this.options);
            this.settings.Values[CredentialStorageKey] = JsonConvert.SerializeObject(user.Credential, this.options);

            return Task.CompletedTask;
        }

        public Task<bool> UserExistsAsync()
        {
            return Task.FromResult(this.settings.Values.ContainsKey(UserStorageKey));
        }
    }
}
