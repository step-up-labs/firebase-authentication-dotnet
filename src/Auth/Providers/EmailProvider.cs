using Firebase.Auth.Requests;
using System.Linq;
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
        private SetAccountLink linkAccount;

        public override FirebaseProviderType ProviderType => FirebaseProviderType.EmailAndPassword;

        internal override void Initialize(FirebaseAuthConfig config)
        {
            base.Initialize(config);

            this.signupNewUser = new SignupNewUser(this.config);
            this.setAccountInfo = new SetAccountInfo(this.config);
            this.getAccountInfo = new GetAccountInfo(this.config);
            this.verifyPassword = new VerifyPassword(this.config);
            this.resetPassword = new ResetPassword(this.config);
            this.linkAccount = new SetAccountLink(config);
        }

        public static AuthCredential GetCredential(string email, string password)
        {
            return new EmailCredential
            {
                ProviderType = FirebaseProviderType.EmailAndPassword,
                Email = email,
                Password = password
            };
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
            var ep = (EmailCredential)credential;

            return this.SignInUserAsync(ep.Email, ep.Password);
        }

        protected internal override async Task<User> LinkWithCredentialAsync(string token, AuthCredential credential)
        {
            var c = (EmailCredential)credential;
            var request = new SetAccountLinkRequest
            {
                IdToken = token,
                Email = c.Email,
                Password = c.Password,
                ReturnSecureToken = true
            };

            var link = await this.linkAccount.ExecuteAsync(request).ConfigureAwait(false);
            var getResult = await this.getAccountInfo.ExecuteAsync(new IdTokenRequest { IdToken = link.IdToken }).ConfigureAwait(false);

            var u = getResult.Users[0];
            var info = new UserInfo
            {
                DisplayName = u.DisplayName,
                Email = u.Email,
                IsEmailVerified = u.EmailVerified,
                FederatedId = u.ProviderUserInfo?.FirstOrDefault(info => info.FederatedId != null)?.FederatedId,
                Uid = u.LocalId,
                PhotoUrl = u.PhotoUrl,
                IsAnonymous = false
            };

            var fc = new FirebaseCredential
            {
                ExpiresIn = link.ExpiresIn,
                IdToken = link.IdToken,
                ProviderType = credential.ProviderType,
                RefreshToken = link.RefreshToken
            };

            return new User(this.config, info, fc);
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

        internal class EmailCredential : AuthCredential
        {
            public string Email { get; set; }

            public string Password { get; set; }
        }
    }
}
