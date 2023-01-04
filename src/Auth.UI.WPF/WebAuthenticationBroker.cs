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
                    var uri = (s as Microsoft.Web.WebView2.Wpf.WebView2).Source.ToString();
                    if (uri.StartsWith(redirectUri))
                    {
                        tcs.SetResult(uri);
                        window.DialogResult = true;
                        window.Close();
                    }
                };
                window.Title = ProviderToTitleConverter.Convert(provider);
                window.WebView.Loaded += (s, e) => window.WebView.Source = new System.Uri(uri);
                window.Owner = owner;
                if (!(window.ShowDialog() ?? false))
                {
                    tcs.SetResult(null);
                }
            });

            return tcs.Task;
        }
    }
}
