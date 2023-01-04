using Firebase.Auth.UI.Pages;
using Firebase.Auth.UI.Resources;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Firebase.Auth.UI
{
    public partial class FirebaseUIControl : UserControl, IFirebaseUIFlow
    {
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(object), typeof(FirebaseUIControl), new PropertyMetadata(HeaderValueChanged));

        private readonly EmailPage emailPage;
        private readonly ProvidersPage providersPage;
        private readonly SignInPage signInPage;
        private readonly SignUpPage signUpPage;
        private readonly RecoverPasswordPage recoverPasswordPage;

        public FirebaseUIControl()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this) && !FirebaseUI.IsInitialized)
            {
                this.Content = new TextBlock
                {
                    Text = "FirebaseUI has not been initialized yet. Make sure to initialize it during the startup of your application, e.g. in App.xaml.cs",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    TextWrapping = TextWrapping.Wrap
                };
                return;
            }

            this.DataContext = this;
            this.emailPage = new EmailPage();
            this.signInPage = new SignInPage();
            this.signUpPage = new SignUpPage();
            this.recoverPasswordPage = new RecoverPasswordPage();
            this.providersPage = new ProvidersPage(this);
            this.Loaded += this.ControlLoaded;
            this.Unloaded += this.ControlUnloaded;
            this.Frame.Navigate(providersPage);
        }

        public event EventHandler<UserEventArgs> AuthStateChanged;

        private void ControlUnloaded(object sender, RoutedEventArgs e)
        {
            FirebaseUI.Instance.Client.AuthStateChanged -= ClientAuthStateChanged;
        }

        private void ControlLoaded(object sender, RoutedEventArgs args)
        {
            FirebaseUI.Instance.Client.AuthStateChanged += ClientAuthStateChanged;
        }

        private void ClientAuthStateChanged(object sender, UserEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() => this.AuthStateChanged?.Invoke(sender, e)));
        }

        public object Header
        {
            get { return (object)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        private static void HeaderValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = (FirebaseUIControl)d;
            if (self.providersPage != null)
            {
                self.providersPage.Header = self.Header;
            }
        }

        Task<string> IFirebaseUIFlow.GetRedirectResponseUriAsync(FirebaseProviderType provider, string uri)
        {
            var redirectUri = FirebaseUI.Instance.Config.RedirectUri;
            var window = Window.GetWindow(this);
            return WebAuthenticationBroker.AuthenticateAsync(window, provider, uri, redirectUri);
        }

        Task<string> IFirebaseUIFlow.PromptForEmailAsync(string error)
        {
            var tcs = new TaskCompletionSource<string>();
            this.Frame.Navigate(this.emailPage.Initialize(tcs, error));
            return tcs.Task;
        }

        Task<EmailUser> IFirebaseUIFlow.PromptForEmailPasswordNameAsync(string email, string error)
        {
            var tcs = new TaskCompletionSource<EmailUser>();
            this.Frame.Navigate(this.signUpPage.Initialize(tcs, email, error));
            return tcs.Task;
        }

        Task<EmailPasswordResult> IFirebaseUIFlow.PromptForPasswordAsync(string email, bool oauthEmailAttempt, string error)
        {
            var tcs = new TaskCompletionSource<EmailPasswordResult>();
            this.Frame.Navigate(this.signInPage.Initialize(tcs, email, oauthEmailAttempt, error));
            return tcs.Task;
        }

        Task<object> IFirebaseUIFlow.PromptForPasswordResetAsync(string email, string error)
        {
            var tcs = new TaskCompletionSource<object>();
            this.Frame.Navigate(this.recoverPasswordPage.Initialize(tcs, email, error));
            return tcs.Task;
        }

        Task IFirebaseUIFlow.ShowPasswordResetConfirmationAsync(string email)
        {
            MessageBox.Show(string.Format(AppResources.Instance.FuiConfirmRecoveryBody, email), AppResources.Instance.FuiTitleConfirmRecoverPassword, MessageBoxButton.OK, MessageBoxImage.Information);
            return Task.CompletedTask;
        }

        Task<bool> IFirebaseUIFlow.ShowEmailProviderConflictAsync(string email, FirebaseProviderType providerType)
        {
            var result = MessageBox.Show(string.Format(AppResources.Instance.FuiWelcomeBackIdpPrompt, email, providerType), AppResources.Instance.FuiWelcomeBackIdpHeader, MessageBoxButton.OKCancel, MessageBoxImage.Information);

            return Task.FromResult(result == MessageBoxResult.OK);
        }

        void IFirebaseUIFlow.Reset()
        {
            while (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
        }

        private void FrameNavigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            if (e.Uri?.ToString().StartsWith("http") ?? false)
            {
                e.Cancel = true;
                if (this.Frame.CanGoBack)
                {
                    Launcher.LaunchUri(e.Uri);
                }
            }
        }
    }
}
