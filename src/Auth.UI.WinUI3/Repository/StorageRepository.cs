using Firebase.Auth.UI.Repository;
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

        //private readonly ApplicationDataContainer settings;
        private readonly JsonSerializerSettings options;

        public StorageRepository()
        {
            //this.settings = ApplicationData.Current.LocalSettings;
            this.options = new JsonSerializerSettings();
            this.options.Converters.Add(new StringEnumConverter());
        }

        public void DeleteUser()
        {
            LocalSettingsWrapper.Instance.SaveSettingAsync<string>(UserStorageKey, null).Wait();
            LocalSettingsWrapper.Instance.SaveSettingAsync<string>(CredentialStorageKey, null).Wait();
            //this.settings.Values[UserStorageKey] = null;
            //this.settings.Values[CredentialStorageKey] = null;
        }

        public (UserInfo userInfo, FirebaseCredential credential) ReadUser()
        {
            string storageKey = LocalSettingsWrapper.Instance.ReadSettingAsync<string>(UserStorageKey).Result;
            string credentialKey = LocalSettingsWrapper.Instance.ReadSettingAsync<string>(CredentialStorageKey).Result;
            if (storageKey == null || credentialKey == null)
            {
                return (null, null);
            }
            var info = JsonConvert.DeserializeObject<UserInfo>(storageKey, this.options);
            var credential = JsonConvert.DeserializeObject<FirebaseCredential>(credentialKey, this.options);

            return (info, credential);
        }

        public void SaveUser(User user)
        {
            LocalSettingsWrapper.Instance.SaveSettingAsync<string>(UserStorageKey, JsonConvert.SerializeObject(user.Info, this.options)).Wait();
            LocalSettingsWrapper.Instance.SaveSettingAsync<string>(CredentialStorageKey, JsonConvert.SerializeObject(user.Credential, this.options)).Wait();
            //this.settings.Values[UserStorageKey] = JsonConvert.SerializeObject(user.Info, this.options);
            //this.settings.Values[CredentialStorageKey] = JsonConvert.SerializeObject(user.Credential, this.options);
        }

        public bool UserExists()
        {
            string storageKey = LocalSettingsWrapper.Instance.ReadSettingAsync<string>(UserStorageKey).Result;
            return storageKey != null;
            //return LocalSettingsWrapper.Instance.Exists(UserStorageKey);
            //return this.settings.Values.ContainsKey(UserStorageKey);
        }
    }
}
