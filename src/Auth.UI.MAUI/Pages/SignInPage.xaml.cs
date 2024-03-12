using Firebase.Auth.UI.Resources;

namespace Firebase.Auth.UI.MAUI.Pages;

public partial class SignInPage : ContentPage, IQueryAttributable
{
    private TaskCompletionSource<EmailPasswordResult> Result { get; set; }
    public string Email { get; set; }
    public string Error { get; set; }
    public bool OauthEmailAttempt { get; set; }
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
    public SignInPage()
	{
		InitializeComponent();
	}


    public SignInPage Initialize()
    {
        this.IsLoading = false;
        this.ButtonsPanel.IsEnabled = true;
        this.PasswordBox.IsEnabled = true;
        this.PasswordBox.Focus();

        this.TitleTextBlock.Text = OauthEmailAttempt ? AppResources.Instance.FuiWelcomeBackIdpHeader : AppResources.Instance.FuiWelcomeBackEmailHeader;

        var message = string.Format(AppResources.Instance.FuiWelcomeBackPasswordPromptBody, Email).Split(Email);
        this.WelcomeSubtitleTextBlock.FormattedText.Spans.Clear();
        this.WelcomeSubtitleTextBlock.FormattedText.Spans.Add(new Span() { Text = message[0] });
        this.WelcomeSubtitleTextBlock.FormattedText.Spans.Add(new Span() { Text = Email, FontAttributes = FontAttributes.Bold });
        this.WelcomeSubtitleTextBlock.FormattedText.Spans.Add(new Span() { Text = message[1] });

        this.ErrorTextBlock.Text = Error ?? string.Empty;
        this.ErrorTextBlock.IsVisible = !string.IsNullOrEmpty(Error);

        return this;
    }

    private void SignInClick(object sender, EventArgs e)
    {
        this.SignIn();
    }

    private void SignIn()
    {
        this.PasswordBox.IsEnabled = false;
        this.ButtonsPanel.IsEnabled = false;
        this.IsLoading = true;
        this.Result.SetResult(EmailPasswordResult.WithPassword(this.PasswordBox.Text));
    }

    private void CancelClick(object sender, EventArgs e)
    {
        this.Result.SetResult(null);
    }

    private void RecoverPasswordClick()
    {
        this.Result.SetResult(EmailPasswordResult.Reset());
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query == null || query.Count == 0) return;

        this.Result = (TaskCompletionSource<EmailPasswordResult>)query[nameof(Result)];
        this.OauthEmailAttempt = (bool)query[nameof(OauthEmailAttempt)];
        this.Email  = (string)query[nameof(Email)];
        this.Error  = (string)query[nameof(Error)];

        Initialize();
    }

    protected override void OnDisappearing()
    {
        if (!this.Result.Task.IsCompleted)
            this.Result.SetResult(null);
    }
}