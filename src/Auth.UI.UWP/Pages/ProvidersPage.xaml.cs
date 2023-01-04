using Firebase.Auth.UI.Resources;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.ApplicationModel;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Firebase.Auth.UI.Pages
{
    public sealed partial class ProvidersPage : Page
    {
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(object), typeof(ProvidersPage), new PropertyMetadata(null));

        public ProvidersPage()
        {
            InitializeComponent();

            if (DesignMode.DesignModeEnabled)
            {
                FirebaseUI.InitializeEmpty();
            }

            this.Providers = new ObservableCollection<FirebaseProviderType>(FirebaseUI.Instance.Providers);

            if (FirebaseUI.Instance.Config.IsAnonymousAllowed)
            {
                this.Providers.Add(FirebaseProviderType.Anonymous);
            }

            var str = AppResources.Instance.FuiTosAndPp;
            var arr = str.Split(new[] { "{0}", "{1}" }, StringSplitOptions.None);
            var tos = new Hyperlink() { NavigateUri = new Uri(FirebaseUI.Instance.Config.TermsOfServiceUrl) };
            tos.Inlines.Add(new Run() { Text = AppResources.Instance.FuiTermsOfService });
            var pp = new Hyperlink() { NavigateUri = new Uri(FirebaseUI.Instance.Config.PrivacyPolicyUrl) };
            pp.Inlines.Add(new Run() { Text = AppResources.Instance.FuiPrivacyPolicy });

            this.FooterTextBlock.Inlines.Add(new Run() { Text = arr[0] });
            this.FooterTextBlock.Inlines.Add(tos);
            this.FooterTextBlock.Inlines.Add(new Run() { Text = arr[1] });
            this.FooterTextBlock.Inlines.Add(pp);
            this.FooterTextBlock.Inlines.Add(new Run() { Text = arr[2] });
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            this.Flow = e.Parameter as IFirebaseUIFlow;
        }

        public object Header
        {
            get { return (object)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public ObservableCollection<FirebaseProviderType> Providers { get; }

        public IFirebaseUIFlow Flow { get; set; }

        private async void ProviderSignInClick(object sender, RoutedEventArgs e)
        {
            this.ProgressBar.Visibility = Visibility.Visible;
            this.ScrollViewer.IsEnabled = false;

            var button = sender as Button;
            var provider = (FirebaseProviderType)button.DataContext;

            try
            {
                await FirebaseUI.Instance.SignInAsync(this.Flow, provider);
            }
            catch (FirebaseAuthException ex)
            {
                await new MessageDialog(FirebaseErrorLookup.LookupError(ex)).ShowAsync();
            }

            this.ProgressBar.Visibility = Visibility.Collapsed;
            this.ScrollViewer.IsEnabled = true;
        }

        private void GridButtonLoaded(object sender, RoutedEventArgs args)
        {
            if (sender is Grid grid)
            {
                var shadow = grid.Resources["ThemeShadow"] as ThemeShadow;
                shadow.Receivers.Add(BackgroundGrid);
                grid.Translation = new System.Numerics.Vector3(0, 0, 32);
            }
        }
    }
}
