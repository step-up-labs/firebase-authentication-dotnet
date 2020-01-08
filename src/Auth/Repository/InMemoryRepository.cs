using System;
using System.Threading.Tasks;

namespace Firebase.Auth.Repository
{
    /// <inherit />
    public class InMemoryRepository : IUserRepository
    {
        private static InMemoryRepository instance;

        private User user;

        public event EventHandler<UserEventArgs> UserChanged;

        private InMemoryRepository()
        {
        }

        public static InMemoryRepository Instance => instance ?? (instance = new InMemoryRepository());

        public Task<bool> UserExistsAsync()
        {
            return Task.FromResult(this.user != null);
        }

        public Task<(UserInfo, FirebaseCredential)> ReadUserAsync()
        {
            return Task.FromResult((this.user?.Info, this.user?.Credential));
        }

        public Task SaveUserAsync(User user)
        {
            this.user = user;
            return Task.CompletedTask;
        }

        public Task DeleteUserAsync()
        {
            this.user = null;
            return Task.CompletedTask;
        }
    }
}
