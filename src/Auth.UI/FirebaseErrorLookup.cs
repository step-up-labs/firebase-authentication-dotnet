using Firebase.Auth.UI.Resources;

namespace Firebase.Auth.UI
{
    public static class FirebaseErrorLookup
    {
        public static string LookupError(FirebaseAuthException exception)
        {
            return LookupError(exception.Reason);
        }

        public static string LookupError(AuthErrorReason reason)
        {
            switch (reason)
            {
                case AuthErrorReason.UserNotFound:
                    return AppResources.Instance.FuiErrorEmailDoesNotExist;
                case AuthErrorReason.InvalidEmailAddress:
                    return AppResources.Instance.FuiInvalidEmailAddress;
                case AuthErrorReason.MissingPassword:
                    return AppResources.Instance.FuiErrorInvalidPassword;
                case AuthErrorReason.WeakPassword:
                    return AppResources.Instance.FuiErrorWeakPasswordWithCount(6);
                case AuthErrorReason.EmailExists:
                    return AppResources.Instance.FuiEmailAccountCreationError;
                case AuthErrorReason.MissingEmail:
                    return AppResources.Instance.FuiMissingEmailAddress;
                case AuthErrorReason.UnknownEmailAddress:
                    return AppResources.Instance.FuiInvalidEmailAddress;
                case AuthErrorReason.WrongPassword:
                    return AppResources.Instance.FuiErrorInvalidPassword;
                case AuthErrorReason.Undefined:
                    return AppResources.Instance.FuiNoInternet;
                case AuthErrorReason.Unknown:
                    return AppResources.Instance.FuiErrorUnknown;
                default:
                    return $"{AppResources.Instance.FuiErrorUnknown} {reason}";
            }
        }
    }
}
