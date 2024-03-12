
using System.ComponentModel;

namespace Firebase.Auth.UI.MAUI.Pages;

public partial class EmailPage : ContentPage, IQueryAttributable
{
    private TaskCompletionSource<string> Result { get; set; }
    private string Error { get; set; }
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
    public EmailPage()
    {
        InitializeComponent();
    }

    public EmailPage Initialize()
    {
        this.IsLoading = false;
        this.ButtonsPanel.IsEnabled = true;
        this.EmailTextBox.IsEnabled = true;
        this.EmailTextBox.Focus();

        this.EmailTextBox.Text = Error ?? string.Empty;
        this.ErrorTextBlock.IsVisible = !string.IsNullOrEmpty(Error);
        
        Unloaded += EmailPage_Unloaded;
        return this;
    }
    private void SignInClick(object sender, EventArgs e)
    {
        this.SignIn();
    }

    private void SignIn()
    {
        if (!this.CheckEmailAddress(this.EmailTextBox.Text))
            return;

        this.IsLoading = true;
        this.EmailTextBox.IsEnabled = false;
        this.ButtonsPanel.IsEnabled = false;
        this.Result.SetResult(this.EmailTextBox.Text);
    }

    private bool CheckEmailAddress(string email)
    {
        if (!EmailValidator.ValidateEmail(email))
        {
            this.ErrorTextBlock.IsVisible = true;
            return false;
        }

        return true;
    }

    private void CancelClick(object sender, EventArgs e)
    {
        Result.SetResult(null);
    }

    private void EmailPage_Unloaded(object sender, EventArgs e)
    {
        if (!Result.Task.IsCompleted)
            Result.SetResult(null);
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query == null || query.Count == 0) return;

        this.Result = (TaskCompletionSource<string>)query[nameof(Result)];
        this.Error  = (string)query[nameof(Error)];

        Initialize();
    }
}