using Firebase.Auth;
using Firebase.Auth.UI;
using System;
using System.Numerics;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace Auth.UWP.Sample
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            FirebaseUI.Instance.Client.AuthStateChanged += this.AuthStateChanged;
        }

        private async void AuthStateChanged(object sender, UserEventArgs e)
        {
            var user = e.User;

            if (user?.IsAnonymous ?? true)
            {
                return;
            }

            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                () =>
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
