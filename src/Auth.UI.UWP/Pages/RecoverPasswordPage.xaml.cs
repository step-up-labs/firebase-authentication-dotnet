using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace Firebase.Auth.UI.Pages
{
    public sealed partial class RecoverPasswordPage : Page
    {
        private TaskCompletionSource<object> tcs;

        public RecoverPasswordPage()
        {
            this.InitializeComponent();
            this.Loaded += this.EmailPageLoaded;
        }

        internal Styles Styles { get; private set; }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            (var styles, var tcs, var email, var error) = ((Styles, TaskCompletionSource<object>, string, string))e.Parameter;

            this.Styles = styles;
            this.tcs = tcs;
            this.Progressbar.Visibility = Visibility.Collapsed;
            this.EmailTextBox.Text = email;
            this.EnableButtons(true);
            this.EmailTextBox.IsEnabled = true;

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
            this.EmailTextBox.Focus(FocusState.Programmatic);
        }

        private void SendClick(object sender, RoutedEventArgs e)
        {
            this.SendRecoverEmail();
        }

        private void SendRecoverEmail()
        {
            this.ErrorTextBlock.Visibility = Visibility.Collapsed;
            this.EmailTextBox.IsEnabled = false;
            this.Progressbar.Visibility = Visibility.Visible;
            this.EnableButtons(false);
            this.tcs.SetResult(this);
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            this.tcs.SetResult(null);
        }

        private void EmailTextBoxKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                this.SendRecoverEmail();
            }
        }

        private void EnableButtons(bool enable)
        {
            this.SendButton.IsEnabled = enable;
            this.CancelButton.IsEnabled = enable;
        }
    }
}
