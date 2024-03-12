namespace Firebase.Auth.UI.MAUI.Components;

public partial class LinksFooter : ContentView
{
    public string TermsOfServiceUrl {  get; set; }
    public string PrivacyPolicyUrl {  get; set; }

    public LinksFooter()
    {
        TermsOfServiceUrl = FirebaseUI.Instance.Config.TermsOfServiceUrl;
        PrivacyPolicyUrl = FirebaseUI.Instance.Config.PrivacyPolicyUrl;

        InitializeComponent();
    }
}