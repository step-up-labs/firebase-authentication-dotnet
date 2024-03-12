using Firebase.Auth.UI.MAUI.Pages;

namespace Firebase.Auth.UI.MAUI;

public partial class FirebaseUIControl : ContentView
{
    public static readonly BindableProperty HeaderProperty = BindableProperty.Create(nameof(FirebaseUIControl), typeof(IView), typeof(FirebaseUIControl),propertyChanged: HeaderValueChanged);
    public event EventHandler<UserEventArgs> AuthStateChanged;
    private ProvidersPage ProvidersPage { get; set; }
    private Page CurrentPage { get; set; }
    private static void HeaderValueChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is FirebaseUIControl control)
        {
            control.ProvidersPage.Header = newValue as IView;
        }
    }

    public IView Header
    {
        get => GetValue(HeaderProperty) as IView;
        set => SetValue(HeaderProperty, value);
    }

    public FirebaseUIControl()
    {
        InitializeComponent();
        if (!FirebaseUI.IsInitialized)
        {
            this.Content = new Label {
                Text = "FirebaseUI has not been initialized yet. Make sure to initialize it during the startup of your application, e.g. in MauiProgram.cs",
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
            };
            return;
        }

        ProvidersPage = new() { Header = Header };
        this.Loaded += ControlLoaded;
        this.Unloaded += ControlUnloaded;

        this.ContentPage.Children.Add(ProvidersPage);
    }
    private void ControlUnloaded(object sender, EventArgs e)
    {
        FirebaseUI.Instance.Client.AuthStateChanged -= ClientAuthStateChanged;
    }

    private void ControlLoaded(object sender, EventArgs args)
    {
        FirebaseUI.Instance.Client.AuthStateChanged += ClientAuthStateChanged;
    }
    private void ClientAuthStateChanged(object sender, UserEventArgs e)
    {
        Dispatcher.DispatchAsync(new Action(() => this.AuthStateChanged?.Invoke(sender, e)));
    }

}