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

        private bool domainChecked;
        
        public FirebaseAuthClient(FirebaseAuthConfig config)
        {
            this.config = config;
            this.projectConfig = new ProjectConfig(this.config);

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
            var (assertion, account) = await continuation.ContinueSignInAsync(redirectUri).ConfigureAwait(false);
            var user = account.Users[0];

            return new User
            {
                DisplayName = user.DisplayName,
                FirstName = assertion.FirstName,
                LastName = assertion.LastName,
                Email = assertion.Email,
                IsEmailVerified = assertion.EmailVerified,
                FederatedId = assertion.FederatedId,
                LocalId = assertion.LocalId,
                PhotoUrl = assertion.PhotoUrl
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

            return user;
        }
        
        public async Task<User> CreateUserWithEmailAndPasswordAsync(string email, string password, string displayName = null)
        {
            await this.CheckAuthDomain().ConfigureAwait(false);

            var provider = (EmailProvider)this.GetAuthProvider(FirebaseProviderType.EmailAndPassword);
            var (user, token) = await provider.SignUpUserAsync(email, password, displayName).ConfigureAwait(false);

            return user;
        }

        //public async Task<string> GetFreshAccessToken()
        //{

        //}

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
}
