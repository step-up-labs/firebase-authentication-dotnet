using Firebase.Auth.UI.Resources;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Navigation;

namespace Firebase.Auth.UI.Pages
{
    public sealed partial class SignInPage : Page
    {
        private TaskCompletionSource<EmailPasswordResult> tcs;

        public SignInPage()
        {
            this.InitializeComponent();
            this.Loaded += this.EmailPageLoaded;
        }

        internal Styles Styles { get; private set; }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            (var styles, var tcs, var email, var oauthEmailAttempt, var error) = ((Styles, TaskCompletionSource<EmailPasswordResult>, string, bool, string))e.Parameter;

            this.tcs = tcs;
            this.Styles = styles;
            this.Progressbar.Visibility = Visibility.Collapsed;
            this.EnableButtons(true);
            this.PasswordBox.IsEnabled = true;
            this.PasswordBox.Focus(FocusState.Programmatic);

            this.TitleTextBlock.Text = oauthEmailAttempt
                ? AppResources.Instance.FuiWelcomeBackIdpHeader
                : AppResources.Instance.FuiWelcomeBackEmailHeader;

            var message = string.Format(AppResources.Instance.FuiWelcomeBackPasswordPromptBody, email).Split(email);
            var bold = new Bold();
            bold.Inlines.Add(new Run { Text = email });
            this.WelcomeSubtitleTextBlock.Inlines.Clear();
            this.WelcomeSubtitleTextBlock.Inlines.Add(new Run { Text = message[0] });
            this.WelcomeSubtitleTextBlock.Inlines.Add(bold);
            this.WelcomeSubtitleTextBlock.Inlines.Add(new Run { Text = message[1] });

            if (error == "")
            {
                this.ErrorTextBlock.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.ErrorTextBlock.Text = error;
                this.ErrorTextBlock.Visibility = Visibility.Visible;
            }
        }

        private void EmailPageLoaded(object sender, RoutedEventArgs e)
        {
            this.PasswordBox.Focus(FocusState.Programmatic);
        }

        private void PasswordBoxKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                this.SignIn();
            }

            this.ErrorTextBlock.Visibility = Visibility.Collapsed;
        }

        private void SignInClick(object sender, RoutedEventArgs e)
        {
            this.SignIn();
        }

        private void SignIn()
        {
            this.PasswordBox.IsEnabled = false;
            this.Progressbar.Visibility = Visibility.Visible;
            this.EnableButtons(false);
            this.tcs.SetResult(EmailPasswordResult.WithPassword(this.PasswordBox.Password));
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            this.tcs.SetResult(null);
        }

        private void EnableButtons(bool enable)
        {
            this.SignInButton.IsEnabled = enable;
            this.CancelButton.IsEnabled = enable;
        }

        private void RecoverPasswordClick(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            this.tcs.SetResult(EmailPasswordResult.Reset());
        }
    }
}
