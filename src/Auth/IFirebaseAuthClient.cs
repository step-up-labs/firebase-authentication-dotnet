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
        /// Checks whether a user with given email exists.
        /// </summary>
        Task<CheckUserRessult> CheckUserEmailExistsAsync(string email);
        
        /// <summary>
        /// Creates a new user with given email, password and display name (optional) and signs this user in.
        /// </summary>
        Task<User> CreateUserWithEmailAndPasswordAsync(string email, string password, string displayName = null);
        
        /// <summary>
        /// Signs in as an anonymous user.
        /// </summary>
        Task<User> SignInAnonymouslyAsync();

        /// <summary>
        /// Signs in via third party OAuth providers - e.g. Google, Facebook etc.
        /// </summary>
        /// <param name="authType"> Type of the provider, must be an external one. </param>
        /// <param name="externalSignInDelegate"> Delegate which should invoke the passed uri for external authentication and return the final redirect uri. </param>
        Task<User> SignInExternallyAsync(FirebaseProviderType authType, ExternalSignInDelegate externalSignInDelegate);
        
        /// <summary>
        /// Signs in with email and password. If the email &amp; password combination is incorrect, <see cref="FirebaseAuthException"/> is thrown.
        /// </summary>
        Task<User> SignInWithEmailAndPasswordAsync(string email, string password);
        
        /// <summary>
        /// Signs user out.
        /// </summary>
        Task SignOutAsync();
    }
}