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
        private readonly CreateAuthUri createAuthUri;
        
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
            this.createAuthUri = new CreateAuthUri(this.config);

            foreach (var provider in this.config.Providers)
            {
                provider.Initialize(this.config);
            }

            this.config.UserManager.UserChanged += (s, e) => this.TriggerAuthStateChanged(this.authStateChanged, e.User);

            var user = this.config.UserManager.GetUser();

            // initialize user 
            if (user.info != null)
            {
                User = new User(this.config, user.info, user.credential);
            }
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

                // for every new listener trigger the AuthStateChanged event
                var user = this.config.UserManager.GetUser();

                if (user.info == null)
                {
                    this.TriggerAuthStateChanged(value, null);
                }
                else
                {
                    this.TriggerAuthStateChanged(value, new User(this.config, user.info, user.credential));
                }
                
            }
            remove
            {
                this.authStateChanged -= value;
            }
        }

        public async Task<UserCredential> SignInWithRedirectAsync(FirebaseProviderType authType, SignInRedirectDelegate redirectDelegate)
        {
            var provider = this.config.GetAuthProvider(authType);
            
            if (!(provider is OAuthProvider oauthProvider))
            {
                throw new InvalidOperationException("You cannot sign in with this provider using this method.");
            }

            await this.CheckAuthDomain();

            var continuation = await oauthProvider.SignInAsync();
            var redirectUri = await redirectDelegate(continuation.Uri).ConfigureAwait(false);

            if (string.IsNullOrEmpty(redirectUri))
            {
                return null;
            }

            var userCredential = await continuation.ContinueSignInAsync(redirectUri).ConfigureAwait(false);

            this.SaveToken(userCredential.User);

            return userCredential;
        }

        public async Task<UserCredential> SignInWithCredentialAsync(AuthCredential credential)
        {
            await this.CheckAuthDomain().ConfigureAwait(false);
            
            var userCredential = await this.config
                .GetAuthProvider(credential.ProviderType)
                .SignInWithCredentialAsync(credential);

            this.SaveToken(userCredential.User);

            return userCredential;
        }

        public async Task<UserCredential> SignInAnonymouslyAsync()
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

            this.SaveToken(user);

            return new UserCredential(user, null, OperationType.SignIn);
        }

        public async Task<FetchUserProvidersResult> FetchSignInMethodsForEmailAsync(string email)
        {
            await this.CheckAuthDomain().ConfigureAwait(false);

            var request = new CreateAuthUriRequest
            {
                ContinueUri = this.config.RedirectUri,
                Identifier = email
            };

            var response = await this.createAuthUri.ExecuteAsync(request).ConfigureAwait(false);

            return new FetchUserProvidersResult(email, response.Registered, response.SigninMethods, response.AllProviders);
        }

        public async Task<UserCredential> SignInWithEmailAndPasswordAsync(string email, string password)
        {
            await this.CheckAuthDomain().ConfigureAwait(false);

            var provider = (EmailProvider)this.config.GetAuthProvider(FirebaseProviderType.EmailAndPassword);
            var result = await provider.SignInUserAsync(email, password).ConfigureAwait(false);

            this.SaveToken(result.User);

            return result;
        }

        public async Task<UserCredential> CreateUserWithEmailAndPasswordAsync(string email, string password, string displayName = null)
        {
            await this.CheckAuthDomain().ConfigureAwait(false);

            var provider = (EmailProvider)this.config.GetAuthProvider(FirebaseProviderType.EmailAndPassword);
            var result = await provider.SignUpUserAsync(email, password, displayName).ConfigureAwait(false);

            this.SaveToken(result.User);

            return result;
        }

        public async Task ResetEmailPasswordAsync(string email)
        {
            await this.CheckAuthDomain().ConfigureAwait(false);

            var provider = (EmailProvider)this.config.GetAuthProvider(FirebaseProviderType.EmailAndPassword);
            await provider.ResetEmailPasswordAsync(email).ConfigureAwait(false);
        }

        public void SignOut()
        {
            var uid = this.User?.Uid;
            this.User = null;
            this.config.UserManager.DeleteExistingUser(uid);
        }

        private void TriggerAuthStateChanged(EventHandler<UserEventArgs> value, User user)
        {
            this.User = user;
            value?.Invoke(this, new UserEventArgs(user));
        }

        private void SaveToken(User user)
        {
            this.config.UserManager.SaveNewUser(user);
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
