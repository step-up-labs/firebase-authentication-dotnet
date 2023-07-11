using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using System.Threading.Tasks;

namespace Firebase.Auth.UI.Pages
{
    public sealed partial class EmailPage : Page
    {
        private TaskCompletionSource<string> tcs;

        public EmailPage()
        {
            this.InitializeComponent();
            this.Loaded += this.EmailPageLoaded;
        }

        internal Styles Styles { get; private set; }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            (var styles, var tcs, var error) = ((Styles, TaskCompletionSource<string>, string))e.Parameter;

            this.tcs = tcs;
            this.Styles = styles;

            this.Progressbar.Visibility = Visibility.Collapsed;
            this.EnableButtons(true);
            this.EmailTextBox.IsEnabled = true;

            if (error == "")
            {
                this.EmailTextBox.Text = string.Empty;
                this.ErrorTextBlock.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.ErrorBorder.Visibility = Visibility.Visible;
                this.ErrorTextBlock.Text = error;
            }
        }

        private void EmailPageLoaded(object sender, RoutedEventArgs e)
        {
            this.EmailTextBox.Focus(FocusState.Programmatic);
        }

        private void EnableButtons(bool enable)
        {
            this.SignInButton.IsEnabled = enable;
            this.CancelButton.IsEnabled = enable;
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            tcs.SetResult(null);
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
            this.EnableButtons(false);
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

        private void EmailTextBoxKeyDown(object sender, KeyRoutedEventArgs e)
        {
            this.ErrorTextBlock.Visibility = Visibility.Collapsed;
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                this.SignIn();
            }
            else if (e.Key == Windows.System.VirtualKey.Escape)
            {
                tcs.SetResult(null);
            }
        }
    }
}
