using System;
using System.Threading.Tasks;

namespace Firebase.Auth.UI
{
    public class FirebaseUpgradeConflict
    {
        public FirebaseUpgradeConflict(IFirebaseAuthClient client, AuthCredential credential)
        {
            this.Client = client;
            this.PendingCredential = credential;
            this.AnonymousUser = client.User;
        }

        /// <summary>
        /// Pending credential which the user wants to sign in.
        /// </summary>
        public AuthCredential PendingCredential { get; }

        /// <summary>
        /// Instance of <see cref="FirebaseAuthClient"/>.
        /// </summary>
        public IFirebaseAuthClient Client { get; }

        /// <summary>
        /// Current anonymous user which couldn't be linked with existing <see cref="PendingCredential"/>.
        /// </summary>
        public User AnonymousUser { get; }

        /// <summary>
        /// Convenience method which signs in using the pending credentials and optionally deletes the current anonymous user
        /// </summary>
        public async Task<UserCredential> SignInWithPendingCredentialAsync(bool deleteAnonymousUser)
        {
            var c = await this.Client.SignInWithCredentialAsync(this.PendingCredential).ConfigureAwait(false);

            if (deleteAnonymousUser)
            {
                await this.AnonymousUser.DeleteAsync().ConfigureAwait(false);
            }

            return c;
        }
    }
}
