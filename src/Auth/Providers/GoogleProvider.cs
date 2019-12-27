namespace Firebase.Auth.Providers
{
    public class GoogleProvider : ExternalAuthProvider
    {
        public const string DefaultProfileScope = "profile";

        public GoogleProvider()
        {
            this.AddScopes(DefaultProfileScope);
        }

        public override FirebaseProviderType ProviderType => FirebaseProviderType.Google;

        protected override string LocaleParameterName => "hl";
    }
}
