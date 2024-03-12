using Firebase.Auth.UI.MAUI.Components;
using Firebase.Auth.UI.Resources;
using System.Collections.ObjectModel;

namespace Firebase.Auth.UI.MAUI.Pages;
public partial class ProvidersPage : ContentView
{
    public ObservableCollection<FirebaseProviderType> Providers { get; set; }
    private bool isLoading = false;
    public bool IsLoading
    {
        get { return isLoading; }
        set
        {
            isLoading = value;
            OnPropertyChanged();
        }
    }
    public static readonly BindableProperty HeaderProperty = BindableProperty.Create(nameof(Header), typeof(IView), typeof(ProvidersPage));
    public IView Header
    {
        get
        {
            return GetValue(HeaderProperty) as IView;
        }

        set
        {
            SetValue(HeaderProperty, value);
            HeaderContent.Content = Header as View;
        }
    }

    public ProvidersPage()
	{
        this.Providers = new ObservableCollection<FirebaseProviderType>(FirebaseUI.Instance.Providers);

        if (FirebaseUI.Instance.Config.IsAnonymousAllowed)
            this.Providers.Add(FirebaseProviderType.Anonymous);

        InitializeComponent();

        var str = AppResources.Instance.FuiTosAndPp;
        var arr = str.Split(new[] { "{0}", "{1}" }, StringSplitOptions.None);

        FooterTextBlock.FormattedText = new();

        FooterTextBlock.FormattedText.Spans.Add(new Span() { 
            Text = arr[0]
        });
        FooterTextBlock.FormattedText.Spans.Add(CreateHiperlink(AppResources.Instance.FuiTermsOfService, FirebaseUI.Instance.Config.TermsOfServiceUrl));
        FooterTextBlock.FormattedText.Spans.Add(new Span() { 
            Text = arr[1]
        });
        FooterTextBlock.FormattedText.Spans.Add(CreateHiperlink(AppResources.Instance.FuiPrivacyPolicy, FirebaseUI.Instance.Config.PrivacyPolicyUrl));
        FooterTextBlock.FormattedText.Spans.Add(new Span() { 
            Text = arr[2]
        });
    }

    private HyperlinkSpan CreateHiperlink(string text, string url)
    {
        var span = new HyperlinkSpan()
        {
            Text = text,
            Url = url
        };

        return span;
    }

    private void ProviderSignInClick(object sender, TappedEventArgs e)
    {
        this.IsLoading = true;
        this.IsEnabled = false;

        var provider = (FirebaseProviderType)e.Parameter;

        try
        {
            _ = FirebaseUI.Instance.SignInAsync(UIFLow.Instance, provider)
                               .ContinueWith((data) => {
                                   MainThread.InvokeOnMainThreadAsync(() => {
                                       this.IsLoading = false;
                                       this.IsEnabled = true;
                                   });
                                });
        }
        catch (FirebaseAuthException ex)
        {
            _ = Application.Current.MainPage.DisplayAlert("Test", FirebaseErrorLookup.LookupError(ex), "Cancelar");
        }
    }

    private void PointerPressed(object sender, PointerEventArgs e)
    {
        VisualStateManager.GoToState(sender as VisualElement, "Pressed");
    }
}