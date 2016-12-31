namespace Firebase.Auth
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// The firebase auth which can be linked to another credentials.
    /// </summary>
    public class FirebaseAuthLink : FirebaseAuth
    {
        internal FirebaseAuthLink()
        {
        }

        public FirebaseAuthLink(FirebaseAuthProvider authProvider, FirebaseAuth auth)
        {
            this.CopyPropertiesLocally(authProvider, auth);
        }

        public event EventHandler<FirebaseAuthEventArgs> FirebaseAuthRefreshed;

        internal FirebaseAuthProvider AuthProvider 
        {
            get;
            set;
        }

        /// <summary>
        /// Links the user with an email and password.  
        /// </summary>
        /// <param name="email"> The email. </param>
        /// <param name="password"> The password. </param>
        /// <returns> The <see cref="FirebaseAuthLink"/>. </returns>
        public async Task<FirebaseAuthLink> LinkToAsync(string email, string password)
        {
            var auth = await this.AuthProvider.LinkAccountsAsync(this, email, password).ConfigureAwait(false);

            this.CopyPropertiesLocally(auth.AuthProvider, auth);

            return this;
        }

        /// <summary>
        /// Links the this user with and account from a third party provider.
        /// </summary> 
        /// <param name="authType"> The auth type.  </param>
        /// <param name="oauthAccessToken"> The access token retrieved from login provider of your choice. </param>
        /// <returns> The <see cref="FirebaseAuthLink"/>.  </returns>
        public async Task<FirebaseAuthLink> LinkToAsync(FirebaseAuthType authType, string oauthAccessToken)
        {
            var auth = await this.AuthProvider.LinkAccountsAsync(this, authType, oauthAccessToken).ConfigureAwait(false);

            this.CopyPropertiesLocally(auth.AuthProvider, auth);

            return this;
        }

        public async Task<FirebaseAuthLink> GetFreshAuthAsync()
        {
            if (this.IsExpired())
            {
                var auth = await this.AuthProvider.RefreshAuthAsync(this).ConfigureAwait(false);
                this.CopyPropertiesLocally(auth.AuthProvider, auth);
                this.OnFirebaseAuthRefreshed(auth);
            }

            return this;
        }

        protected void OnFirebaseAuthRefreshed(FirebaseAuth auth)
        {
            this.FirebaseAuthRefreshed?.Invoke(this, new FirebaseAuthEventArgs(auth));
        }

        private void CopyPropertiesLocally(FirebaseAuthProvider authProvider, FirebaseAuth auth)
        {
            this.AuthProvider = authProvider;

            if (auth != null)
            {
                this.User = auth.User;
                this.Created = auth.Created;
                this.ExpiresIn = auth.ExpiresIn;
                this.RefreshToken = auth.RefreshToken;
                this.FirebaseToken = auth.FirebaseToken;
            }
        }
    }
}
