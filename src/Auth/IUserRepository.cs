using System;
using System.Threading.Tasks;

namespace Firebase.Auth
{
    /// <summary>
    /// Repository for user related data - namely <see cref="UserInfo"/> and <see cref="FirebaseCredential"/>.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Get user related details from repository.
        /// </summary>
        Task<(UserInfo info, FirebaseCredential credential)> GetUserAsync();

        /// <summary>
        /// Save user to repository.
        /// </summary>
        Task SaveUserAsync(User user);

        /// <summary>
        /// Event raised when <see cref="SaveUserAsync(User)"/> is called.
        /// </summary>
        event EventHandler<UserEventArgs> UserChanged;
    }
}
