using System;

namespace Firebase.Auth
{
    /// <summary>
    /// Firebase credentials used to make Firebase requests.
    /// </summary>
    public class FirebaseCredential
    {
        public FirebaseCredential()
        {
            this.Created = DateTime.Now;
        }

        /// <summary>
        /// Value of the token to be used with Firebase requests.
        /// </summary>
        public string IdToken { get; set; }

        /// <summary>
        /// Value of the refresh token which can be used to refresh the <see cref="IdToken"/>.
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// Specifies when the token was created.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Specifies in how many second the token expires in from the moment it was created.
        /// </summary>
        public int ExpiresIn { get; set; }

        /// <summary>
        /// Type of the firebase auth provider.
        /// </summary>
        public FirebaseProviderType ProviderType { get; set; }

        /// <summary>
        /// Specifies whether the token already expired. 
        /// </summary>
        public bool IsExpired()
        {
            // include a small 10s window when the token is technically valid but it's a good idea to refresh it already.
            return DateTime.Now > this.Created.AddSeconds(this.ExpiresIn - 10);
        }
    }
}
