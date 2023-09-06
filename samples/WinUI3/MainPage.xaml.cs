using Firebase.Auth;
using Firebase.Auth.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System;

namespace Auth.WinUI3.Sample
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            FirebaseUI.Instance.Client.AuthStateChanged += this.AuthStateChanged;
        }

        private void AuthStateChanged(object sender, UserEventArgs e)
        {
            var user = e.User;

            if (user?.IsAnonymous ?? true)
            {
                return;
            }

            DispatcherQueue.TryEnqueue(() =>
            {
                this.UidTextBlock.Text = user.Uid ?? "";
                this.NameTextBlock.Text = user.Info.DisplayName ?? "";
                this.EmailTextBlock.Text = user.Info.Email ?? "";
                this.ProviderTextBlock.Text = user.Credential.ProviderType.ToString();

                if (!string.IsNullOrWhiteSpace(user.Info.PhotoUrl))
                {
                    this.ProfileImage.Source = new BitmapImage(new Uri(user.Info.PhotoUrl));
                }
            });
        }

        private void SignOutClick(object sender, RoutedEventArgs e)
        {
            FirebaseUI.Instance.Client.SignOut();
        }
    }
}
