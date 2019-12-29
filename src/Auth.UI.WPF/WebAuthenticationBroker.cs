using Firebase.Auth.UI.Converters;
using System.Threading.Tasks;
using System.Windows;

namespace Firebase.Auth.UI
{
    internal static class WebAuthenticationBroker
    {
        public static Task<string> AuthenticateAsync(Window owner, FirebaseProviderType provider, string uri, string redirectUri)
        {
            var tcs = new TaskCompletionSource<string>();

            Application.Current.Dispatcher.Invoke(() =>
            {
                var window = new WebAuthenticationBrokerWindow();
                window.WebView.NavigationCompleted += (s, e) =>
                {
                    if (e.Uri.ToString().StartsWith(redirectUri))
                    {
                        tcs.SetResult(e.Uri.ToString());
                        window.Close();
                    }
                };
                window.Title = ProviderToTitleConverter.Convert(provider);
                window.WebView.Loaded += (s, e) => window.WebView.Navigate(uri);
                window.Owner = owner;
                window.ShowDialog();
            });

            return tcs.Task;
        }
    }
}
