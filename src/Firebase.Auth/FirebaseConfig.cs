namespace Firebase.Auth
{
    /// <summary>
    /// The auth config. 
    /// </summary>
    public class FirebaseConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FirebaseConfig"/> class.
        /// </summary>
        /// <param name="apiKey"> The api key of your Firebase app. </param>
        public FirebaseConfig(string apiKey)
        {
            this.ApiKey = apiKey;
        }

        /// <summary>
        /// Gets or sets the api key of your Firebase app. 
        /// </summary>
        public string ApiKey 
        {
            get;
            set;
        }
    }
}
