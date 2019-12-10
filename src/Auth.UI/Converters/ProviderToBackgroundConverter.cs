namespace Firebase.Auth.UI.Converters
{
    public class ProviderToBackgroundConverter
    {
        public string Convert(FirebaseProviderType provider)
        {
            switch (provider)
            {
                case FirebaseProviderType.Facebook: return "#3b5998";
                case FirebaseProviderType.Google: return "#ffffff";
                case FirebaseProviderType.Github: return "#333333";
                case FirebaseProviderType.Twitter: return "#55acee";
                case FirebaseProviderType.Microsoft: return "#2f2f2f";
                case FirebaseProviderType.EmailAndPassword: return "#db4437";
                case FirebaseProviderType.Anonymous: return "#f4b400";
                default:
                    return string.Empty;
            }
        }
    }
}
