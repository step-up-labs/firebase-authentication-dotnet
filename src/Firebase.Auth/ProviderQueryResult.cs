namespace Firebase.Auth
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System.Collections.Generic;

    public class ProviderQueryResult
    {
        internal ProviderQueryResult()
        {
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

        [JsonProperty("allProviders", ItemConverterType = typeof(StringEnumConverter))]
        public List<FirebaseAuthType> Providers
        {
            get;
            set;
        }
    }
}
