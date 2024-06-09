namespace Firebase.Auth.Providers
{
    public class MicrosoftProvider : OAuthProvider
    {
        public static string[] DefaultScopes = new[] 
        {
            "profile",
            "email",
            "openid",
            "User.Read",
        };

        public MicrosoftProvider()
        {
            this.AddScopes(DefaultScopes);
        }

        public static AuthCredential GetCredential(string accessToken, string tenantId = null) => GetCredential(FirebaseProviderType.Microsoft, accessToken, tenantId, OAuthCredentialTokenType.AccessToken);

        public override FirebaseProviderType ProviderType => FirebaseProviderType.Microsoft;
    }
}
