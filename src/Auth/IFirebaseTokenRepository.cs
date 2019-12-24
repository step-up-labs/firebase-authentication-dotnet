using System;
using System.Threading.Tasks;

namespace Firebase.Auth
{
    public interface IFirebaseTokenRepository
    {
        Task<(UserInfo info, FirebaseCredential credential)> GetUserAsync();

        Task SaveUserAsync(User user);

        event EventHandler<UserEventArgs> UserChanged;
    }
}
