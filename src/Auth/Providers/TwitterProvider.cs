namespace Firebase.Auth.Providers
{
    public class TwitterProvider : OAuthProvider
    {
        public override FirebaseProviderType ProviderType => FirebaseProviderType.Twitter;

        protected override string LocaleParameterName => "lang";
    }
}
