using Firebase.Auth;
using Firebase.Auth.UI;
using Firebase.Auth.UI.MAUI;

namespace Auth.MAUI.Sample;

public partial class AppNavigation : NavigationPage
{
	public AppNavigation()
	{
        Router.RegisterMainType<LoginPage>(Router.NavigationModeEnum.Stack);
        InitializeComponent();

        FirebaseUI.Instance.Client.AuthStateChanged += this.AuthStateChanged;
    }

    private void AuthStateChanged(object sender, UserEventArgs e)
    {
        Application.Current.Dispatcher.DispatchAsync(async () =>
        {
            Shell.SetBackButtonBehavior(this, new BackButtonBehavior() { IsVisible = false });

            if (e.User == null)
            {
                if (this.Navigation.NavigationStack.LastOrDefault()?.GetType() != Router.MainType)
                    await Router.NavigateToMain();
            }
            else if (e.User.IsAnonymous)
            {
                if (this.Navigation.NavigationStack.LastOrDefault()?.GetType() != typeof(MainPage))
                    await Router.NavigateToMain();
            }
            else if (this.Navigation.NavigationStack.LastOrDefault()?.GetType() != typeof(MainPage))
            {
                await this.Navigation.PushAsync(new MainPage());
            }
        });
    }
}