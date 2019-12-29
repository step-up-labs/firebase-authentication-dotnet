using System;
using System.Threading.Tasks;

namespace Firebase.Auth
{
    /// <inherit />
    public class InMemoryUserRepository : IUserRepository
    {
        private static InMemoryUserRepository instance;

        private User user;

        public event EventHandler<UserEventArgs> UserChanged;

        private InMemoryUserRepository()
        {
        }

        public static InMemoryUserRepository Instance => instance ?? (instance = new InMemoryUserRepository());

        public Task SaveUserAsync(User user)
        {
            this.user = user;
            this.UserChanged?.Invoke(this, new UserEventArgs(user));
            return Task.CompletedTask;
        }

        public Task<(UserInfo, FirebaseCredential)> GetUserAsync()
        {
            return Task.FromResult((this.user?.Info, this.user?.Credential));
        }
    }
}
