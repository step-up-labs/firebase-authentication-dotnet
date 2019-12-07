namespace Firebase.Auth.Providers
{
    public class MicrosoftProvider : ExternalAuthProvider
    {
        public static string[] DefaultScopes = new[] 
        {
            "profile",
            "email",
            "openid",
            "User.Read",
        };

        public MicrosoftProvider()
        {
            this.AddScopes(DefaultScopes);
        }

        public override FirebaseProviderType AuthType => FirebaseProviderType.Microsoft;
    }
}
