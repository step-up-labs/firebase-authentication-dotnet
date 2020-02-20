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

        private UserCredential userCredential;

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
            userCredential = await this.client.SignInAnonymouslyAsync();

            userCredential.AuthCredential.Should().BeNull();
            userCredential.User.Info.IsAnonymous.Should().BeTrue();
            userCredential.User.Uid.Should().NotBeNullOrEmpty();
            userCredential.User.Info.Email.Should().BeNull();
        }

        [Fact]
        public async Task AuthStateChangedInvokeAfterTokenRefresh()
        {
            userCredential = await this.client.SignInAnonymouslyAsync();

            var invoked = 0;

            this.client.AuthStateChanged += (s, e) => invoked++;

            await userCredential.User.GetIdTokenAsync(true);

            invoked.Should().Be(2); // 1x after event subscribe, one after refresh
        }

        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        public async Task DisposeAsync()
        {
            await this.userCredential?.User?.DeleteAsync();
        }
    }
}
