
namespace Firebase.Auth.UI.MAUI.Pages;

public partial class RecoverPasswordPage : ContentPage, IQueryAttributable
{
    private TaskCompletionSource<object> Result { get; set; }
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

    public RecoverPasswordPage()
	{
		InitializeComponent();
    }
    public RecoverPasswordPage Initialize()
    {
        this.IsLoading = false;
        this.EmailTextBox.Text = Email;
        this.ButtonsPanel.IsEnabled = true;
        this.EmailTextBox.IsEnabled = true;
        this.EmailTextBox.Focus();

        this.EmailTextBox.Text = Error ?? string.Empty;
        this.ErrorTextBlock.IsVisible = !string.IsNullOrEmpty(Error);

        return this;
    }

    private void SendInClick(object sender, EventArgs e)
    {
        this.SendRecoverEmail();
    }
    private void SendRecoverEmail()
    {
        this.ErrorTextBlock.IsVisible = false;
        this.EmailTextBox.IsEnabled = false;
        this.IsLoading = true;
        this.ButtonsPanel.IsEnabled = false;
        this.Result.SetResult(this);
    }
    private void CancelClick(object sender, EventArgs e)
    {
        this.Result.SetResult(null);
    }

    protected override void OnDisappearing()
    {
        if (!this.Result.Task.IsCompleted)
            this.Result.SetResult(null);
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query == null || query.Count == 0) return;

        this.Result = (TaskCompletionSource<object>)query[nameof(Result)];
        this.Email  = (string)query[nameof(Email)];
        this.Error  = (string)query[nameof(Error)];

        Initialize();
    }
}