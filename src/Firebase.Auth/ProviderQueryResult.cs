namespace Firebase.Auth
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System.Collections.Generic;

    /// <summary>
    /// More info at <see cref="https://developers.google.com/identity/toolkit/web/reference/relyingparty/createAuthUri"/>.
    /// </summary>
    public class ProviderQueryResult
    {
        internal ProviderQueryResult()
        {
            this.Providers = new List<FirebaseAuthType>();
        }

        public string Email
        {
            get;
            set;
        }

        [JsonProperty("registered")]
        public bool IsRegistered
        {
            get;
            set;
        }

        [JsonProperty("forExistingProvider")]
        public bool IsForExistingProvider
        {
            get;
            set;
        }

        [JsonProperty("authUri")]
        public string AuthUri
        {
            get;
            set;
        }

        [JsonProperty("providerId")]
        [JsonConverter(typeof(StringEnumConverter))]
        public FirebaseAuthType? ProviderId
        {
            get;
            set;
        }

        [JsonProperty("allProviders", ItemConverterType = typeof(StringEnumConverter))]
        public List<FirebaseAuthType> Providers
        {
            get;
            set;
        }
    }
}
