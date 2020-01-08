using Firebase.Auth.Repository;
using System;
using System.Threading.Tasks;

namespace Firebase.Auth
{
    public class UserManager
    {
        private readonly IUserRepository fs;

        private (UserInfo info, FirebaseCredential credential)? cache;

        public event EventHandler<UserEventArgs> UserChanged;

        /// <summary>
        /// Creates a new instance of <see cref="UserManager"/> with custom repository.
        /// </summary>
        /// <param name="fileSystem"> Proxy to the filesystem operations. </param>
        public UserManager(IUserRepository fileSystem) 
        {
            this.fs = fileSystem;
        }

        public async Task<(UserInfo info, FirebaseCredential credential)> GetUserAsync()
        {
            if (!this.cache.HasValue)
            {
                if (!await this.fs.UserExistsAsync().ConfigureAwait(false))
                {
                    this.cache = (null, null);
                    return this.cache.Value;
                }

                this.cache = await this.fs.ReadUserAsync().ConfigureAwait(false);
            }

            return this.cache.Value;
        }

        public async Task SaveUserAsync(User user)
        {
            this.cache = (user?.Info, user?.Credential);

            if (user == null)
            {
                await this.fs.DeleteUserAsync().ConfigureAwait(false);
            }
            else
            {
                await this.fs.SaveUserAsync(user).ConfigureAwait(false);
            }

            this.UserChanged?.Invoke(this, new UserEventArgs(user));
        }
    }
}
