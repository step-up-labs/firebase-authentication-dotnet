using System.Threading.Tasks;

namespace Firebase.Auth.Repository
{
    /// <summary>
    /// Repository abstraction for <see cref="User"/>.
    /// </summary>
    public interface IUserRepository
    {
        Task<bool> UserExistsAsync();

        Task<(UserInfo userInfo, FirebaseCredential credential)> ReadUserAsync();

        Task SaveUserAsync(User user);

        Task DeleteUserAsync();
    }
}
