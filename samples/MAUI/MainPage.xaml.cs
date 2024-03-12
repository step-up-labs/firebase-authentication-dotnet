using Firebase.Auth;
using Firebase.Auth.UI;

namespace Auth.MAUI.Sample
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            FirebaseUI.Instance.Client.AuthStateChanged += this.AuthStateChanged;
        }

        private void AuthStateChanged(object sender, UserEventArgs e)
        {
            var user = e.User;

            Application.Current.Dispatcher.DispatchAsync(() =>
            {
                this.UidTextBlock.Text = user?.Uid;
                this.NameTextBlock.Text = user?.Info?.DisplayName;
                this.EmailTextBlock.Text = user?.Info?.Email;
                this.ProviderTextBlock.Text = user?.Credential?.ProviderType.ToString();

                if (!string.IsNullOrWhiteSpace(user?.Info?.PhotoUrl))
                {
                    this.ProfileImage.Source = new Uri(user.Info.PhotoUrl);
                }
            });
        }

        private void SignOutClick(object sender, EventArgs e)
        {
            FirebaseUI.Instance.Client.AuthStateChanged -= this.AuthStateChanged;
            FirebaseUI.Instance.Client.SignOut();
        }
    }

}
