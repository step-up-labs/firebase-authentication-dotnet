using Firebase.Auth.UI.Resources;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;

namespace Firebase.Auth.UI
{
    /// <summary>
    /// Interaction logic for FirebaseUILogin.xaml
    /// </summary>
    public partial class FirebaseUILogin : UserControl
    {
        private readonly FirebaseUI firebaseUI;

        public FirebaseUILogin()
        {
            InitializeComponent();
            
            if (DesignerProperties.GetIsInDesignMode(this))
            {
                FirebaseUI.InitializeEmpty();
            }

            this.DataContext = this;
            this.firebaseUI = FirebaseUI.Instance;
            this.Providers = new ObservableCollection<FirebaseProviderType>(this.firebaseUI.Providers);

            if (this.firebaseUI.Config.IsAnonymousAllowed)
            {
                this.Providers.Add(FirebaseProviderType.Anonymous);
            }

            var str = AppResources.Instance.FuiTosAndPp;
            var arr = str.Split(new[] { "{0}", "{1}" }, StringSplitOptions.None);
            var tos = new Hyperlink(new Run(AppResources.Instance.FuiTermsOfService)) { NavigateUri = new Uri(this.firebaseUI.Config.TermsOfServiceUrl) };
            var pp = new Hyperlink(new Run(AppResources.Instance.FuiPrivacyPolicy)) { NavigateUri = new Uri(this.firebaseUI.Config.PrivacyPolicyUrl) };

            tos.RequestNavigate += this.NavigateLink;
            pp.RequestNavigate += this.NavigateLink;

            this.FooterTextBlock.Inlines.Add(new Run(arr[0]));
            this.FooterTextBlock.Inlines.Add(tos);
            this.FooterTextBlock.Inlines.Add(new Run(arr[1]));
            this.FooterTextBlock.Inlines.Add(pp);
            this.FooterTextBlock.Inlines.Add(new Run(arr[2]));
        }

        public ObservableCollection<FirebaseProviderType> Providers { get; }

        private void NavigateLink(object sender, RequestNavigateEventArgs e)
        {
            var ps = new ProcessStartInfo(e.Uri.ToString())
            {
                UseShellExecute = true,
                Verb = "open"
            };
            Process.Start(ps);
        }

        private async void ProviderSignInClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var provider = (FirebaseProviderType)button.DataContext;
            var window = Window.GetWindow(this);
            var redirectUri = this.firebaseUI.Config.RedirectUri;

            switch (provider)
            {
                case FirebaseProviderType.Facebook:
                case FirebaseProviderType.Google:
                case FirebaseProviderType.Github:
                case FirebaseProviderType.Twitter:
                case FirebaseProviderType.Microsoft:
                    var u1 = await this.firebaseUI.Client.SignInExternallyAsync(provider, uri => WebAuthenticationBroker.AuthenticateAsync(window, uri, redirectUri));
                    window.Title = u1.Info.DisplayName;
                    break;
                case FirebaseProviderType.EmailAndPassword:
                    var u2 = await this.firebaseUI.Client.SignInWithEmailAndPasswordAsync("tomas@settleup.io", "xxx");
                    window.Title = u2.Info.DisplayName;
                    break;
                case FirebaseProviderType.Anonymous:
                    var u3 = await this.firebaseUI.Client.SignInAnonymouslyAsync();
                    window.Title = u3.Uid;
                    break;
                default:
                    break;
            }
        }
    }
}
