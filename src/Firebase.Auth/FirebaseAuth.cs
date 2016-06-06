namespace Firebase.Auth
{
    using Newtonsoft.Json;

    /// <summary>
    /// The firebase auth.
    /// </summary>
    public class FirebaseAuth
    {
        /// <summary>
        /// Gets or sets the firebase token which can be used for authenticated queries. 
        /// </summary>
        [JsonProperty("idToken")]
        public string FirebaseToken
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the numbers of seconds until the token expires.
        /// </summary>
        [JsonProperty("expiresIn")]
        public int ExpiresIn
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        public User User
        {
            get;
            set;
        }
    }
}
