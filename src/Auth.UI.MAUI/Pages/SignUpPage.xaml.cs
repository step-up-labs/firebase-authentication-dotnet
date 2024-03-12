namespace Firebase.Auth.UI.MAUI.Pages;

public partial class SignUpPage : ContentPage, IQueryAttributable
{
    private TaskCompletionSource<EmailUser> Result { get; set; }

    public string Email { get; set; }
    public string Error { get; set; }
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

    public SignUpPage()
	{
		InitializeComponent();
    }
    public SignUpPage Initialize()
    {
        this.IsLoading = false;
        this.ButtonsPanel.IsEnabled = true;
        this.PasswordBox.IsEnabled = true;
        this.NameTextBox.Focus();
        this.EmailTextBox.Text = Email;

        this.ErrorTextBlock.Text = Error ?? string.Empty;
        this.ErrorTextBlock.IsVisible = !string.IsNullOrEmpty(Error);

        return this;
    }

    private void NameTextBoxLostFocus(object sender, EventArgs e)
    {
        this.CheckDisplayName();
    }

    private void PasswordBoxLostFocus(object sender, EventArgs e)
    {
        this.CheckPassword();
    }

    private void SignUpClick(object sender, EventArgs e)
    {
        this.SignUp();
    }

    private void SignUp()
    {
        if (!this.CheckDisplayName() || !this.CheckPassword())
        {
            return;
        }

        this.ButtonsPanel.IsEnabled = false;
        this.IsLoading = true;

        this.Result.SetResult(new EmailUser {
            DisplayName = this.NameTextBox.Text,
            Email = this.EmailTextBox.Text,
            Password = this.PasswordBox.Text
        });
    }

    private void CancelClick(object sender, EventArgs e)
    {
        this.Result.SetResult(null);
    }

    private bool CheckDisplayName()
    {
        var invalid = this.NameTextBox.Text == string.Empty;
        this.NameErrorTextBlock.IsVisible = invalid;
        return !invalid;
    }

    private bool CheckPassword()
    {
        var invalid = this.PasswordBox.Text.Length < 6;
        this.ErrorTextBlock.IsVisible = invalid;
        return !invalid;
    }
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query == null || query.Count == 0) return;

        this.Result = (TaskCompletionSource<EmailUser>)query[nameof(Result)];
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