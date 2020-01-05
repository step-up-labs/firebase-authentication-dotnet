using System.Threading.Tasks;

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

        public static AuthCredential GetCredential(string token, OAuthCredentialTokenType tokenType)
        {
            return new OAuthCredential
            {
                ProviderType = FirebaseProviderType.Google,
                Token = token,
                TokenType = tokenType
            };
        }

        public override FirebaseProviderType ProviderType => FirebaseProviderType.Google;

        protected override string LocaleParameterName => "hl";
    }
}
