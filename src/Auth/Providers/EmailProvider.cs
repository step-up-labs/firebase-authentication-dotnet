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

        public Task<UserCredential> SignInUserAsync(string email, string password)
        {
            return this.SignInWithCredentialAsync(GetCredential(email, password));
        }

        public async Task<UserCredential> SignUpUserAsync(string email, string password, string displayName)
        {
            var authCredential = GetCredential(email, password);
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

                return new UserCredential(new User(this.config, setUser, credential), authCredential, OperationType.SignIn);
            }

            var getUser = await this.GetUserInfoAsync(signupResponse.IdToken).ConfigureAwait(false);

            return new UserCredential(new User(this.config, getUser, credential), authCredential, OperationType.SignIn);
        }

        protected internal override async Task<UserCredential> SignInWithCredentialAsync(AuthCredential credential)
        {
            var ec = (EmailCredential)credential;

            var response = await this.verifyPassword.ExecuteAsync(new VerifyPasswordRequest
            {
                Email = ec.Email,
                Password = ec.Password,
                ReturnSecureToken = true
            }).ConfigureAwait(false);

            var user = await this.GetUserInfoAsync(response.IdToken).ConfigureAwait(false);
            var fc = new FirebaseCredential
            {
                ExpiresIn = response.ExpiresIn,
                IdToken = response.IdToken,
                RefreshToken = response.RefreshToken,
                ProviderType = FirebaseProviderType.EmailAndPassword
            };

            return new UserCredential(new User(this.config, user, fc), ec, OperationType.SignIn);
        }

        protected internal override async Task<UserCredential> LinkWithCredentialAsync(string token, AuthCredential credential)
        {
            var c = (EmailCredential)credential;
            var request = new SetAccountLinkRequest
            {
                IdToken = token,
                Email = c.Email,
                Password = c.Password,
                ReturnSecureToken = true
            };

            SetAccountLinkResponse link;

            try
            {
                link = await this.linkAccount.ExecuteAsync(request).ConfigureAwait(false);
            }
            catch (FirebaseAuthException e) when (e.Reason == AuthErrorReason.EmailExists)
            {
                throw new FirebaseAuthWithCredentialException("The email address is already in use by another account.", credential, AuthErrorReason.EmailExists);
            }

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

            return new UserCredential(new User(this.config, info, fc), credential, OperationType.Link);
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
