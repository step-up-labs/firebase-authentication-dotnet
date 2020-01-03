namespace Firebase.Auth.Providers
{
    public class GoogleProvider : OAuthProvider
    {
        public const string DefaultProfileScope = "profile";
        public const string DefaultEmailScope = "email";

        public GoogleProvider()
        {
            this.AddScopes(DefaultProfileScope, DefaultEmailScope);
        }

        public static AuthCredential GetCredential(string accessToken) => GetCredential(FirebaseProviderType.Google, accessToken);

        public override FirebaseProviderType ProviderType => FirebaseProviderType.Google;

        protected override string LocaleParameterName => "hl";
    }
}
