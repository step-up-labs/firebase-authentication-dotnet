using Firebase.Auth.UI;
using Firebase.Auth;
using Firebase.Auth.UI.MAUI;

namespace Auth.MAUI.Sample
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            Router.RegisterShellRoutes<LoginPage>();
            Router.RegisterMainType<LoginPage>(Router.NavigationModeEnum.StackModal);
            InitializeComponent();

            FirebaseUI.Instance.Client.AuthStateChanged += this.AuthStateChanged;
        }

        private void AuthStateChanged(object sender, UserEventArgs e)
        {
            Application.Current.Dispatcher.DispatchAsync(async () =>
            {
                if (e.User == null)
                {
                    if (this.CurrentPage.GetType() != Router.MainType)
                        await Router.NavigateToMain();
                }
                else if (e.User.IsAnonymous)
                {
                    if (this.CurrentPage.GetType() != typeof(MainPage))
                        await GoToAsync(nameof(MainPage));
                }
                else if (this.CurrentPage.GetType() != typeof(MainPage))
                {
                    await GoToAsync(nameof(MainPage));
                }
            });
        }
    }
}
