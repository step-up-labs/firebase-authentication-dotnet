using System;

namespace Firebase.Auth
{
    public class FirebaseAuthToken
    {
        public FirebaseAuthToken()
        {
            this.Created = DateTime.Now;
        }

        public string IdToken { get; set; }

        public string RefreshToken { get; set; }

        public DateTime Created { get; set; }

        /// <summary>
        /// Specifies in how many second the token expires in from the moment it was created.
        /// </summary>
        public int ExpiresIn { get; set; }

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
