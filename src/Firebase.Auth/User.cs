namespace Firebase.Auth
{
    using Newtonsoft.Json;

    /// <summary>
    /// Basic information about the logged in user.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Gets or sets the local id.
        /// </summary>
        [JsonProperty("localId")]
        public string LocalId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the federated id.
        /// </summary>
        [JsonProperty("federatedId")]
        public string FederatedId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        [JsonProperty("firstName")]
        public string FirstName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        [JsonProperty("lastName")]
        public string LastName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        [JsonProperty("displayName")]
        public string DisplayName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        [JsonProperty("email")]
        public string Email
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the photo url.
        /// </summary>
        [JsonProperty("photoUrl")]
        public string PhotoUrl
        {
            get;
            set;
        }
    }
}
