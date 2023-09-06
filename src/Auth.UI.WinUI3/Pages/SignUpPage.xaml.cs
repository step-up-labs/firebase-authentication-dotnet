using Firebase.Auth.UI.Resources;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using System.Threading.Tasks;
using Windows.System;

namespace Firebase.Auth.UI.Pages
{
    public sealed partial class SignUpPage : Page
    {
        private TaskCompletionSource<EmailUser> tcs;

        public SignUpPage()
        {
            this.InitializeComponent();
            this.Loaded += this.EmailPageLoaded;
        }

        internal Styles Styles { get; private set; }

        private void EmailPageLoaded(object sender, RoutedEventArgs e)
        {
            this.NameTextBox.Focus(FocusState.Programmatic);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            (var styles, var tcs, var email, var error) = ((Styles, TaskCompletionSource<EmailUser>, string, string))e.Parameter;

            this.Styles = styles;
            this.tcs = tcs;

            //this.Progressbar.Visibility = Visibility.Collapsed;
            this.EnableButtons(true);
            this.PasswordBox.IsEnabled = true;
            this.EmailTextBox.Text = email;

            if (error == "")
            {
                this.ErrorTextBlock.Visibility = Visibility.Collapsed;
                this.ErrorTextBlock.Text = AppResources.Instance.FuiErrorWeakPasswordWithCount(6);
                this.NameErrorTextBlock.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.ErrorTextBlock.Text = error;
                this.ErrorTextBlock.Visibility = Visibility.Visible;
            }
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

            this.EnableButtons(false);
            //this.Progressbar.Visibility = Visibility.Visible;

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
            this.NameErrorTextBlock.Visibility = invalid
                ? Visibility.Visible
                : Visibility.Collapsed;
            return !invalid;
        }

        private bool CheckPassword()
        {
            var invalid = this.PasswordBox.Password.Length < 6;
            this.ErrorTextBlock.Visibility = invalid
                ? Visibility.Visible
                : Visibility.Collapsed;
            return !invalid;
        }

        private void NameTextBoxKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                this.SignUp();
            }
        }

        private void PasswordBoxKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                this.SignUp();
            }
        }

        private void EnableButtons(bool enable)
        {
            this.SignUpButton.IsEnabled = enable;
            this.CancelButton.IsEnabled = enable;
        }
    }
}
