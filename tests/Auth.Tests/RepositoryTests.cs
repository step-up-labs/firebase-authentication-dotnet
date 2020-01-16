using Firebase.Auth.Repository;
using FluentAssertions;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace Firebase.Auth.Tests
{
    public class RepositoryTests
    {
        [Fact]
        public async Task UserRepositoryInvokesChangedEventTest()
        {
            var manager = new UserManager(InMemoryRepository.Instance);
            var user = new User(new FirebaseAuthConfig(), new UserInfo(), new FirebaseCredential());
            User invokedUser = null;

            manager.UserChanged += (s, e) => invokedUser = e.User;

            await manager.SaveNewUserAsync(user);

            invokedUser.Should().Be(user);
        }

        [Fact]
        public async Task UserRepositoryUsesCacheTest()
        {
            var fs = new Mock<IUserRepository>();
            var repository = new UserManager(fs.Object);
            var user = new User(new FirebaseAuthConfig(), new UserInfo(), new FirebaseCredential());

            await repository.SaveNewUserAsync(user);
            await repository.GetUserAsync();

            fs.Verify(f => f.ReadUserAsync(), Times.Never);
            fs.Verify(f => f.SaveUserAsync(user), Times.Once);
        }

        [Fact]
        public async Task UpdatingOldUserIsIgnoredTest()
        {
            const string uid = "abcd";
            const string obsoleteUid = "efgh";

            var fs = new Mock<IUserRepository>();
            var repository = new UserManager(fs.Object);
            var user = new User(new FirebaseAuthConfig(), new UserInfo { Uid = uid }, new FirebaseCredential());
            var obsoleteUser = new User(new FirebaseAuthConfig(), new UserInfo { Uid = obsoleteUid }, new FirebaseCredential());

            await repository.SaveNewUserAsync(user);
            await repository.UpdateExistingUserAsync(obsoleteUser);

            fs.Verify(f => f.SaveUserAsync(It.IsAny<User>()), Times.Once);

            await repository.SaveNewUserAsync(obsoleteUser);

            fs.Verify(f => f.SaveUserAsync(It.IsAny<User>()), Times.Exactly(2));
        }

        [Fact]
        public async Task DeletingOldUserIsIgnoredTest()
        {
            const string uid = "abcd";
            const string obsoleteUid = "efgh";

            var fs = new Mock<IUserRepository>();
            var repository = new UserManager(fs.Object);
            var user = new User(new FirebaseAuthConfig(), new UserInfo { Uid = uid }, new FirebaseCredential());
            
            await repository.SaveNewUserAsync(user);
            await repository.DeleteExistingUserAsync(obsoleteUid);

            fs.Verify(f => f.DeleteUserAsync(), Times.Never);

            await repository.DeleteExistingUserAsync(uid);

            fs.Verify(f => f.DeleteUserAsync(), Times.Once);
        }
    }
}
