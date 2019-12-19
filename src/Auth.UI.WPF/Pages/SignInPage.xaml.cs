using Firebase.Auth.UI.Resources;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Firebase.Auth.UI.Pages
{
    public partial class SignInPage : Page
    {
        private TaskCompletionSource<string> tcs;

        public SignInPage()
        {
            InitializeComponent();
        }

        public SignInPage Initialize(TaskCompletionSource<string> tcs, string email, string error = "")
        {
            this.tcs = tcs;
            this.WelcomeSubtitleTextBlock.Text = string.Format(AppResources.Instance.FuiWelcomeBackPasswordPromptBody, email);
            this.Progressbar.Visibility = Visibility.Hidden;
            this.ButtonsPanel.IsEnabled = true;
            this.PasswordBox.IsEnabled = true;
            this.PasswordBox.Focus();

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
            this.tcs.SetResult(this.PasswordBox.Password);
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

    }
}
