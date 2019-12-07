using Newtonsoft.Json;
using System;

namespace Firebase.Auth.Requests
{
    public class GetAccountInfoRequest
    {
        public string IdToken { get; set; }
    }

    public class GetAccountInfoResponse
    {
        public UserInfo[] Users { get; set; }
    }

    public class UserInfo
    {
        public string LocalId { get; set; }
        
        public string Email { get; set; }
        
        public string DisplayName { get; set; }
        
        public string PhotoUrl { get; set; }
        
        public bool EmailVerified { get; set; }
        
        public ProviderUserInfo[] ProviderUserInfo { get; set; }

        [JsonConverter(typeof(JavaScriptDateTimeConverter))]
        public DateTime ValidSince { get; set; }

        [JsonConverter(typeof(JavaScriptDateTimeConverter))]
        public DateTime LastLoginAt { get; set; }

        [JsonConverter(typeof(JavaScriptDateTimeConverter))]
        public DateTime CreatedAt { get; set; }

        public DateTime LastRefreshAt { get; set; }
    }

    public class ProviderUserInfo
    {
        public FirebaseProviderType ProviderId { get; set; }

        public string DisplayName { get; set; }
        
        public string PhotoUrl { get; set; }
        
        public string FederatedId { get; set; }
        
        public string Email { get; set; }
        
        public string RawId { get; set; }
    }

    public class GetAccountInfo : FirebaseRequestBase<GetAccountInfoRequest, GetAccountInfoResponse>
    {
        public GetAccountInfo(FirebaseAuthConfig config) : base(config)
        {
        }

        protected override string UrlFormat => Endpoints.GoogleGetUser;
    }

}
