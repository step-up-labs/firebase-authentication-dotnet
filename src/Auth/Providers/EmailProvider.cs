using Firebase.Auth.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Firebase.Auth.Providers
{
    public class EmailProvider : FirebaseAuthProvider
    {
        private SignupNewUser signupNewUser;
        private SetAccountInfo setAccountInfo;
        private GetAccountInfo getAccountInfo;
        private VerifyPassword verifyPassword;

        public override FirebaseProviderType ProviderType => FirebaseProviderType.EmailAndPassword;

        internal override void Initialize(FirebaseAuthConfig config)
        {
            base.Initialize(config);

            this.signupNewUser = new SignupNewUser(this.config);
            this.setAccountInfo = new SetAccountInfo(this.config);
            this.getAccountInfo = new GetAccountInfo(this.config);
            this.verifyPassword = new VerifyPassword(this.config);
        }

        public async Task<CheckUserRessult> CheckUserExistsAsync(string email)
        {
            var request = new CreateAuthUriRequest
            {
                ContinueUri = this.GetContinueUri(),
                Identifier = email
            };

            var response = await this.createAuthUri.ExecuteAsync(request).ConfigureAwait(false);

            return new CheckUserRessult(response.Registered, response.SigninMethods);
        }

        public async Task<User> SignInUserAsync(string email, string password)
        {
            var response = await this.verifyPassword.ExecuteAsync(new VerifyPasswordRequest
            {
                Email = email,
                Password = password,
                ReturnSecureToken = true
            }).ConfigureAwait(false);

            var user = await this.GetUserInfoAsync(response.IdToken).ConfigureAwait(false);
            var credential = new FirebaseCredential
            {
                ExpiresIn = response.ExpiresIn,
                IdToken = response.IdToken,
                RefreshToken = response.RefreshToken,
                ProviderType = FirebaseProviderType.EmailAndPassword
            };

            return new User(this.config, user, credential);
        }

        public async Task<User> SignUpUserAsync(string email, string password, string displayName)
        {
            var signupResponse = await this.signupNewUser.ExecuteAsync(new SignupNewUserRequest
            {
                Email = email,
                Password = password,
                ReturnSecureToken = true
            }).ConfigureAwait(false);

            var credential = new FirebaseCredential
            {
                ExpiresIn = signupResponse.ExpiresIn,
                IdToken = signupResponse.IdToken,
                RefreshToken = signupResponse.RefreshToken,
                ProviderType = FirebaseProviderType.EmailAndPassword
            };

            // set display name if available
            if (!string.IsNullOrWhiteSpace(displayName))
            {
                var setResponse = await this.setAccountInfo.ExecuteAsync(new SetAccountInfoRequest
                {
                    DisplayName = displayName,
                    IdToken = signupResponse.IdToken,
                    ReturnSecureToken = true
                }).ConfigureAwait(false);

                var setUser = new UserInfo
                {
                    DisplayName = setResponse.DisplayName,
                    Email = setResponse.Email,
                    IsEmailVerified = setResponse.EmailVerified,
                    Uid = setResponse.LocalId,
                    IsAnonymous = false
                };

                return new User(this.config, setUser, credential);
            }

            var getUser = await this.GetUserInfoAsync(signupResponse.IdToken).ConfigureAwait(false);

            return new User(this.config, getUser, credential);
        }

        private async Task<UserInfo> GetUserInfoAsync(string idToken)
        {
            var getResponse = await this.getAccountInfo.ExecuteAsync(new IdTokenRequest { IdToken = idToken }).ConfigureAwait(false);
            var user = getResponse.Users[0];

            return new UserInfo
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                IsEmailVerified = user.EmailVerified,
                Uid = user.LocalId,
                PhotoUrl = user.PhotoUrl,
                IsAnonymous = false
            };
        }
    }

    public class CheckUserRessult
    {
        public CheckUserRessult(bool exists, IReadOnlyCollection<FirebaseProviderType> signinProviders)
        {
            this.UserExists = exists;
            this.SigninProviders = signinProviders;
        }

        public bool UserExists { get; }

        public IReadOnlyCollection<FirebaseProviderType> SigninProviders { get; }
    }
}
