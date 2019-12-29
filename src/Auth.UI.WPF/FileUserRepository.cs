using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Firebase.Auth.UI
{
    /// <summary>
    /// <see cref="IUserRepository"/> implementation which saves user related data to a json file in ApplicationData folder.
    /// </summary>
    public class FileUserRepository : IUserRepository
    {
        public const string UserFileName = "firebase.json";

        private readonly string filename;
        private readonly JsonSerializerSettings options;

        private (UserInfo info, FirebaseCredential credential)? cache;

        public event EventHandler<UserEventArgs> UserChanged;

        /// <summary>
        /// Creates a new instance of <see cref="FileUserRepository"/>.
        /// </summary>
        /// <param name="foldername"> Name of subfolder to be created under ApplicationData folder. </param>
        public FileUserRepository(string foldername)
        {
            var appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            this.filename = Path.Combine(appdata, foldername, UserFileName);
            this.options = new JsonSerializerSettings();
            this.options.Converters.Add(new StringEnumConverter());

            Directory.CreateDirectory(Path.Combine(appdata, foldername));
        }

        public Task<(UserInfo info, FirebaseCredential credential)> GetUserAsync()
        {
            if (!this.cache.HasValue)
            {
                if (!File.Exists(this.filename))
                {
                    this.cache = (null, null);
                    return Task.FromResult<(UserInfo, FirebaseCredential)>(this.cache.Value);
                }

                var content = File.ReadAllText(this.filename);
                var obj = JsonConvert.DeserializeObject<UserDal>(content, this.options);

                this.cache = (obj.UserInfo, obj.Credential);
            }

            return Task.FromResult(this.cache.Value);
        }

        public Task SaveUserAsync(User user)
        {
            this.cache = (user?.Info, user?.Credential);

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
