namespace Firebase.Auth.Providers
{
    public class AppleProvider : ExternalAuthProvider
    {
        public const string DefaultEmailScope = "email";

        public AppleProvider()
        {
            this.AddScopes(DefaultEmailScope);
        }

        public override FirebaseProviderType ProviderType => FirebaseProviderType.Apple;

        protected override string LocaleParameterName => "locale";
    }
}
