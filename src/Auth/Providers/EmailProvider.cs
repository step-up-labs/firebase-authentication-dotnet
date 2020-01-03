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
        private ResetPassword resetPassword;
        private VerifyAssertion verifyAssertion;

        public override FirebaseProviderType ProviderType => FirebaseProviderType.EmailAndPassword;

        internal override void Initialize(FirebaseAuthConfig config)
        {
            base.Initialize(config);

            this.signupNewUser = new SignupNewUser(this.config);
            this.setAccountInfo = new SetAccountInfo(this.config);
            this.getAccountInfo = new GetAccountInfo(this.config);
            this.verifyPassword = new VerifyPassword(this.config);
            this.resetPassword = new ResetPassword(this.config);
            this.verifyAssertion = new VerifyAssertion(this.config);
        }

        public static AuthCredential GetCredential(string email, string password)
        {
            return new AuthCredential
            {
                ProviderType = FirebaseProviderType.EmailAndPassword,
                Object = new EmailCredential
                {
                    Email = email,
                    Password = password
                }
            };
        }

        public async Task<CheckUserRessult> CheckUserExistsAsync(string email)
        {
            var request = new CreateAuthUriRequest
            {
                ContinueUri = this.GetContinueUri(),
                Identifier = email
            };

            var response = await this.createAuthUri.ExecuteAsync(request).ConfigureAwait(false);

            return new CheckUserRessult(email, response.Registered, response.SigninMethods);
        }

        public Task ResetEmailPasswordAsync(string email)
        {
            var request = new ResetPasswordRequest
            {
                Email = email
            };

            return this.resetPassword.ExecuteAsync(request);
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
                var setResponse = await this.setAccountInfo.ExecuteAsync(new SetAccountDisplayName
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

        protected internal override Task<User> SignInWithCredentialAsync(AuthCredential credential)
        {
            var ep = (EmailCredential)credential.Object;

            return this.SignInUserAsync(ep.Email, ep.Password);
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

        internal class EmailCredential
        {
            public string Email { get; set; }

            public string Password { get; set; }
        }
    }

    public class CheckUserRessult
    {
        public CheckUserRessult(string email, bool exists, IReadOnlyCollection<FirebaseProviderType> signinProviders)
        {
            this.Email = email;
            this.UserExists = exists;
            this.SigninProviders = signinProviders;
        }

        public string Email { get; }

        public bool UserExists { get; }

        public IReadOnlyCollection<FirebaseProviderType> SigninProviders { get; }
    }
}
