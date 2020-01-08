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

            await manager.SaveUserAsync(user);

            invokedUser.Should().Be(user);
        }

        [Fact]
        public async Task UserRepositoryUsesCacheTest()
        {
            var fs = new Mock<IUserRepository>();
            var repository = new UserManager(fs.Object);
            var user = new User(new FirebaseAuthConfig(), new UserInfo(), new FirebaseCredential());

            fs.Setup(f => f.SaveUserAsync(It.IsAny<User>())).Returns<User>(u => Task.CompletedTask);
            
            await repository.SaveUserAsync(user);
            await repository.GetUserAsync();

            fs.Verify(f => f.ReadUserAsync(), Times.Never);
            fs.Verify(f => f.SaveUserAsync(user), Times.Once);
        }

    }
}
