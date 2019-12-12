namespace Firebase.Auth.UI.Converters
{
    public class ProviderToAssetConverter
    {
        public static string Convert(FirebaseProviderType provider)
        {
            switch (provider)
            {
                case FirebaseProviderType.Facebook:
                case FirebaseProviderType.Google:
                case FirebaseProviderType.Github:
                case FirebaseProviderType.Twitter:
                case FirebaseProviderType.Microsoft:
                case FirebaseProviderType.Anonymous:
                    return $"/Assets/{provider.ToString().ToLower()}.png";
                case FirebaseProviderType.EmailAndPassword:
                    return "/Assets/mail.png";
                default:
                    return string.Empty;
            }
        }
    }
}
