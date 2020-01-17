using Firebase.Auth.UI;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Firebase.Auth.Wpf.Sample
{
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();

            FirebaseUI.Instance.Client.AuthStateChanged += this.AuthStateChanged;
        }

        private void AuthStateChanged(object sender, UserEventArgs e)
        {
            var user = e.User;

            Application.Current.Dispatcher.Invoke(() =>
            {
                this.UidTextBlock.Text = user.Uid;
                this.NameTextBlock.Text = user.Info.DisplayName;
                this.EmailTextBlock.Text = user.Info.Email;
                this.ProviderTextBlock.Text = user.Credential.ProviderType.ToString();

                if (!string.IsNullOrWhiteSpace(user.Info.PhotoUrl))
                {
                    this.ProfileImage.Source = new BitmapImage(new Uri(user.Info.PhotoUrl));
                }
            });
        }

        private async void SignOutClick(object sender, RoutedEventArgs e)
        {
            FirebaseUI.Instance.Client.AuthStateChanged -= this.AuthStateChanged;
            await FirebaseUI.Instance.Client.SignOutAsync();
        }
    }
}
