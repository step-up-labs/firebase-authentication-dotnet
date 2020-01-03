using Firebase.Auth.Providers;
using Firebase.Auth.Requests;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Firebase.Auth
{
    /// <inherit />
    public class FirebaseAuthClient : IFirebaseAuthClient
    {
        private readonly FirebaseAuthConfig config;
        private readonly ProjectConfig projectConfig;
        private readonly SignupNewUser signupNewUser;

        private bool domainChecked;
        private event EventHandler<UserEventArgs> authStateChanged;

        public FirebaseAuthClient(FirebaseAuthConfig config)
        {
            if (string.IsNullOrWhiteSpace(config?.ApiKey))
            {
                throw new ArgumentException($"API Key must be set.");
            }

            if (string.IsNullOrWhiteSpace(config?.AuthDomain))
            {
                throw new ArgumentException($"Auth domain must be set.");
            }

            this.config = config;
            this.projectConfig = new ProjectConfig(this.config);
            this.signupNewUser = new SignupNewUser(this.config);

            foreach (var provider in this.config.Providers)
            {
                provider.Initialize(this.config);
            }

            this.config.UserRepository.UserChanged += (s, e) => this.TriggerAuthStateChanged(this.authStateChanged, e.User);
        }

        public User User
        {
            get;
            private set;
        }

        public event EventHandler<UserEventArgs> AuthStateChanged
        {
            add
            {
                this.authStateChanged += value;
                if (this.User == null)
                {
                    this.config.UserRepository.GetUserAsync().ContinueWith(t =>
                    {
                        if (t.Result.Item1 == null)
                        {
                            this.TriggerAuthStateChanged(value, null);
                        }
                        else
                        {
                            this.TriggerAuthStateChanged(value, new User(this.config, t.Result.info, t.Result.credential));
                        }
                    });
                }
            }
            remove
            {
                this.authStateChanged -= value;
            }
        }

        public async Task<User> SignInWithRedirectAsync(FirebaseProviderType authType, SignInRedirectDelegate redirectDelegate)
        {
            var provider = this.GetAuthProvider(authType);

            if (!(provider is OAuthProvider oauthProvider))
            {
                throw new InvalidOperationException("You cannot sign in with this provider using this method.");
            }

            await this.CheckAuthDomain().ConfigureAwait(false);

            var continuation = await oauthProvider.SignInAsync().ConfigureAwait(false);
            var redirectUri = await redirectDelegate(continuation.Uri).ConfigureAwait(false);
            var user = await continuation.ContinueSignInAsync(redirectUri).ConfigureAwait(false);

            await this.SaveTokenAsync(user).ConfigureAwait(false);

            return user;
        }

        public async Task<User> SignInWithCredentialAsync(AuthCredential credential)
        {
            await this.CheckAuthDomain().ConfigureAwait(false);
            
            return await this
                .GetAuthProvider(credential.ProviderType)
                .SignInWithCredentialAsync(credential);
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

        public async Task ResetEmailPasswordAsync(string email)
        {
            await this.CheckAuthDomain().ConfigureAwait(false);

            var provider = (EmailProvider)this.GetAuthProvider(FirebaseProviderType.EmailAndPassword);
            await provider.ResetEmailPasswordAsync(email).ConfigureAwait(false);
        }

        public async Task SignOutAsync()
        {
            await this.config.UserRepository.SaveUserAsync(null);
            this.User = null;
            this.authStateChanged?.Invoke(this, new UserEventArgs(null));
        }

        private void TriggerAuthStateChanged(EventHandler<UserEventArgs> value, User user)
        {
            this.User = user;
            value?.Invoke(this, new UserEventArgs(user));
        }

        private async Task SaveTokenAsync(User user)
        {
            await this.config.UserRepository.SaveUserAsync(user);
        }

        private FirebaseAuthProvider GetAuthProvider(FirebaseProviderType authType)
        {
            return this.config.Providers.FirstOrDefault(f => f.ProviderType == authType)
                ?? throw new InvalidOperationException($"Provider {authType} is not configured, you need to add it to your FirebaseAuthConfig");
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
}
