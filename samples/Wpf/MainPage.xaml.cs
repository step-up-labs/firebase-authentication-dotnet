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

            var user = FirebaseUI.Instance.Client.User;

            this.UidTextBlock.Text = user.Uid;
            this.NameTextBlock.Text = user.Info.DisplayName;
            this.EmailTextBlock.Text = user.Info.Email;

            if (!string.IsNullOrWhiteSpace(user.Info.PhotoUrl))
            {
                this.ProfileImage.Source = new BitmapImage(new Uri(user.Info.PhotoUrl));
            }
        }

        private async void SignOutClick(object sender, RoutedEventArgs e)
        {
            await FirebaseUI.Instance.Client.SignOutAsync();
        }
    }
}
