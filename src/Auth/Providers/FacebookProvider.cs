namespace Firebase.Auth.Providers
{
    public class FacebookProvider : OAuthProvider
    {
        public const string DefaultEmailScope = "email";

        public FacebookProvider()
        {
            this.AddScopes(DefaultEmailScope);
        }

        public static AuthCredential GetCredential(string accessToken, string tenantId = null) => GetCredential(FirebaseProviderType.Facebook, accessToken, tenantId, OAuthCredentialTokenType.AccessToken);

        public override FirebaseProviderType ProviderType => FirebaseProviderType.Facebook;

        protected override string LocaleParameterName => "locale";
    }
}
