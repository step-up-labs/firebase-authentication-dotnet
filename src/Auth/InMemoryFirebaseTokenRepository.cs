using System;
using System.Threading.Tasks;

namespace Firebase.Auth
{
    public class InMemoryFirebaseTokenRepository : IFirebaseTokenRepository
    {
        private static InMemoryFirebaseTokenRepository instance;

        private User user;

        public event EventHandler<UserEventArgs> UserChanged;

        private InMemoryFirebaseTokenRepository()
        {
        }

        public static InMemoryFirebaseTokenRepository Instance => instance ?? (instance = new InMemoryFirebaseTokenRepository());

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
