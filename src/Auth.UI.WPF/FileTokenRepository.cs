using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Firebase.Auth.UI
{
    public class FileTokenRepository : IFirebaseTokenRepository
    {
        private readonly string filename;
        private readonly JsonSerializerSettings options;

        public event EventHandler<UserEventArgs> UserChanged;

        public FileTokenRepository(string foldername)
        {
            var appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            this.filename = Path.Combine(appdata, foldername, "firebase.json");
            this.options = new JsonSerializerSettings();
            this.options.Converters.Add(new StringEnumConverter());

            Directory.CreateDirectory(Path.Combine(appdata, foldername));
        }

        public Task<(UserInfo info, FirebaseCredential credential)> GetUserAsync()
        {
            if (!File.Exists(this.filename))
            {
                return Task.FromResult<(UserInfo, FirebaseCredential)>((null, null));
            }

            var content = File.ReadAllText(this.filename);

            var obj = JsonConvert.DeserializeObject<UserDal>(content, this.options);

            return Task.FromResult((obj.UserInfo, obj.Credential));
        }

        public Task SaveUserAsync(User user)
        {
            if (user == null)
            {
                File.Delete(this.filename);
            }
            else
            {
                var content = JsonConvert.SerializeObject(new UserDal(user.Info, user.Credential), this.options);
                File.WriteAllText(this.filename, content);
            }

            this.UserChanged?.Invoke(this, new UserEventArgs(user));

            return Task.CompletedTask;
        }

        internal class UserDal
        {
            public UserDal()
            {
            }

            public UserDal(UserInfo userInfo, FirebaseCredential credential)
            {
                this.UserInfo = userInfo;
                this.Credential = credential;
            }

            public UserInfo UserInfo { get; set; }

            public FirebaseCredential Credential { get; set; }
        }
    }
}
