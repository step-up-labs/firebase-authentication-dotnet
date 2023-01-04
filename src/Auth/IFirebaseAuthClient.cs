using Firebase.Auth.Providers;
using System;
using System.Threading.Tasks;

namespace Firebase.Auth
{
    /// <summary>
    /// Firebase client which encapsulates communication with Firebase servers.
    /// </summary>
    public interface IFirebaseAuthClient
    {
        /// <summary>
        /// Currently signed in user.
        /// </summary>
        User User { get; }

        /// <summary>
        /// Event raised when the user auth state change changes. This can happen during sign in / sign out or when credential tokens change.
        /// </summary>
        event EventHandler<UserEventArgs> AuthStateChanged;

        /// <summary>
        /// Gets a list of sign-in methods for given email. If there are no methods, it means the user with given email doesn't exist.
        /// </summary>
        Task<FetchUserProvidersResult> FetchSignInMethodsForEmailAsync(string email);
        
        /// <summary>
        /// Creates a new user with given email, password and display name (optional) and signs this user in.
        /// </summary>
        Task<UserCredential> CreateUserWithEmailAndPasswordAsync(string email, string password, string displayName = null);
        
        /// <summary>
        /// Signs in as an anonymous user.
        /// </summary>
        Task<UserCredential> SignInAnonymouslyAsync();

        /// <summary>
        /// Signs in via third party OAuth providers - e.g. Google, Facebook etc.
        /// </summary>
        /// <param name="authType"> Type of the provider, must be an oauth one. </param>
        /// <param name="redirectDelegate"> Delegate which should invoke the passed uri for oauth authentication and return the final redirect uri. </param>
        Task<UserCredential> SignInWithRedirectAsync(FirebaseProviderType authType, SignInRedirectDelegate redirectDelegate);
        
        /// <summary>
        /// Signs in with email and password. If the email &amp; password combination is incorrect, <see cref="FirebaseAuthException"/> is thrown.
        /// </summary>
        Task<UserCredential> SignInWithEmailAndPasswordAsync(string email, string password);

        /// <summary>
        /// Sign in with platform specific credential. For example:
        /// <code>
        /// var credential = GoogleProvider.GetCredential("token");
        /// </code>
        /// </summary>
        Task<UserCredential> SignInWithCredentialAsync(AuthCredential credential);

        /// <summary>
        /// Sends a password reset email to given address.
        /// </summary>
        Task ResetEmailPasswordAsync(string email);

        /// <summary>
        /// Signs current user out.
        /// </summary>
        void SignOut();
    }
}