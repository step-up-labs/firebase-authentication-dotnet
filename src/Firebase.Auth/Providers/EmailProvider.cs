using Firebase.Auth.Requests;
using System;
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

        public override FirebaseProviderType AuthType => FirebaseProviderType.EmailAndPassword;

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

        public async Task<(User, FirebaseAuthToken)> SignInUserAsync(string email, string password)
        {
            var response = await this.verifyPassword.ExecuteAsync(new VerifyPasswordRequest
            {
                Email = email,
                Password = password,
                ReturnSecureToken = true
            }).ConfigureAwait(false);

            var user = await this.GetUserInfoAsync(response.IdToken).ConfigureAwait(false);
            var token = new FirebaseAuthToken
            {
                ExpiresIn = response.ExpiresIn,
                IdToken = response.IdToken,
                RefreshToken = response.RefreshToken
            };

            return (user, token);
        }

        public async Task<(User, FirebaseAuthToken)> SignUpUserAsync(string email, string password, string displayName)
        {
            var signupResponse = await this.signupNewUser.ExecuteAsync(new SignupNewUserRequest
            {
                Email = email,
                Password = password,
                ReturnSecureToken = true
            }).ConfigureAwait(false);

            var token = new FirebaseAuthToken
            {
                ExpiresIn = signupResponse.ExpiresIn,
                IdToken = signupResponse.IdToken,
                RefreshToken = signupResponse.RefreshToken
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

                var setUser = new User
                {
                    DisplayName = setResponse.DisplayName,
                    Email = setResponse.Email,
                    IsEmailVerified = setResponse.EmailVerified,
                    LocalId = setResponse.LocalId
                };

                return (setUser, token);
            }

            var getUser = await this.GetUserInfoAsync(signupResponse.IdToken).ConfigureAwait(false);

            return (getUser, token);
        }

        private async Task<User> GetUserInfoAsync(string idToken)
        {
            var getResponse = await this.getAccountInfo.ExecuteAsync(new GetAccountInfoRequest { IdToken = idToken }).ConfigureAwait(false);
            var user = getResponse.Users[0];

            return new User
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                IsEmailVerified = user.EmailVerified,
                LocalId = user.LocalId,
                PhotoUrl = user.PhotoUrl
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
