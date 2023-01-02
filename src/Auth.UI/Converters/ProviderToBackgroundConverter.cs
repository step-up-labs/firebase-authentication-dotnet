namespace Firebase.Auth.UI.Converters
{
    /// <summary>
    /// Convertor of <see cref="FirebaseProviderType"/> to hexa color of button background.
    /// </summary>
    public class ProviderToBackgroundConverter
    {
        public static string Convert(FirebaseProviderType provider)
        {
            switch (provider)
            {
                case FirebaseProviderType.Facebook: return "#3b5998";
                case FirebaseProviderType.Google: return "#f9f9f9";
                case FirebaseProviderType.Github: return "#333333";
                case FirebaseProviderType.Twitter: return "#55acee";
                case FirebaseProviderType.Microsoft: return "#2f2f2f";
                case FirebaseProviderType.Apple: return "#000000";
                case FirebaseProviderType.EmailAndPassword: return "#db4437";
                case FirebaseProviderType.Anonymous: return "#f4b400";
                default:
                    return "#000000";
            }
        }
    }
}
