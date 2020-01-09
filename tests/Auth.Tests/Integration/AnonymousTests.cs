using Firebase.Auth.Providers;
using FluentAssertions;
using System.Threading.Tasks;
using Xunit;

namespace Firebase.Auth.Tests.Integration
{
    [Trait("Category", "Integration")]
    public class AnonymousTests : IAsyncLifetime
    {
        private readonly FirebaseAuthClient client;

        private User user;

        public AnonymousTests()
        {
            var setup = Setup.LoadFromEnvironment();
            this.client = new FirebaseAuthClient(new FirebaseAuthConfig
            {
                ApiKey = setup.ApiKey,
                AuthDomain = setup.AuthDomain
            });
        }

        [Fact]
        public async Task AnonymousSignInTest()
        {
            user = await this.client.SignInAnonymouslyAsync();

            user.Info.IsAnonymous.Should().BeTrue();
            user.Uid.Should().NotBeNullOrEmpty();
            user.Info.Email.Should().BeNull();
        }

        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        public async Task DisposeAsync()
        {
            await this.user?.DeleteAsync();
        }
    }
}
