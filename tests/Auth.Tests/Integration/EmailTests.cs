using Firebase.Auth.Providers;
using FluentAssertions;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Firebase.Auth.Tests.Integration
{
    [Trait("Category", "Integration")]
    public class EmailTests : IAsyncLifetime
    {
        private const string ValidPassword = "super_secret_password";
        private const string InvalidPassword = "short";
        private const string NewUserEmail = "newintegration@test.com"; 
        private const string DisplayName = "Test User"; 

        private readonly FirebaseAuthClient client;

        private User user;

        public EmailTests()
        {
            var setup = Setup.LoadFromEnvironment();
            this.client = new FirebaseAuthClient(new FirebaseAuthConfig
            {
                ApiKey = setup.ApiKey,
                AuthDomain = setup.AuthDomain,
                Providers = new FirebaseAuthProvider[]
                {
                    new EmailProvider()
                }
            });
        }

        [Fact]
        public async Task CreateUserTest()
        {
            user = await this.client.CreateUserWithEmailAndPasswordAsync(NewUserEmail, ValidPassword, DisplayName);

            user.Uid.Should().NotBeNullOrEmpty();
            user.Info.Email.Should().Be(NewUserEmail);
            user.Info.DisplayName.Should().Be(DisplayName);
        }

        [Fact]
        public async Task SignInTest()
        {
            user = await this.client.CreateUserWithEmailAndPasswordAsync(NewUserEmail, ValidPassword, DisplayName);
            
            var signedInUser = await this.client.SignInWithEmailAndPasswordAsync(NewUserEmail, ValidPassword);

            signedInUser.Uid.Should().NotBeNullOrEmpty();
            signedInUser.Info.Email.Should().Be(NewUserEmail);
            signedInUser.Info.DisplayName.Should().Be(DisplayName);
        }

        [Fact]
        public async Task SignInWithCredentialTest()
        {
            user = await this.client.CreateUserWithEmailAndPasswordAsync(NewUserEmail, ValidPassword, DisplayName);

            var credential = EmailProvider.GetCredential(NewUserEmail, ValidPassword);
            var signedInUser = await this.client.SignInWithCredentialAsync(credential);

            signedInUser.Uid.Should().NotBeNullOrEmpty();
            signedInUser.Info.Email.Should().Be(NewUserEmail);
            signedInUser.Info.DisplayName.Should().Be(DisplayName);
        }

        [Fact]
        public async Task SignInInvalidPasswordTest()
        {
            user = await this.client.CreateUserWithEmailAndPasswordAsync(NewUserEmail, ValidPassword, DisplayName);

            Func<Task<User>> signIn = () => this.client.SignInWithEmailAndPasswordAsync(NewUserEmail, InvalidPassword);
            await signIn.Should().ThrowAsync<FirebaseAuthException>();
        }

        [Fact]
        public async Task ResetPasswordTest()
        {
            user = await this.client.CreateUserWithEmailAndPasswordAsync(NewUserEmail, ValidPassword, DisplayName);

            // only check the call succeeds
            await client.ResetEmailPasswordAsync(NewUserEmail);
        }
        
        [Fact]
        public async Task FetchSignInProvidersTest()
        {
            user = await this.client.CreateUserWithEmailAndPasswordAsync(NewUserEmail, ValidPassword, DisplayName);

            // only check the call succeeds
            var result = await client.FetchSignInMethodsForEmailAsync(NewUserEmail);
            result.AllProviders.Should().BeEquivalentTo(FirebaseProviderType.EmailAndPassword);
        }

        [Fact]
        public async Task LinkAnonymousWithEmailTest()
        {
            user = await this.client.SignInAnonymouslyAsync();

            var u = await user.LinkWithCredentialAsync(EmailProvider.GetCredential(NewUserEmail, ValidPassword));

            u.Uid.Should().BeEquivalentTo(user.Uid);
            u.Info.Email.Should().BeEquivalentTo(NewUserEmail);
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
