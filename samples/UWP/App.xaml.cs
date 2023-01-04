using Firebase.Auth.Providers;
using Firebase.Auth.Repository;
using Firebase.Auth.UI;
using Firebase.Auth;
using System;
using Windows.ApplicationModel.Activation;
using Windows.Globalization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Core;

namespace Auth.UWP.Sample
{
    sealed partial class App : Application
    {
        private Frame frame = new Frame();

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

        private async void AuthStateChanged(object sender, UserEventArgs e)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                async () =>
                {
                    if (e.User == null)
                    {
                        await FirebaseUI.Instance.Client.SignInAnonymouslyAsync();
                        this.frame.Navigate(typeof(LoginPage));
                    }
                    else if (e.User.IsAnonymous)
                    {
                        this.frame.Navigate(typeof(LoginPage));
                    }
                    else if ((this.frame.Content == null || this.frame.Content.GetType() != typeof(MainPage)))
                    {
                        this.frame.Navigate(typeof(MainPage));
                    }
                });
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            if (Window.Current.Content as Frame == null)
            {
                Window.Current.Content = this.frame;
                FirebaseUI.Instance.Client.AuthStateChanged += this.AuthStateChanged;
            }

            Window.Current.Activate();
        }
    }
}
