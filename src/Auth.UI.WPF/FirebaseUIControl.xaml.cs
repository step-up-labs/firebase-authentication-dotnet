using Firebase.Auth.UI.Pages;
using System;
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

        public FirebaseUIControl()
        {
            InitializeComponent();

            this.DataContext = this;
            this.emailPage = new EmailPage();
            this.signInPage = new SignInPage();
            this.signUpPage = new SignUpPage();
            this.providersPage = new ProvidersPage(this);
            this.Frame.Navigate(providersPage);
            this.Loaded += this.ControlLoaded;
        }

        public event EventHandler<UserEventArgs> AuthStateChanged;

        private void ControlLoaded(object sender, RoutedEventArgs e)
        {
            FirebaseUI.Instance.Client.AuthStateChanged += (s, e) => this.AuthStateChanged?.Invoke(s, e);
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

        Task<User> IFirebaseUIFlow.SignInExternallyAsync(FirebaseProviderType provider)
        {
            var redirectUri = FirebaseUI.Instance.Config.RedirectUri;
            var window = Window.GetWindow(this);

            return FirebaseUI.Instance.Client.SignInExternallyAsync(provider, uri => WebAuthenticationBroker.AuthenticateAsync(window, provider, uri, redirectUri));
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

        Task<string> IFirebaseUIFlow.PromptForPasswordAsync(string email, string error)
        {
            var tcs = new TaskCompletionSource<string>();
            this.Frame.Navigate(this.signInPage.Initialize(tcs, email, error));
            return tcs.Task;
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
            }
        }
    }
}
