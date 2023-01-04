using Firebase.Auth.UI.Resources;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Firebase.Auth.UI.Pages
{
    public partial class SignUpPage : Page
    {
        private TaskCompletionSource<EmailUser> tcs;

        public SignUpPage()
        {
            InitializeComponent();
        }

        public SignUpPage Initialize(TaskCompletionSource<EmailUser> tcs, string email, string error)
        {
            this.tcs = tcs;

            this.Progressbar.Visibility = Visibility.Hidden;
            this.ButtonsPanel.IsEnabled = true;
            this.PasswordBox.IsEnabled = true;
            this.NameTextBox.Focus();
            this.EmailTextBox.Text = email;

            if (error == "")
            {
                this.ErrorTextBlock.Visibility = Visibility.Hidden;
                this.ErrorTextBlock.Text = AppResources.Instance.FuiErrorWeakPasswordWithCount(6);
                this.NameErrorTextBlock.Visibility = Visibility.Hidden;
            }
            else
            {
                this.ErrorTextBlock.Text = error;
                this.ErrorTextBlock.Visibility = Visibility.Visible;
            }

            return this;
        }

        private void NameTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            this.CheckDisplayName();
        }

        private void PasswordBoxLostFocus(object sender, RoutedEventArgs e)
        {
            this.CheckPassword();
        }

        private void SignUpClick(object sender, RoutedEventArgs e)
        {
            this.SignUp();
        }

        private void SignUp()
        {
            if (!this.CheckDisplayName() || !this.CheckPassword())
            {
                return;
            }

            this.ButtonsPanel.IsEnabled = false;
            this.Progressbar.Visibility = Visibility.Visible;

            this.tcs.SetResult(new EmailUser
            {
                DisplayName = this.NameTextBox.Text,
                Email = this.EmailTextBox.Text,
                Password = this.PasswordBox.Password
            });
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            this.tcs.SetResult(null);
        }

        private bool CheckDisplayName()
        {
            var invalid = this.NameTextBox.Text == string.Empty;
            this.NameErrorTextBlock.Visibility =  invalid
                ? Visibility.Visible
                : Visibility.Hidden;
            return !invalid;
        }

        private bool CheckPassword()
        {
            var invalid = this.PasswordBox.Password.Length < 6;
            this.ErrorTextBlock.Visibility = invalid
                ? Visibility.Visible
                : Visibility.Hidden;
            return !invalid;
        }

        private void NameTextBoxKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                this.SignUp();
            }
        }

        private void PasswordBoxKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                this.SignUp();
            }
        }
    }
}
