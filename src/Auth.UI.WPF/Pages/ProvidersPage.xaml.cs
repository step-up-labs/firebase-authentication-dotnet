using Firebase.Auth.UI.Resources;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;

namespace Firebase.Auth.UI.Pages
{
    public partial class ProvidersPage : Page
    {
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(object), typeof(ProvidersPage), new PropertyMetadata(null));

        public ProvidersPage()
        {
            InitializeComponent();

            if (DesignerProperties.GetIsInDesignMode(this))
            {
                FirebaseUI.InitializeEmpty();
            }

            this.DataContext = this;
            this.Providers = new ObservableCollection<FirebaseProviderType>(FirebaseUI.Instance.Providers);

            if (FirebaseUI.Instance.Config.IsAnonymousAllowed)
            {
                this.Providers.Add(FirebaseProviderType.Anonymous);
            }

            var str = AppResources.Instance.FuiTosAndPp;
            var arr = str.Split(new[] { "{0}", "{1}" }, StringSplitOptions.None);
            var tos = new Hyperlink(new Run(AppResources.Instance.FuiTermsOfService)) { NavigateUri = new Uri(FirebaseUI.Instance.Config.TermsOfServiceUrl) };
            var pp = new Hyperlink(new Run(AppResources.Instance.FuiPrivacyPolicy)) { NavigateUri = new Uri(FirebaseUI.Instance.Config.PrivacyPolicyUrl) };

            tos.RequestNavigate += this.NavigateLink;
            pp.RequestNavigate += this.NavigateLink;

            this.FooterTextBlock.Inlines.Add(new Run(arr[0]));
            this.FooterTextBlock.Inlines.Add(tos);
            this.FooterTextBlock.Inlines.Add(new Run(arr[1]));
            this.FooterTextBlock.Inlines.Add(pp);
            this.FooterTextBlock.Inlines.Add(new Run(arr[2]));
        }

        public ProvidersPage(IFirebaseUIFlow flow) : this()
        {
            this.Flow = flow;
        }

        public object Header
        {
            get { return (object)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public ObservableCollection<FirebaseProviderType> Providers { get; }

        public IFirebaseUIFlow Flow { get; set; }

        private void NavigateLink(object sender, RequestNavigateEventArgs e)
        {
            Launcher.LaunchUri(e.Uri);
        }

        private async void ProviderSignInClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var provider = (FirebaseProviderType)button.DataContext;

            var user = await FirebaseUI.Instance.SignInAsync(this.Flow, provider);

            if (user != null)
            {
                MessageBox.Show($"{user.Info.DisplayName} | {user.Uid}");
            }
        }
    }
}
