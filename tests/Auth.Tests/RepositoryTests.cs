using Firebase.Auth.Repository;
using FluentAssertions;
using Moq;
using Xunit;

namespace Firebase.Auth.Tests
{
    public class RepositoryTests
    {
        [Fact]
        public void UserRepositoryInvokesChangedEventTest()
        {
            var manager = new UserManager(InMemoryRepository.Instance);
            var user = new User(new FirebaseAuthConfig(), new UserInfo(), new FirebaseCredential());
            User invokedUser = null;

            manager.UserChanged += (s, e) => invokedUser = e.User;

            manager.SaveNewUser(user);

            invokedUser.Should().Be(user);
        }

        [Fact]
        public void UserRepositoryUsesCacheTest()
        {
            var fs = new Mock<IUserRepository>();
            var repository = new UserManager(fs.Object);
            var user = new User(new FirebaseAuthConfig(), new UserInfo(), new FirebaseCredential());

            repository.SaveNewUser(user);
            repository.GetUser();

            fs.Verify(f => f.ReadUser(), Times.Never);
            fs.Verify(f => f.SaveUser(user), Times.Once);
        }

        [Fact]
        public void UpdatingOldUserIsIgnoredTest()
        {
            const string uid = "abcd";
            const string obsoleteUid = "efgh";

            var fs = new Mock<IUserRepository>();
            var repository = new UserManager(fs.Object);
            var user = new User(new FirebaseAuthConfig(), new UserInfo { Uid = uid }, new FirebaseCredential());
            var obsoleteUser = new User(new FirebaseAuthConfig(), new UserInfo { Uid = obsoleteUid }, new FirebaseCredential());

            repository.SaveNewUser(user);
            repository.UpdateExistingUser(obsoleteUser);

            fs.Verify(f => f.SaveUser(It.IsAny<User>()), Times.Once);

            repository.SaveNewUser(obsoleteUser);

            fs.Verify(f => f.SaveUser(It.IsAny<User>()), Times.Exactly(2));
        }

        [Fact]
        public void DeletingOldUserIsIgnoredTest()
        {
            const string uid = "abcd";
            const string obsoleteUid = "efgh";

            var fs = new Mock<IUserRepository>();
            var repository = new UserManager(fs.Object);
            var user = new User(new FirebaseAuthConfig(), new UserInfo { Uid = uid }, new FirebaseCredential());
            
            repository.SaveNewUser(user);
            repository.DeleteExistingUser(obsoleteUid);

            fs.Verify(f => f.DeleteUser(), Times.Never);

            repository.DeleteExistingUser(uid);

            fs.Verify(f => f.DeleteUser(), Times.Once);
        }
    }
}
