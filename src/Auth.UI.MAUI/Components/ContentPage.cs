namespace Firebase.Auth.UI.MAUI.Components;

public class ContentPage<TView> : ContentPage where TView : View
{
    public TView View { get => Content as TView; }
    public ContentPage(TView content)
    {
        Content = content;
    }
}