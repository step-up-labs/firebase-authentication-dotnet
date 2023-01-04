using System.Threading.Tasks;

namespace Firebase.Auth.UI
{
    /// <summary>
    /// Represents a platform specific UI flow for Firebase authentication. 
    /// </summary>
    public interface IFirebaseUIFlow
    {
        /// <summary>
        /// Reset the flow to the initiali login UI.
        /// </summary>
        void Reset();

        /// <summary>
        /// Do an oauth redirect sign in in webview / browser and return the final uri.
        /// </summary>
        /// <param name="provider"> <see cref="FirebaseProviderType"/> </param>
        /// <param name="redirectUri"> The uri to navigate user to. </param>
        Task<string> GetRedirectResponseUriAsync(FirebaseProviderType provider, string redirectUri);

        /// <summary>
        /// Get user's email. Used to determine if the user exists or not.
        /// </summary>
        Task<string> PromptForEmailAsync(string error = "");

        /// <summary>
        /// Get user's details. Used to create a new user via email.
        /// </summary>
        /// <param name="email"> Email the user entered in previous step. </param>
        /// <param name="error"> Error encountered in previous attempt. </param>
        Task<EmailUser> PromptForEmailPasswordNameAsync(string email, string error = "");

        /// <summary>
        /// Get user's password. Used to sign in an existing user via email.
        /// </summary>
        /// <param name="email"> Email the user entered in the previous step. </param>
        /// <param name="oauthEmailAttempt"> Specifies whether federated oauth sign-in was attempted. </param>
        /// <param name="error"> Error encountered in previous attempt. </param>
        Task<EmailPasswordResult> PromptForPasswordAsync(string email, bool oauthEmailAttempt, string error = "");

        /// <summary>
        /// Ask user to confirm email password reset.
        /// </summary>
        /// <param name="email"> Email the user entered in the previous step. </param>
        /// <param name="error"> Error encountered in previous attempt. </param>
        Task<object> PromptForPasswordResetAsync(string email, string error = "");

        /// <summary>
        /// Show user a message that password reset email has been sent.
        /// </summary>
        /// <param name="email"> Email where the reset message has been sent to. </param>
        Task ShowPasswordResetConfirmationAsync(string email);

        /// <summary>
        /// Show user a message that given email was already used to sign in with a different provider.
        /// </summary>
        /// <param name="email"> Email used to sign in. </param>
        /// <param name="providerType"> Provider previously used to sign in. </param>
        /// <returns> True whether user wants to continue to sign in, or false to cancel flow. </returns>
        Task<bool> ShowEmailProviderConflictAsync(string email, FirebaseProviderType providerType);
    }
}
