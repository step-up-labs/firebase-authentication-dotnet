namespace Firebase.Auth
{
    using System.Threading.Tasks;

    /// <summary>
    /// The firebase auth which can be linked to another credentials.
    /// </summary>
    public class FirebaseAuthLink : FirebaseAuth
    {
        internal FirebaseAuthLink()
        {
        }

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
        public Task<FirebaseAuthLink> LinkToAsync(string email, string password)
        {
            return this.AuthProvider.LinkAccountsAsync(this, email, password);
        }

        /// <summary>
        /// Links the this user with and account from a third party provider.
        /// </summary> 
        /// <param name="authType"> The auth type.  </param>
        /// <param name="oauthAccessToken"> The access token retrieved from login provider of your choice. </param>
        /// <returns> The <see cref="FirebaseAuthLink"/>.  </returns>
        public Task<FirebaseAuthLink> LinkToAsync(FirebaseAuthType authType, string oauthAccessToken)
        {
            return this.AuthProvider.LinkAccountsAsync(this, authType, oauthAccessToken);
        }
    }
}
