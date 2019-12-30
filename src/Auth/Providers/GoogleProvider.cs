namespace Firebase.Auth.Providers
{
    public class GoogleProvider : ExternalAuthProvider
    {
        public const string DefaultProfileScope = "profile";
        public const string DefaultEmailScope = "email";

        public GoogleProvider()
        {
            this.AddScopes(DefaultProfileScope, DefaultEmailScope);
        }

        public override FirebaseProviderType ProviderType => FirebaseProviderType.Google;

        protected override string LocaleParameterName => "hl";
    }
}
