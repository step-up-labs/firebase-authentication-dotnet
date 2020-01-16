using Firebase.Auth.UI;
using System.Windows;

namespace Firebase.Auth.Wpf.Sample
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            FirebaseUI.Instance.Client.AuthStateChanged += this.AuthStateChanged;
        }

        private void AuthStateChanged(object sender, UserEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(async () =>
            {
                if (e.User == null)
                {
                    await FirebaseUI.Instance.Client.SignInAnonymouslyAsync();
                    this.Frame.Navigate(new LoginPage());
                }
                else if (e.User.IsAnonymous)
                {
                    this.Frame.Navigate(new LoginPage());
                }
                else if ((this.Frame.Content == null || this.Frame.Content.GetType() != typeof(MainPage)))
                {
                    this.Frame.Navigate(new MainPage());
                }
            });
        }
    }
}
