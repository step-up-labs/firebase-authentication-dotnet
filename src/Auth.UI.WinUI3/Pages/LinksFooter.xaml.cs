using Microsoft.UI.Xaml.Controls;
using System;
using Windows.ApplicationModel;

namespace Firebase.Auth.UI.Pages
{
    public sealed partial class LinksFooter : UserControl
    {
        public LinksFooter()
        {
            this.InitializeComponent();

            if (DesignMode.DesignModeEnabled)
            {
                FirebaseUI.InitializeEmpty();
            }

            this.TosHyperlink.NavigateUri = new Uri(FirebaseUI.Instance.Config.TermsOfServiceUrl);
            this.PpHyperlink.NavigateUri = new Uri(FirebaseUI.Instance.Config.PrivacyPolicyUrl);
        }
    }
}
