using Firebase.Auth.Providers;
using Firebase.Auth.Repository;
using Firebase.Auth.UI;
using System.Globalization;
using Windows.ApplicationModel.Activation;
using Windows.Globalization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Auth.UWP.Sample
{
    sealed partial class App : Application
    {
        public App()
        {
            this.InitializeComponent();

            // Force override culture & language
            ApplicationLanguages.PrimaryLanguageOverride = "cs";

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
                UserRepository = new StorageRepository(),
                // Func called when upgrade of anonymous user fails because the user already exists
                // You should grab any data created under your anonymous user, sign in with the pending credential
                // and copy the existing data to the new user
                // see details here: https://github.com/firebase/firebaseui-web#upgrading-anonymous-users
                AnonymousUpgradeConflict = conflict => conflict.SignInWithPendingCredentialAsync(true)
            });
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            var rootFrame = Window.Current.Content as Frame;

            if (rootFrame == null)
            {
                rootFrame = new Frame();

                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }

                Window.Current.Activate();
            }
        }
    }
}
