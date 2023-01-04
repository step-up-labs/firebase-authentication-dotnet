using Firebase.Auth.UI.Resources;

namespace Firebase.Auth.UI.Converters
{
    /// <summary>
    /// Convertor of <see cref="FirebaseProviderType"/> to localized title for the sign in button.
    /// </summary>
    public class ProviderToTitleConverter
    {
        public static string Convert(FirebaseProviderType provider)
        {
            switch (provider)
            {
                case FirebaseProviderType.Facebook:
                    return AppResources.Instance.FuiSignInWithFacebook;
                case FirebaseProviderType.Google:
                    return AppResources.Instance.FuiSignInWithGoogle;
                case FirebaseProviderType.Github:
                    return AppResources.Instance.FuiSignInWithGithub;
                case FirebaseProviderType.Twitter:
                    return AppResources.Instance.FuiSignInWithTwitter;
                case FirebaseProviderType.Microsoft:
                    return AppResources.Instance.FuiSignInWithMicrosoft;
                case FirebaseProviderType.Apple:
                    return AppResources.Instance.FuiSignInWithApple;
                case FirebaseProviderType.EmailAndPassword:
                    return AppResources.Instance.FuiSignInWithEmail;
                case FirebaseProviderType.Anonymous:
                    return AppResources.Instance.FuiSignInAnonymously;
                default:
                    return string.Empty;
            }
        }
    }
}
