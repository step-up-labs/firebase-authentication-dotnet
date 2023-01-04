using System.Threading.Tasks;

namespace Firebase.Auth.Repository
{
    /// <summary>
    /// Repository abstraction for <see cref="User"/>.
    /// </summary>
    public interface IUserRepository
    {
        bool UserExists();

        (UserInfo userInfo, FirebaseCredential credential) ReadUser();

        void SaveUser(User user);

        void DeleteUser();
    }
}
