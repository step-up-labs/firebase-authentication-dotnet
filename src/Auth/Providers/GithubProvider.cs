namespace Firebase.Auth.Providers
{
    public class GithubProvider : OAuthProvider
    {
        public static AuthCredential GetCredential(string accessToken, string tenantId = null) => GetCredential(FirebaseProviderType.Github, accessToken, tenantId, OAuthCredentialTokenType.AccessToken);

        public override FirebaseProviderType ProviderType => FirebaseProviderType.Github;
    }
}
