using Firebase.Auth.UI.Converters;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;

namespace Firebase.Auth.UI
{
    internal static class WebAuthenticationBroker
    {
        public static Task<string> AuthenticateAsync(Control parent, FirebaseProviderType provider, string uri, string redirectUri)
        {
            var tcs = new TaskCompletionSource<string>();

            var dispatcherQueue = DispatcherQueue.GetForCurrentThread();
            dispatcherQueue.TryEnqueue(
                async () =>
                {
                    var window = new WebAuthenticationBrokerWindow();
                    //if (ApiInformation.IsApiContractPresent("Windows.Foundation.FoundationContract", 7))
                    //{
                    //    window.CornerRadius = new CornerRadius(0);
                    //}
                    window.XamlRoot = parent.XamlRoot;
                    window.WebView.NavigationCompleted += (s, e) =>
                    {
                        var uri = s.Source.ToString();
                        if (uri.StartsWith(redirectUri))
                        {
                            tcs.SetResult(uri);
                            window.Hide();
                        }
                    };

                    window.Loaded += async (s, e) =>
                    {
                        await window.WebView.EnsureCoreWebView2Async();
                    };

                    window.WebView.CoreWebView2Initialized += (s, e) =>
                    {
                        window.WebView.Source = new Uri(uri);
                    };

                    window.Title = ProviderToTitleConverter.Convert(provider);                    
           
                    await window.ShowAsync();
                });

            return tcs.Task;
        }
    }
}
