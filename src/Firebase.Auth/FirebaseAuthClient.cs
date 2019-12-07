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

        public async Task<User> SignInExternallyAsync(FirebaseProviderType authType)
        {
            if (!IsProviderExternal(authType))
            {
                throw new InvalidOperationException("You cannot sign in with this provider using this method.");
            }

            await this.CheckAuthDomain().ConfigureAwait(false);

            var provider = (ExternalAuthProvider)this.GetAuthProvider(authType);
            var continuation = await provider.SignInAsync().ConfigureAwait(false);
            var redirectUri = await this.config.ExternalSignInDelegate(continuation.Uri).ConfigureAwait(false);
            var (user, token) = await continuation.ContinueSignInAsync(redirectUri).ConfigureAwait(false);

            await this.SaveTokenAsync(token).ConfigureAwait(false);

            return user;
        }

        public async Task<User> SignInAnonymouslyAsync()
        {
            var response = await this.signupNewUser.ExecuteAsync(new SignupNewUserRequest { ReturnSecureToken = true }).ConfigureAwait(false);
            var token = new FirebaseAuthToken
            {
                ExpiresIn = response.ExpiresIn,
                IdToken = response.IdToken,
                RefreshToken = response.RefreshToken
            };

            await this.SaveTokenAsync(token);

            return new User
            {
                LocalId = response.LocalId
            };
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
            var (user, token) = await provider.SignInUserAsync(email, password).ConfigureAwait(false);

            await this.SaveTokenAsync(token).ConfigureAwait(false);

            return user;
        }
        
        public async Task<User> CreateUserWithEmailAndPasswordAsync(string email, string password, string displayName = null)
        {
            await this.CheckAuthDomain().ConfigureAwait(false);

            var provider = (EmailProvider)this.GetAuthProvider(FirebaseProviderType.EmailAndPassword);
            var (user, token) = await provider.SignUpUserAsync(email, password, displayName).ConfigureAwait(false);

            await this.SaveTokenAsync(token).ConfigureAwait(false);

            return user;
        }

        public async Task<string> GetAccessTokenAsync()
        {
            var token = await this.config.TokenRepository.GetTokenAsync().ConfigureAwait(false);

            if (token.IsExpired())
            {

            }

            return token.IdToken;
        }

        private Task SaveTokenAsync(FirebaseAuthToken token)
        {
            return this.config.TokenRepository.SaveTokenAsync(token);
        }

        private FirebaseAuthProvider GetAuthProvider(FirebaseProviderType authType)
        {
            return this.config.Providers.FirstOrDefault(f => f.AuthType == authType) 
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
        Task<FirebaseAuthToken> GetTokenAsync();

        Task SaveTokenAsync(FirebaseAuthToken token);
    }

    public class InMemoryFirebaseTokenRepository : IFirebaseTokenRepository
    {
        private static InMemoryFirebaseTokenRepository instance;

        private FirebaseAuthToken token;

        private InMemoryFirebaseTokenRepository()
        {
        }

        public static InMemoryFirebaseTokenRepository Instance => instance ?? (instance = new InMemoryFirebaseTokenRepository());

        public Task SaveTokenAsync(FirebaseAuthToken token)
        {
            this.token = token;
            return Task.CompletedTask;
        }

        public Task<FirebaseAuthToken> GetTokenAsync()
        {
            return Task.FromResult(this.token);
        }
    }
}
