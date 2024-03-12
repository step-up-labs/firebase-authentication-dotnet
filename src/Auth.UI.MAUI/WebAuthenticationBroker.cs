using Firebase.Auth.UI.Converters;

namespace Firebase.Auth.UI.MAUI
{
    internal static class WebAuthenticationBroker
    {
        public static Task<string> AuthenticateAsync(NavigableElement owner, FirebaseProviderType provider, string uri, string redirectUri)
        {
            var tcs = new TaskCompletionSource<string>();

            var webviewPage = new WebAuthenticationBrokerWindow();
            var window = new Window(webviewPage)
            {
                MaximumHeight = 650,
                MaximumWidth = 600
            };
            webviewPage.WebView.Navigating += (s, e) =>
            {
                var uri = e.Url;
                if (uri.StartsWith(redirectUri))
                {
                    tcs.SetResult(uri);
                    #if MACCATALYST || WINDOWS
                        Application.Current?.CloseWindow(window);
                    #else
                        owner.Navigation.PopModalAsync(true);
                    #endif
                }
            };
            webviewPage.Title = ProviderToTitleConverter.Convert(provider);
            webviewPage.WebView.Loaded += (s, e) => webviewPage.WebView.Source = new Uri(uri);

            window.Parent = owner;
            window.Destroying += (s, e) => {
                if (!tcs.Task.IsCompleted)
                    tcs.SetResult(null);

                Application.Current?.CloseWindow(window);

            };


#if MACCATALYST || WINDOWS
                //Application.Current.OpenWindow(window);
#else
            owner.Navigation.PushModalAsync(new NavigationPage(webviewPage));
#endif
            Application.Current.OpenWindow(window);
            //owner.Dispatcher.Dispatch(() => owner.Navigation.PushModalAsync(webviewPage));

            return tcs.Task;
        }
    }
}
