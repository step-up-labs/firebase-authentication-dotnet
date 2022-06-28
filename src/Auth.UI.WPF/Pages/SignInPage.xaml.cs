using Firebase.Auth.UI.Resources;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Firebase.Auth.UI.Pages
{
    public partial class SignInPage : Page
    {
        private TaskCompletionSource<EmailPasswordResult> tcs;

        public SignInPage()
        {
            InitializeComponent();
        }

        public SignInPage Initialize(TaskCompletionSource<EmailPasswordResult> tcs, string email, bool oauthEmailAttempt, string error = "")
        {
            this.tcs = tcs;
            this.Progressbar.Visibility = Visibility.Hidden;
            this.ButtonsPanel.IsEnabled = true;
            this.PasswordBox.IsEnabled = true;
            this.PasswordBox.Focus();

            this.TitleTextBlock.Text = oauthEmailAttempt
                ? AppResources.Instance.FuiWelcomeBackIdpHeader
                : AppResources.Instance.FuiWelcomeBackEmailHeader;

            var message = string.Format(AppResources.Instance.FuiWelcomeBackPasswordPromptBody, email).Split(new string[] { email }, System.StringSplitOptions.None);
            this.WelcomeSubtitleTextBlock.Inlines.Clear();
            this.WelcomeSubtitleTextBlock.Inlines.Add(message[0]);
            this.WelcomeSubtitleTextBlock.Inlines.Add(new Bold(new Run(email)));
            this.WelcomeSubtitleTextBlock.Inlines.Add(message[1]);

            if (error == "")
            {
                this.ErrorTextBlock.Visibility = Visibility.Hidden;
            }
            else
            {
                this.ErrorTextBlock.Text = error;
                this.ErrorTextBlock.Visibility = Visibility.Visible;
            }

            return this;
        }

        private void SignInClick(object sender, RoutedEventArgs e)
        {
            this.SignIn();
        }

        private void SignIn()
        {
            this.PasswordBox.IsEnabled = false;
            this.Progressbar.Visibility = Visibility.Visible;
            this.ButtonsPanel.IsEnabled = false;
            this.tcs.SetResult(EmailPasswordResult.WithPassword(this.PasswordBox.Password));
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            this.tcs.SetResult(null);
        }

        private void PasswordBoxKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                this.SignIn();
            }

            this.ErrorTextBlock.Visibility = Visibility.Hidden;
        }

        private void RecoverPasswordClick(object sender, RoutedEventArgs e)
        {
            this.tcs.SetResult(EmailPasswordResult.Reset());
        }
    }
}
