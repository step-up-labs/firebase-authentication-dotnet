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
        /// <returns> The <see cref="FirebaseAuth"/>. </returns>
        Task<FirebaseAuth> CreateUserWithEmailAndPassword(string email, string password);
        
        /// <summary>
        /// Sends user an email with a link to reset his password.
        /// </summary>
        /// <param name="email"> The email. </param>
        Task SendPasswordResetEmail(string email);

        /// <summary>
        /// Sign in user anonymously. He would still have a user id and access token generated, but name and other personal user properties will be null.
        /// </summary>
        /// <returns> The <see cref="FirebaseAuth"/>. </returns>
        Task<FirebaseAuth> SignInAnonymously();

        /// <summary>
        /// Using the provided email and password, get the firebase auth with token and basic user credentials.
        /// </summary>
        /// <param name="email"> The email. </param>
        /// <param name="password"> The password. </param>
        /// <returns> The <see cref="FirebaseAuth"/>. </returns>
        Task<FirebaseAuth> SignInWithEmailAndPassword(string email, string password);

        /// <summary>
        /// Using the provided access token from third party auth provider (google, facebook...), get the firebase auth with token and basic user credentials.
        /// </summary>
        /// <param name="authType"> The auth type. </param>
        /// <param name="oauthAccessToken"> The access token retrieved from login provider of your choice. </param>
        /// <returns> The <see cref="FirebaseAuth"/>. </returns>
        Task<FirebaseAuth> SignInWithOAuth(FirebaseAuthType authType, string oauthAccessToken);
    }
}