namespace Firebase.Auth.Providers
{
    public class AppleProvider : OAuthProvider
    {
        public const string DefaultEmailScope = "email";

        public AppleProvider()
        {
            this.AddScopes(DefaultEmailScope);
        }

        public static AuthCredential GetCredential(string Token, OAuthCredentialTokenType TokenType) => GetCredential(FirebaseProviderType.Apple, Token, TokenType);

        public override FirebaseProviderType ProviderType => FirebaseProviderType.Apple;

        protected override string LocaleParameterName => "locale";
    }
}
