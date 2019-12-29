using Firebase.Auth.Providers;
using Firebase.Auth.UI;
using System.Globalization;
using System.Windows;

namespace Firebase.Auth.Wpf.Sample
{
    public partial class App : Application
    {
        public App()
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
                UserRepository = new FileUserRepository("FirebaseSample")
            });
        }
    }
}
