using Firebase.Auth.UI.Converters;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Forms;

namespace Firebase.Auth.UI
{
    public static class WebAuthenticationBroker
    {
        public static Task<string> AuthenticateAsync(object owner, FirebaseProviderType provider, string uri, string redirectUri)
        {
            var tcs = new TaskCompletionSource<string>();

            System.Windows.Application.Current.Dispatcher.Invoke(() =>
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
                if (owner is Window owner_window) window.Owner = owner_window;
                else if (owner is Form owner_form)
                {
                    WindowInteropHelper helper = new WindowInteropHelper(window);
                    helper.Owner = owner_form.Handle;
                }
                if (!(window.ShowDialog() ?? false))
                {
                    tcs.SetResult(null);
                }
            });

            return tcs.Task;
        }
    }
}
