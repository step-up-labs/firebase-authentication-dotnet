namespace Firebase.Auth.UI.Converters
{
    /// <summary>
    /// Convertor of <see cref="FirebaseProviderType"/> to hexa color of button foreground.
    /// </summary>
    public class ProviderToForegroundConverter
    {
        public static string Convert(FirebaseProviderType provider)
        {
            switch (provider)
            {
                case FirebaseProviderType.Google: return "#757575";
                default:
                    return "#ffffff";
            }
        }
    }
}
