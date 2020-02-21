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

        private UserCredential userCredential;

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
            userCredential = await this.client.CreateUserWithEmailAndPasswordAsync(NewUserEmail, ValidPassword, DisplayName);

            userCredential.User.Uid.Should().NotBeNullOrEmpty();
            userCredential.User.Info.Email.Should().Be(NewUserEmail);
            userCredential.User.Info.DisplayName.Should().Be(DisplayName);
        }

        [Fact]
        public async Task SignInTest()
        {
            userCredential = await this.client.CreateUserWithEmailAndPasswordAsync(NewUserEmail, ValidPassword, DisplayName);
            
            var signedInUserCrdential = await this.client.SignInWithEmailAndPasswordAsync(NewUserEmail, ValidPassword);

            signedInUserCrdential.User.Uid.Should().NotBeNullOrEmpty();
            signedInUserCrdential.User.Info.Email.Should().Be(NewUserEmail);
            signedInUserCrdential.User.Info.DisplayName.Should().Be(DisplayName);
        }

        [Fact]
        public async Task SignInWithCredentialTest()
        {
            userCredential = await this.client.CreateUserWithEmailAndPasswordAsync(NewUserEmail, ValidPassword, DisplayName);

            var credential = EmailProvider.GetCredential(NewUserEmail, ValidPassword);
            var signedInUserCrdential = await this.client.SignInWithCredentialAsync(credential);

            signedInUserCrdential.User.Uid.Should().NotBeNullOrEmpty();
            signedInUserCrdential.User.Info.Email.Should().Be(NewUserEmail);
            signedInUserCrdential.User.Info.DisplayName.Should().Be(DisplayName);
        }

        [Fact]
        public async Task SignInInvalidPasswordTest()
        {
            userCredential = await this.client.CreateUserWithEmailAndPasswordAsync(NewUserEmail, ValidPassword, DisplayName);

            Func<Task<UserCredential>> signIn = () => this.client.SignInWithEmailAndPasswordAsync(NewUserEmail, InvalidPassword);
            await signIn.Should().ThrowAsync<FirebaseAuthException>();
        }

        [Fact]
        public async Task ResetPasswordTest()
        {
            userCredential = await this.client.CreateUserWithEmailAndPasswordAsync(NewUserEmail, ValidPassword, DisplayName);

            // only check the call succeeds
            await client.ResetEmailPasswordAsync(NewUserEmail);
        }
        
        [Fact]
        public async Task FetchSignInProvidersTest()
        {
            userCredential = await this.client.CreateUserWithEmailAndPasswordAsync(NewUserEmail, ValidPassword, DisplayName);

            // only check the call succeeds
            var result = await client.FetchSignInMethodsForEmailAsync(NewUserEmail);
            result.AllProviders.Should().BeEquivalentTo(FirebaseProviderType.EmailAndPassword);
        }

        [Fact]
        public async Task LinkAnonymousWithEmailTest()
        {
            userCredential = await this.client.SignInAnonymouslyAsync();

            var u = await userCredential.User.LinkWithCredentialAsync(EmailProvider.GetCredential(NewUserEmail, ValidPassword));

            u.User.Uid.Should().BeEquivalentTo(userCredential.User.Uid);
            u.User.Info.Email.Should().BeEquivalentTo(NewUserEmail);
        }

        [Fact]
        public async Task LinkAnonymousWithExistingEmailThrowsExceptionTest()
        {
            var emailCredential = await this.client.CreateUserWithEmailAndPasswordAsync(NewUserEmail, ValidPassword);

            userCredential = await this.client.SignInAnonymouslyAsync();

            try
            {
                var e = await Assert.ThrowsAsync<FirebaseAuthWithCredentialException>(() => userCredential.User.LinkWithCredentialAsync(emailCredential.AuthCredential));
                e.Reason.Should().Be(AuthErrorReason.EmailExists);
            }
            finally
            {
                // anonymous user gets deleted in DisposeAsync, also delete the email user here
                await emailCredential.User.DeleteAsync();
            }

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
