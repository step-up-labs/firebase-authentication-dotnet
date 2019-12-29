using Firebase.Auth.UI.Resources;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Firebase.Auth.UI.Pages
{
    public partial class RecoverPasswordPage : Page
    {
        private TaskCompletionSource<object> tcs;

        public RecoverPasswordPage()
        {
            InitializeComponent();
        }

        public RecoverPasswordPage Initialize(TaskCompletionSource<object> tcs, string email, string error = "")
        {
            this.tcs = tcs;
            this.Progressbar.Visibility = Visibility.Hidden;
            this.EmailTextBox.Text = email;
            this.ButtonsPanel.IsEnabled = true;
            this.EmailTextBox.IsEnabled = true;
            this.EmailTextBox.Focus();

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

        private void SendInClick(object sender, RoutedEventArgs e)
        {
            this.SendRecoverEmail();
        }

        private void SendRecoverEmail()
        {
            this.ErrorTextBlock.Visibility = Visibility.Hidden;
            this.EmailTextBox.IsEnabled = false;
            this.Progressbar.Visibility = Visibility.Visible;
            this.ButtonsPanel.IsEnabled = false;
            this.tcs.SetResult(this);
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            this.tcs.SetResult(null);
        }

        private void EmailTextBoxKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                this.SendRecoverEmail();
            }
        }
    }
}
