using Firebase.Auth.Providers;
using Firebase.Auth.Repository;
using Firebase.Auth.UI;
using Microsoft.Extensions.Logging;

namespace Auth.MAUI.Sample
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            // Force override culture & language
            //CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("cs");
            //CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("cs");

            // Firebase UI initialization
            FirebaseUI.Initialize(new FirebaseUIConfig
            {
                ApiKey = "<YOUR API KEY>",
                AuthDomain = "<YOUR PROJECT DOMAIN>.firebaseapp.com",
                Providers = new FirebaseAuthProvider[]
                {
                    new GoogleProvider(),
                    new FacebookProvider(),
                    new AppleProvider(),
                    new TwitterProvider(),
                    new GithubProvider(),
                    new MicrosoftProvider(),
                    new EmailProvider()
                },
                PrivacyPolicyUrl = "https://github.com/step-up-labs/firebase-authentication-dotnet",
                TermsOfServiceUrl = "https://github.com/step-up-labs/firebase-database-dotnet",
                IsAnonymousAllowed = true,
                AutoUpgradeAnonymousUsers = true,
                UserRepository = new FileUserRepository("FirebaseSample"),
                // Func called when upgrade of anonymous user fails because the user already exists
                // You should grab any data created under your anonymous user, sign in with the pending credential
                // and copy the existing data to the new user
                // see details here: https://github.com/firebase/firebaseui-web#upgrading-anonymous-users
                AnonymousUpgradeConflict = conflict => conflict.SignInWithPendingCredentialAsync(true)
            });

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
