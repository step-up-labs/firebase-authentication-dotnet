using Firebase.Auth.Providers;
using Firebase.Auth.Requests;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Firebase.Auth
{
    public class FirebaseAuthClient
    {
        private readonly FirebaseAuthConfig config;
        private readonly ProjectConfig projectConfig;
        private readonly SignupNewUser signupNewUser;

        private bool domainChecked;
        
        public FirebaseAuthClient(FirebaseAuthConfig config)
        {
            this.config = config;
            this.projectConfig = new ProjectConfig(this.config);
            this.signupNewUser = new SignupNewUser(this.config);

            foreach (var provider in this.config.Providers)
            {
                provider.Initialize(this.config);
            }
        }

        public async Task<User> SignInExternallyAsync(FirebaseProviderType authType, ExternalSignInDelegate externalSignInDelegate)
        {
            if (!IsProviderExternal(authType))
            {
                throw new InvalidOperationException("You cannot sign in with this provider using this method.");
            }

            await this.CheckAuthDomain().ConfigureAwait(false);

            var provider = (ExternalAuthProvider)this.GetAuthProvider(authType);
            var continuation = await provider.SignInAsync().ConfigureAwait(false);
            var redirectUri = await externalSignInDelegate(continuation.Uri).ConfigureAwait(false);
            var user = await continuation.ContinueSignInAsync(redirectUri).ConfigureAwait(false);

            await this.SaveTokenAsync(user).ConfigureAwait(false);

            return user;
        }

        public async Task<User> SignInAnonymouslyAsync()
        {
            var response = await this.signupNewUser.ExecuteAsync(new SignupNewUserRequest { ReturnSecureToken = true }).ConfigureAwait(false);
            var credential = new FirebaseCredential
            {
                ExpiresIn = response.ExpiresIn,
                IdToken = response.IdToken,
                RefreshToken = response.RefreshToken,
                ProviderType = FirebaseProviderType.Anonymous
            };

            var info = new UserInfo
            {
                Uid = response.LocalId,
                IsAnonymous = true
            };

            var user = new User(this.config, info, credential);

            await this.SaveTokenAsync(user);

            return user;
        }

        public async Task<CheckUserRessult> CheckUserEmailExistsAsync(string email)
        {
            await this.CheckAuthDomain().ConfigureAwait(false);

            var provider = (EmailProvider)this.GetAuthProvider(FirebaseProviderType.EmailAndPassword);

            return await provider.CheckUserExistsAsync(email).ConfigureAwait(false);
        }

        public async Task<User> SignInWithEmailAndPasswordAsync(string email, string password)
        {
            await this.CheckAuthDomain().ConfigureAwait(false);

            var provider = (EmailProvider)this.GetAuthProvider(FirebaseProviderType.EmailAndPassword);
            var result = await provider.SignInUserAsync(email, password).ConfigureAwait(false);

            await this.SaveTokenAsync(result).ConfigureAwait(false);

            return result;
        }
        
        public async Task<User> CreateUserWithEmailAndPasswordAsync(string email, string password, string displayName = null)
        {
            await this.CheckAuthDomain().ConfigureAwait(false);

            var provider = (EmailProvider)this.GetAuthProvider(FirebaseProviderType.EmailAndPassword);
            var result = await provider.SignUpUserAsync(email, password, displayName).ConfigureAwait(false);

            await this.SaveTokenAsync(result).ConfigureAwait(false);

            return result;
        }

        private Task SaveTokenAsync(User user)
        {
            return this.config.UserRepository.SaveTokenAsync(user);
        }

        private FirebaseAuthProvider GetAuthProvider(FirebaseProviderType authType)
        {
            return this.config.Providers.FirstOrDefault(f => f.ProviderType == authType) 
                ?? throw new InvalidOperationException($"Provider {authType} is not configured, you need to add it to your FirebaseAuthConfig");
        }

        private bool IsProviderExternal(FirebaseProviderType authType)
        {
            switch (authType)
            {
                case FirebaseProviderType.Facebook:
                case FirebaseProviderType.Google:
                case FirebaseProviderType.Github:
                case FirebaseProviderType.Twitter:
                case FirebaseProviderType.Microsoft:
                    return true;
                case FirebaseProviderType.EmailAndPassword:
                case FirebaseProviderType.Anonymous:
                    return false;
                default:
                    throw new ArgumentException("Unknown provider");
            }
        }

        private async Task CheckAuthDomain()
        {
            if (this.domainChecked)
            {
                return;
            }

            var result = await this.projectConfig.ExecuteAsync(null).ConfigureAwait(false);
            if (!result.AuthorizedDomains.Contains(this.config.AuthDomain))
            {
                throw new InvalidOperationException("Auth domain is not among the authorized ones");
            }

            this.domainChecked = true;
        }
    }

    public interface IFirebaseTokenRepository
    {
        Task<User> GetUserAsync();

        Task SaveTokenAsync(User credential);
    }

    public class InMemoryFirebaseTokenRepository : IFirebaseTokenRepository
    {
        private static InMemoryFirebaseTokenRepository instance;

        private User user;

        private InMemoryFirebaseTokenRepository()
        {
        }

        public static InMemoryFirebaseTokenRepository Instance => instance ?? (instance = new InMemoryFirebaseTokenRepository());

        public Task SaveTokenAsync(User user)
        {
            this.user = user;
            return Task.CompletedTask;
        }

        public Task<User> GetUserAsync()
        {
            return Task.FromResult(this.user);
        }
    }
}
