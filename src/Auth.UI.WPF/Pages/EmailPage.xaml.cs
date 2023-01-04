using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Firebase.Auth.UI.Pages
{
    public partial class EmailPage : Page
    {
        private TaskCompletionSource<string> tcs;

        public EmailPage()
        {
            InitializeComponent();
        }

        public EmailPage Initialize(TaskCompletionSource<string> tcs, string error)
        {
            this.tcs = tcs;
            this.Progressbar.Visibility = Visibility.Hidden;
            this.ButtonsPanel.IsEnabled = true;
            this.EmailTextBox.IsEnabled = true;
            this.EmailTextBox.Focus();

            if (error == "")
            {
                this.EmailTextBox.Text = string.Empty;
                this.ErrorTextBlock.Visibility = Visibility.Hidden;
            } 
            else
            {
                this.ErrorTextBlock.Visibility = Visibility.Visible;
                this.ErrorTextBlock.Text = error;
            }
            return this;
        }

        private void SignInClick(object sender, RoutedEventArgs e)
        {
            this.SignIn();
        }

        private void SignIn()
        {
            if (!this.CheckEmailAddress(this.EmailTextBox.Text))
            {
                return;
            }

            this.EmailTextBox.IsEnabled = false;
            this.Progressbar.Visibility = Visibility.Visible;
            this.ButtonsPanel.IsEnabled = false;
            this.tcs.SetResult(this.EmailTextBox.Text);
        }

        private bool CheckEmailAddress(string email)
        {
            if (!EmailValidator.ValidateEmail(email))
            {
                this.ErrorTextBlock.Visibility = Visibility.Visible;
                return false;
            }

            return true;
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            tcs.SetResult(null);
        }

        private void EmailTextBoxKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            this.ErrorTextBlock.Visibility = Visibility.Hidden;
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                this.SignIn();
            }
        }
    }
}
