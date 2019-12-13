using System.Threading.Tasks;
using System.Windows;

namespace Firebase.Auth.UI
{
    public static class WebAuthenticationBroker
    {
        public static Task<string> AuthenticateAsync(Window owner, string uri, string redirectUri)
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
                window.WebView.Loaded += (s, e) => window.WebView.Navigate(uri);
                window.Owner = owner;
                window.ShowDialog();
            });

            return tcs.Task;
        }
    }
}
