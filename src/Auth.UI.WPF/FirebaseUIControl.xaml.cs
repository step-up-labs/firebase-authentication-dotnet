using System.Windows;
using System.Windows.Controls;

namespace Firebase.Auth.UI
{
    public partial class FirebaseUIControl : UserControl
    {
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(object), typeof(FirebaseUIControl), new PropertyMetadata(null));
        
        public FirebaseUIControl()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public object Header
        {
            get { return (object)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }
    }
}
