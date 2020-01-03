namespace Firebase.Auth.Providers
{
    public class GithubProvider : OAuthProvider
    {
        public static AuthCredential GetCredential(string accessToken) => GetCredential(FirebaseProviderType.Github, accessToken);

        public override FirebaseProviderType ProviderType => FirebaseProviderType.Github;
    }
}
