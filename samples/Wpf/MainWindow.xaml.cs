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
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (e.User == null)
                {
                    this.Frame.Navigate(new LoginPage());
                }
                else if (this.Frame.Content.GetType() != typeof(MainPage))
                {
                    this.Frame.Navigate(new MainPage());
                }
            });
        }
    }
}
