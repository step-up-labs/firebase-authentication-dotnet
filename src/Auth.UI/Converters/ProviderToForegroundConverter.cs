namespace Firebase.Auth.UI.Converters
{
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
