namespace Firebase.Auth.Providers
{
    public class GithubProvider : ExternalAuthProvider
    {
        public override FirebaseProviderType AuthType => FirebaseProviderType.Github;
    }
}
