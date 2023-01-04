namespace Firebase.Auth.UI.Converters
{
    /// <summary>
    /// Convertor of <see cref="FirebaseProviderType"/> to PNG file asset path.
    /// </summary>
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
