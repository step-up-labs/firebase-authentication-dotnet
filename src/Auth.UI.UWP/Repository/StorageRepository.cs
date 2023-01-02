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

        public void DeleteUser()
        {
            this.settings.Values[UserStorageKey] = null;
            this.settings.Values[CredentialStorageKey] = null;
        }

        public (UserInfo userInfo, FirebaseCredential credential) ReadUser()
        {
            var info = JsonConvert.DeserializeObject<UserInfo>(this.settings.Values[UserStorageKey].ToString(), this.options);
            var credential = JsonConvert.DeserializeObject<FirebaseCredential>(this.settings.Values[CredentialStorageKey].ToString(), this.options);

            return (info, credential); 
        }

        public void SaveUser(User user)
        {
            this.settings.Values[UserStorageKey] = JsonConvert.SerializeObject(user.Info, this.options);
            this.settings.Values[CredentialStorageKey] = JsonConvert.SerializeObject(user.Credential, this.options);
        }

        public bool UserExists()
        {
            return this.settings.Values.ContainsKey(UserStorageKey);
        }
    }
}
