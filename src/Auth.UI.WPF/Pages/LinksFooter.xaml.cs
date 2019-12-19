using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Firebase.Auth.UI.Pages
{
    public partial class LinksFooter : UserControl
    {
        public LinksFooter()
        {
            InitializeComponent();

            if (DesignerProperties.GetIsInDesignMode(this))
            {
                FirebaseUI.InitializeEmpty();
            }

            this.TosHyperlink.NavigateUri = new Uri(FirebaseUI.Instance.Config.TermsOfServiceUrl);
            this.PpHyperlink.NavigateUri = new Uri(FirebaseUI.Instance.Config.PrivacyPolicyUrl);
        }

        private void NavigateLink(object sender, RequestNavigateEventArgs e)
        {
            Launcher.LaunchUri(e.Uri);
        }
    }
}
