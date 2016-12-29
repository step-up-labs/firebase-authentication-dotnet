namespace Firebase.Auth
{
    using System.Threading.Tasks;

    /// <summary>
    /// The auth token provider.
    /// </summary>
    public interface IFirebaseAuthProvider
    {
        /// <summary>
        /// Creates new user with given credentials.
        /// </summary>
        /// <param name="email"> The email. </param>
        /// <param name="password"> The password. </param>
        /// <param name="displayName"> Optional display name. </param>
        /// <returns> The <see cref="FirebaseAuthLink"/>. </returns>
        Task<FirebaseAuthLink> CreateUserWithEmailAndPasswordAsync(string email, string password, string displayName = "");
        
        /// <summary>
        /// Sends user an email with a link to reset his password.
        /// </summary>
        /// <param name="email"> The email.  </param>
        /// <returns> The <see cref="Task"/>. </returns>
        Task SendPasswordResetEmailAsync(string email);

        /// <summary>
        /// Sign in user anonymously. He would still have a user id and access token generated, but name and other personal user properties will be null.
        /// </summary>
        /// <returns> The <see cref="FirebaseAuthLink"/>. </returns>
        Task<FirebaseAuthLink> SignInAnonymouslyAsync();

        /// <summary>
        /// Using the provided email and password, get the firebase auth with token and basic user credentials.
        /// </summary>
        /// <param name="email"> The email. </param>
        /// <param name="password"> The password. </param>
        /// <returns> The <see cref="FirebaseAuthLink"/>. </returns>
        Task<FirebaseAuthLink> SignInWithEmailAndPasswordAsync(string email, string password);

        /// <summary>
        /// Using the provided access token from third party auth provider (google, facebook...), get the firebase auth with token and basic user credentials.
        /// </summary>
        /// <param name="authType"> The auth type. </param>
        /// <param name="oauthAccessToken"> The access token retrieved from login provider of your choice. </param>
        /// <returns> The <see cref="FirebaseAuthLink"/>. </returns>
        Task<FirebaseAuthLink> SignInWithOAuthAsync(FirebaseAuthType authType, string oauthAccessToken);

        /// <summary>
        /// Sign in with a custom token. You would usually create and sign such a token on your server to integrate with your existing authentiocation system.
        /// </summary>
        /// <param name="customToken"> The access token retrieved from login provider of your choice. </param>
        /// <returns> The <see cref="FirebaseAuth"/>. </returns>
        Task<FirebaseAuthLink> SignInWithCustomTokenAsync(string customToken);

        /// <summary>
        /// Links the authenticated user represented by <see cref="auth"/> with an email and password. 
        /// </summary>
        /// <param name="auth"> The authenticated user to link with specified email and password. </param>
        /// <param name="email"> The email. </param>
        /// <param name="password"> The password. </param>
        /// <returns> The <see cref="FirebaseAuthLink"/>. </returns>
        Task<FirebaseAuthLink> LinkAccountsAsync(FirebaseAuth auth, string email, string password);

        /// <summary>
        /// Links the authenticated user represented by <see cref="auth"/> with and account from a third party provider.
        /// </summary>
        /// <param name="auth"> The auth. </param>
        /// <param name="authType"> The auth type.  </param>
        /// <param name="oauthAccessToken"> The access token retrieved from login provider of your choice. </param>
        /// <returns> The <see cref="FirebaseAuthLink"/>.  </returns>
        Task<FirebaseAuthLink> LinkAccountsAsync(FirebaseAuth auth, FirebaseAuthType authType, string oauthAccessToken);
        
        /// <summary>
        /// Gets a list of accounts linked to given email.
        /// </summary>
        /// <param name="email"> Email address. </param>
        /// <returns> The <see cref="ProviderQueryResult"/></returns>
        Task<ProviderQueryResult> GetLinkedAccountsAsync(string email);
    }
}