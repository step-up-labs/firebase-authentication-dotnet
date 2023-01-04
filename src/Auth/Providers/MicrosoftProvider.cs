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

        public static AuthCredential GetCredential(string accessToken) => GetCredential(FirebaseProviderType.Microsoft, accessToken, OAuthCredentialTokenType.AccessToken);

        public override FirebaseProviderType ProviderType => FirebaseProviderType.Microsoft;
    }
}
