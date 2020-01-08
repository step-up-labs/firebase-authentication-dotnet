using Firebase.Auth;
using Firebase.Auth.Providers;
using FluentAssertions;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Firebase.Auth.Tests.Integration
{
    [Trait("Category", "Integration")]
    public class EmailTests
    {
        private const string ValidPassword = "super_secret_password";
        private const string InvalidPassword = "short";
        private const string NewUserEmail = "newintegration@test.com"; 
        private const string DisplayName = "Test User"; 

        private readonly FirebaseAuthClient client;
        
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
            var user = await this.client.CreateUserWithEmailAndPasswordAsync(NewUserEmail, ValidPassword, DisplayName);

            try
            {
                user.Uid.Should().NotBeNullOrEmpty();
                user.Info.Email.Should().Be(NewUserEmail);
                user.Info.DisplayName.Should().Be(DisplayName);
            }
            finally
            {
                await user.DeleteAsync();
            }
        }

        [Fact]
        public async Task SignInTest()
        {
            var originalUser = await this.client.CreateUserWithEmailAndPasswordAsync(NewUserEmail, ValidPassword, DisplayName);
            
            try
            {
                var user = await this.client.SignInWithEmailAndPasswordAsync(NewUserEmail, ValidPassword);

                user.Uid.Should().NotBeNullOrEmpty();
                user.Info.Email.Should().Be(NewUserEmail);
                user.Info.DisplayName.Should().Be(DisplayName);
            }
            finally
            {
                await originalUser.DeleteAsync();
            }
        }

        [Fact]
        public async Task SignInWithCredentialTest()
        {
            var originalUser = await this.client.CreateUserWithEmailAndPasswordAsync(NewUserEmail, ValidPassword, DisplayName);

            try
            {
                var credential = EmailProvider.GetCredential(NewUserEmail, ValidPassword);
                var user = await this.client.SignInWithCredentialAsync(credential);

                user.Uid.Should().NotBeNullOrEmpty();
                user.Info.Email.Should().Be(NewUserEmail);
                user.Info.DisplayName.Should().Be(DisplayName);
            }
            finally
            {
                await originalUser.DeleteAsync();
            }
        }

        [Fact]
        public async Task SignInInvalidPasswordTest()
        {
            var user = await this.client.CreateUserWithEmailAndPasswordAsync(NewUserEmail, ValidPassword, DisplayName);

            try
            {
                Func<Task<User>> signIn = () => this.client.SignInWithEmailAndPasswordAsync(NewUserEmail, InvalidPassword);
                await signIn.Should().ThrowAsync<FirebaseAuthException>();
            }
            finally
            {
                await user.DeleteAsync();
            }
        }

        [Fact]
        public async Task ResetPasswordTest()
        {
            var user = await this.client.CreateUserWithEmailAndPasswordAsync(NewUserEmail, ValidPassword, DisplayName);

            try
            {
                // only check the call succeeds
                await client.ResetEmailPasswordAsync(NewUserEmail);
            }
            finally
            {
                await user.DeleteAsync();
            }
        }
        
        [Fact]
        public async Task FetchSignInProvidersTest()
        {
            var user = await this.client.CreateUserWithEmailAndPasswordAsync(NewUserEmail, ValidPassword, DisplayName);

            try
            {
                // only check the call succeeds
                var result = await client.FetchSignInMethodsForEmailAsync(NewUserEmail);
                result.AllProviders.Should().BeEquivalentTo(FirebaseProviderType.EmailAndPassword);
            }
            finally
            {
                await user.DeleteAsync();
            }
        }
    }
}
