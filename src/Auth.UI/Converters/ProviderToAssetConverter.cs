namespace Firebase.Auth.UI.Converters
{
    public class ProviderToAssetConverter
    {
        public static string Convert(FirebaseProviderType provider)
        {
            switch (provider)
            {
                case FirebaseProviderType.EmailAndPassword:
                    return "/Assets/mail.png";
                default:
                    return $"/Assets/{provider.ToString().ToLower()}.png";
            }
        }
    }
}
