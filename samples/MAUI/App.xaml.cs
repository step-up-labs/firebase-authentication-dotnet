using Firebase.Auth;
using Firebase.Auth.UI;
using Firebase.Auth.UI.MAUI;

namespace Auth.MAUI.Sample
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            // Use Direct Navigation
            UseDirectNavigation();
            // Use APP Shell
            //MainPage = new AppShell();
            // Use Navigation Page
            //MainPage = new AppNavigation();
        }

        private void UseDirectNavigation()
        {
            Router.RegisterMainType<LoginPage>(Router.NavigationModeEnum.Direct);
            MainPage = new LoginPage();
            FirebaseUI.Instance.Client.AuthStateChanged += this.AuthStateChanged;
        }
        private void AuthStateChanged(object sender, UserEventArgs e)
        {
            Application.Current.Dispatcher.DispatchAsync(async () =>
            {
                if (e.User == null)
                {
                    if (this.MainPage?.GetType() != Router.MainType)
                        await Router.NavigateToMain();
                }
                else if (e.User.IsAnonymous)
                {
                    if (this.MainPage?.GetType() != typeof(MainPage))
                        MainPage = new MainPage();
                }
                else if (this.MainPage?.GetType() != typeof(MainPage))
                {
                    MainPage = new MainPage();
                }
            });
        }
    }
}
