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

        public async Task SaveNewUserAsync(User user)
        {
            this.cache = (user.Info, user.Credential);

            await this.fs.SaveUserAsync(user).ConfigureAwait(false);

            this.UserChanged?.Invoke(this, new UserEventArgs(user));
        }

        public Task UpdateExistingUserAsync(User user)
        {
            if (user.Uid != this.cache?.info.Uid)
            {
                // if updating a user, it needs to be the active one, otherwise don't do anything
                return Task.CompletedTask;
            }

            // continue updating current user
            return this.SaveNewUserAsync(user);
        }

        public async Task DeleteExistingUserAsync(string uid)
        {
            if (cache?.info.Uid != uid)
            {
                // deleting a user which is not an active user
                return;
            }

            this.cache = (null, null);

            await this.fs.DeleteUserAsync().ConfigureAwait(false);
            
            this.UserChanged?.Invoke(this, new UserEventArgs(null));
        }
    }
}
