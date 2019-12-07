namespace Firebase.Auth.Providers
{
    public class TwitterProvider : ExternalAuthProvider
    {
        public override FirebaseProviderType AuthType => FirebaseProviderType.Twitter;
    }
}
