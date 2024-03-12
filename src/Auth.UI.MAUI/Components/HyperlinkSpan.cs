namespace Firebase.Auth.UI.MAUI.Components;

public class HyperlinkSpan : Span
{
    public static readonly BindableProperty UrlProperty = BindableProperty.Create(nameof(Url), typeof(string), typeof(HyperlinkSpan), null);
    public static readonly BindableProperty ClickProperty = BindableProperty.Create(nameof(Click), typeof(Func<Task>), typeof(HyperlinkSpan), null);

    public string Url
    {
        get { return (string)GetValue(UrlProperty); }
        set { SetValue(UrlProperty, value); }
    }

    public Func<Task> Click
    {
        get { return (Func<Task>)GetValue(ClickProperty); }
        set { SetValue(ClickProperty, value); }
    }

    public HyperlinkSpan()
    {
        TextColor = Application.Current.RequestedTheme == AppTheme.Dark ? Colors.SteelBlue : Colors.Blue;
        TextDecorations = TextDecorations.Underline;

        GestureRecognizers.Add(new TapGestureRecognizer {
            Command = new Command(async () => await (Click == null ? Launcher.LaunchUriAsync(Url) : Click()))
        });
        //GestureRecognizers.Add(new PointerGestureRecognizer
        //{
        //    PointerReleasedCommand = new Command(async () => await Launcher.LaunchUriAsync(Url)),
        //    PointerEnteredCommand = new Command(() => TextColor =  Colors.Red),
        //    PointerExitedCommand = new Command(() => TextColor = Application.Current.RequestedTheme == AppTheme.Dark ? Colors.SteelBlue : Colors.Blue),
        //});
    }
}

public class Hyperlink : Label
{
    public static readonly BindableProperty UrlProperty = BindableProperty.Create(nameof(Url), typeof(string), typeof(Hyperlink), null);
    public static readonly BindableProperty ClickProperty = BindableProperty.Create(nameof(Click), typeof(Func<Task>), typeof(Hyperlink), null);
    public static readonly BindableProperty ActiveTextColorProperty = BindableProperty.Create(nameof(ActiveTextColor), typeof(Color), typeof(Hyperlink), Colors.Red);

    public string Url
    {
        get { return (string)GetValue(UrlProperty); }
        set { SetValue(UrlProperty, value); }
    }

    public Func<Task> Click
    {
        get { return (Func<Task>)GetValue(ClickProperty); }
        set { SetValue(ClickProperty, value); }
    }
    public Color ActiveTextColor
    {
        get { return (Color)GetValue(ActiveTextColorProperty); }
        set { SetValue(ActiveTextColorProperty, value); }
    }

    public Hyperlink()
    {
        TextColor = Application.Current.RequestedTheme == AppTheme.Dark ? Colors.SteelBlue : Colors.Blue;
        TextDecorations = TextDecorations.Underline;

        GestureRecognizers.Add(new TapGestureRecognizer
        {
            Command = new Command(async () => await (Click == null ? Launcher.LaunchUriAsync(Url) : Click()))
        });

        GestureRecognizers.Add(new PointerGestureRecognizer
        {
            //PointerPressedCommand
            PointerEnteredCommand = new Command(() => TextColor =  ActiveTextColor),
            PointerExitedCommand = new Command(() => TextColor = Application.Current.RequestedTheme == AppTheme.Dark ? Colors.SteelBlue : Colors.Blue),
        });
    }
}