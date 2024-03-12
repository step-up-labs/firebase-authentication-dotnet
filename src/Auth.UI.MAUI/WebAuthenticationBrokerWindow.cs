namespace Firebase.Auth.UI.MAUI;

public class WebAuthenticationBrokerWindow : ContentPage
{
    public WebView WebView { get; } = new();

    public WebAuthenticationBrokerWindow()
    {
        Content = new Grid
        {
            Children = { WebView }
        };
        Title = "Connecting to a service";
        HeightRequest = 650;
        WidthRequest = 600;
    }
}