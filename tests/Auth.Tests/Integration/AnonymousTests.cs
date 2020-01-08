using FluentAssertions;
using System.Threading.Tasks;
using Xunit;

namespace Firebase.Auth.Tests.Integration
{
    [Trait("Category", "Integration")]
    public class AnonymousTests
    {
        private readonly FirebaseAuthClient client;

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
            var user = await this.client.SignInAnonymouslyAsync();

            try
            {
                user.Info.IsAnonymous.Should().BeTrue();
                user.Uid.Should().NotBeNullOrEmpty();
                user.Info.Email.Should().BeNull();
            }
            finally
            {
                await user.DeleteAsync();
            }
        }
    }
}
