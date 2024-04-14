using Firebase.Auth;
using Firebase.Auth.Providers;
using Firebase.Auth.Repository;
using Firebase.Auth.UI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.AppLifecycle;
using System;
using Windows.Globalization;

namespace Auth.WinUI3.Sample
{
    public partial class App : Application
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

        private void AuthStateChanged(object sender, UserEventArgs e)
        {             
            dispatcherQueue.TryEnqueue(
                async () =>
                {
                    if (e.User == null)
                    {
                        if (FirebaseUI.Instance.Config.IsAnonymousAllowed)
                        {
                            await FirebaseUI.Instance.Client.SignInAnonymouslyAsync();
                        }
                        (Window.Content as Frame).Navigate(typeof(LoginPage));
                    }
                    else if (e.User.IsAnonymous)
                    {
                        (Window.Content as Frame).Navigate(typeof(LoginPage));
                    }
                    else if (Window.Content == null || Window.Content.GetType() != typeof(MainPage))
                    {
                        (Window.Content as Frame).Navigate(typeof(MainPage));
                    }
                });
        }


        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            var mainInstance = AppInstance.FindOrRegisterForKey("main");
            var activatedEventArgs = AppInstance.GetCurrent().GetActivatedEventArgs();
            
            if (!mainInstance.IsCurrent)
            {
                // Redirect the activation (and args) to the "main" instance, and exit.
                await mainInstance.RedirectActivationToAsync(activatedEventArgs);
                System.Diagnostics.Process.GetCurrentProcess().Kill();
                return;
            }

            dispatcherQueue = DispatcherQueue.GetForCurrentThread();

            if (Window == null)
            {            
                FirebaseUI.Instance.Client.AuthStateChanged += this.AuthStateChanged;
            }

            Window = new MainWindow();
            Frame rootFrame = new Frame();
            rootFrame.Navigate(typeof(MainPage));
            Window.Content = rootFrame;
            Window.Activate();
            WindowHandle = WinRT.Interop.WindowNative.GetWindowHandle(Window);
        }

        private DispatcherQueue dispatcherQueue;

        public static MainWindow Window { get; private set; }

        public static IntPtr WindowHandle { get; private set; }
    }
}
