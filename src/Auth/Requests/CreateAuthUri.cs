using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace Firebase.Auth.Requests
{
    public class CreateAuthUriResponse
    {
        public string AuthUri { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public FirebaseProviderType ProviderId { get; set; }

        public string SessionId { get; set; }

        public bool Registered { get; set; }

        public List<FirebaseProviderType> SigninMethods { get; set; }
        
        public List<FirebaseProviderType> AllProviders { get; set; }
    }

    public class CreateAuthUriRequest
    {
        public FirebaseProviderType? ProviderId { get; set; }

        public string ContinueUri { get; set; }

        [JsonProperty("customParameter")]
        public Dictionary<string, string> CustomParameters { get; set; }
        
        public string OauthScope { get; set; }

        public string Identifier { get; set; }
    }

    /// <summary>
    /// Creates oauth authentication uri that user needs to navigate to in order to authenticate.
    /// </summary>
    public class CreateAuthUri : FirebaseRequestBase<CreateAuthUriRequest, CreateAuthUriResponse>
    {
        public CreateAuthUri(FirebaseAuthConfig config) : base(config)
        {
        }

        protected override string UrlFormat => Endpoints.GoogleCreateAuthUrl;
    }
}
