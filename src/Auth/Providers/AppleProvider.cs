namespace Firebase.Auth.Providers
{
    public class AppleProvider : OAuthProvider
    {
        public const string DefaultEmailScope = "email";

        public AppleProvider()
        {
            this.AddScopes(DefaultEmailScope);
        }

        public static AuthCredential GetCredential(string accessToken) => GetCredential(FirebaseProviderType.Apple, accessToken, OAuthCredentialTokenType.AccessToken);

        public override FirebaseProviderType ProviderType => FirebaseProviderType.Apple;

        protected override string LocaleParameterName => "locale";
    }
}
