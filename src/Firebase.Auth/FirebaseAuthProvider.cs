namespace Firebase.Auth
{
    using System;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    using Newtonsoft.Json;

    /// <summary>
    /// The auth token provider.
    /// </summary>
    public class FirebaseAuthProvider : IDisposable
    {
        private const string GoogleIdentityUrl = "https://www.googleapis.com/identitytoolkit/v3/relyingparty/verifyAssertion?key={0}";
        private const string GoogleAnonymousUrl = "https://www.googleapis.com/identitytoolkit/v3/relyingparty/signupNewUser?key={0}";

        private readonly FirebaseConfig authConfig;
        private readonly HttpClient client;

        /// <summary>
        /// Initializes a new instance of the <see cref="FirebaseAuthProvider"/> class.
        /// </summary>
        /// <param name="authConfig"> The auth config. </param>
        public FirebaseAuthProvider(FirebaseConfig authConfig)
        {
            this.authConfig = authConfig;
            this.client = new HttpClient();
        }

        /// <summary>
        /// Using the provided access token from third party auth provider (google, facebook...), get the firebase auth with token and basic user credentials.
        /// </summary>
        /// <param name="authType"> The auth type. </param>
        /// <param name="oauthAccessToken"> The access token retrieved from login provider of your choice. </param>
        /// <returns> The <see cref="FirebaseAuth"/>. </returns>
        public async Task<FirebaseAuth> SignInWithOAuth(FirebaseAuthType authType, string oauthAccessToken)
        {
            var providerId = this.GetProviderId(authType);
            var content = $"{{\"postBody\":\"access_token={oauthAccessToken}&providerId={providerId}\",\"requestUri\":\"http://localhost\",\"returnSecureToken\":true}}";

            return await this.SignInWithPostContent(GoogleIdentityUrl, content);
        }

        /// <summary>
        /// Sign in user anonymously. He would still have a user id and access token generated, but name and other personal user properties will be null.
        /// </summary>
        /// <returns> The <see cref="FirebaseAuth"/>. </returns>
        public async Task<FirebaseAuth> SignInAnonymously()
        {
            var content = $"{{\"returnSecureToken\":true}}";

            return await this.SignInWithPostContent(GoogleAnonymousUrl, content);
        }

        /// <summary>
        /// Disposes all allocated resources. 
        /// </summary>
        public void Dispose() 
        {
            this.client.Dispose();
        }

        private async Task<FirebaseAuth> SignInWithPostContent(string googleUrl, string postContent)
        {
            var response = await this.client.PostAsync(new Uri(string.Format(googleUrl, this.authConfig.ApiKey)), new StringContent(postContent, Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();

            var responseData = await response.Content.ReadAsStringAsync();
            var user = JsonConvert.DeserializeObject<User>(responseData);
            var auth = JsonConvert.DeserializeObject<FirebaseAuth>(responseData);

            auth.User = user;

            return auth;
        }

        private string GetProviderId(FirebaseAuthType authType)
        {
            switch (authType)
            {
                case FirebaseAuthType.Facebook:
                    return "facebook.com";
                case FirebaseAuthType.Google:
                    return "google.com";
                case FirebaseAuthType.Github:
                    return "github.com";
                case FirebaseAuthType.Twitter:
                    return "twitter.com";
                default: throw new NotImplementedException("");
            }
        }
    }
}
