using Firebase.Auth.Providers;
using Firebase.Auth.Repository;
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
                ApiKey = "AIzaSyCfMEZut1bOgu9d1NHrJiZ7ruRdzfKEHbk",
                AuthDomain = "settle-up-sandbox.firebaseapp.com",
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
        }
    }
}
