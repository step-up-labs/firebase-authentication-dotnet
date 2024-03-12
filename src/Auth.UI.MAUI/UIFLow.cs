using Firebase.Auth.UI.MAUI.Pages;
using Firebase.Auth.UI.Resources;

namespace Firebase.Auth.UI.MAUI
{
    internal class UIFLow : IFirebaseUIFlow
    {
        public static UIFLow Instance { get; } = new();
        private INavigation Navigator { get => Application.Current.MainPage.Navigation; }
        public Task<string> GetRedirectResponseUriAsync(FirebaseProviderType provider, string uri)
        {
            return WebAuthenticationBroker.AuthenticateAsync(Application.Current.MainPage, provider, uri, FirebaseUI.Instance.Config.RedirectUri);
        }

        public Task<string> PromptForEmailAsync(string error = "")
        {
            var tcs = new TaskCompletionSource<string>();
            var parameters = new Dictionary<string, object>()
            {
                { "Error", error },
                { "Result", tcs },
            };

            Navigate<EmailPage>(parameters);
            return tcs.Task;
        }

        public Task<EmailUser> PromptForEmailPasswordNameAsync(string email, string error = "")
        {
            var tcs = new TaskCompletionSource<EmailUser>();
            var parameters = new Dictionary<string, object>()
            {
                { "Email", email },
                { "Error", error },
                { "Result", tcs },
            };

            Navigate<SignUpPage>(parameters);
            return tcs.Task;
        }

        public Task<EmailPasswordResult> PromptForPasswordAsync(string email, bool oauthEmailAttempt, string error = "")
        {
            var tcs = new TaskCompletionSource<EmailPasswordResult>();
            var parameters = new Dictionary<string, object>()
            {
                { "Email", email },
                { "Error", error },
                { "Result", tcs },
                { "OauthEmailAttempt", oauthEmailAttempt },
            };

            Navigate<SignInPage>(parameters);
            return tcs.Task;
        }

        public Task<object> PromptForPasswordResetAsync(string email, string error = "")
        {
            var tcs = new TaskCompletionSource<object>();
            var parameters = new Dictionary<string, object>()
            {
                { "Email", email },
                { "Error", error },
                { "Result", tcs },
            };

            Navigate<RecoverPasswordPage>(parameters);
            return tcs.Task;
        }

        public void Reset()
        {
            if (FirebaseUI.Instance.Client.User == null)
                NavigateToMain();
        }

        private Task DirectNavigateToAsync<TPage>(IDictionary<string, object> parameters = null) where TPage : Page, new()
        {
            return DirectNavigateToAsync(typeof(TPage), parameters);
        }
        private Task DirectNavigateToAsync(Type type, IDictionary<string, object> parameters = null)
        {
            return MainThread.InvokeOnMainThreadAsync(() => {
                var page = Activator.CreateInstance(type) as Page;
                Application.Current.MainPage = page;
                SetParameters(page, parameters);
            });
        }

        private Task ShellNavigateToAsync<TPage>(IDictionary<string, object> parameters = null) where TPage : Page
        {
            return ShellNavigateToAsync(typeof(TPage), parameters ?? new Dictionary<string, object>());
        }
        private Task ShellNavigateToAsync(Type type, IDictionary<string, object> parameters = null)
        {
            return MainThread.InvokeOnMainThreadAsync(async () => {
                await Shell.Current.GoToAsync(Router.GetPath(type), parameters ?? new Dictionary<string, object>());
            });
        }

        private Task StackNavigateToAsync<TPage>(IDictionary<string, object> parameters = null) where TPage : Page, new()
        {
            return StackNavigateToAsync(typeof(TPage), parameters);
        }
        private Task StackNavigateToAsync(Type type, IDictionary<string, object> parameters = null)
        {
            return MainThread.InvokeOnMainThreadAsync(async () => {
                var page = Activator.CreateInstance(type) as Page;
                await Navigator.PushAsync(page);

                if (Navigator.NavigationStack.Count > 1)
                    Navigator.RemovePage(Navigator.NavigationStack.SkipLast(1).Last());

                SetParameters(page, parameters);
            });
        }

        private Task StackModalNavigateToAsync<TPage>(IDictionary<string, object> parameters = null) where TPage : Page, new()
        {
            return StackModalNavigateToAsync(typeof(TPage), parameters);
        }
        private Task StackModalNavigateToAsync(Type type, IDictionary<string, object> parameters = null)
        {
            return MainThread.InvokeOnMainThreadAsync(async () => {
                var page = Activator.CreateInstance(type) as Page;

                while (Navigator.ModalStack.Count > 1)
                    await Navigator.PopModalAsync();

                await Navigator.PushModalAsync(page);

                SetParameters(page, parameters);
            });
        }

        private void SetParameters(Page page, IDictionary<string, object> parameters)
        {
            if (page.GetType().IsAssignableTo(typeof(IQueryAttributable)))
            {
                (page as IQueryAttributable).ApplyQueryAttributes(parameters ?? new Dictionary<string, object>());
            }
        }

        private Task Navigate(Type type, IDictionary<string, object> parameters = null)
        {
            switch (Router.NavigationMode)
            {
                case Router.NavigationModeEnum.Shell:
                    return ShellNavigateToAsync(type, parameters);
                case Router.NavigationModeEnum.Stack:
                    return StackNavigateToAsync(type, parameters);
                case Router.NavigationModeEnum.Direct:
                    return DirectNavigateToAsync(type, parameters);
                case Router.NavigationModeEnum.StackModal:
                    return StackModalNavigateToAsync(type, parameters);
            }

            return Task.CompletedTask;
        }
        private Task Navigate<TPage>(IDictionary<string, object> parameters = null) where TPage : Page, new()
        {
            switch (Router.NavigationMode)
            {
                case Router.NavigationModeEnum.Shell:
                    return ShellNavigateToAsync<TPage>(parameters);
                case Router.NavigationModeEnum.Stack:
                    return StackNavigateToAsync<TPage>(parameters);
                case Router.NavigationModeEnum.Direct:
                    return DirectNavigateToAsync<TPage>(parameters);
                case Router.NavigationModeEnum.StackModal:
                    return StackModalNavigateToAsync<TPage>(parameters);
            }

            return Task.CompletedTask;
        }
        internal Task NavigateToMain()
        {
            return MainThread.InvokeOnMainThreadAsync(async() => {
                switch (Router.NavigationMode)
                {
                    case Router.NavigationModeEnum.Shell:
                        await Shell.Current.GoToAsync(Router.GetMainPath());
                        break;
                    case Router.NavigationModeEnum.Stack:
                    case Router.NavigationModeEnum.Direct:
                    case Router.NavigationModeEnum.StackModal:
                        await Navigate(Router.MainType);
                        break;
                }
            });
        }

        public Task<bool> ShowEmailProviderConflictAsync(string email, FirebaseProviderType providerType)
        {
            return Application.Current.MainPage.DisplayAlert(AppResources.Instance.FuiWelcomeBackIdpHeader, string.Format(AppResources.Instance.FuiWelcomeBackIdpPrompt, email, providerType), AppResources.Instance.FuiContinue, AppResources.Instance.FuiCancel);
        }

        public Task ShowPasswordResetConfirmationAsync(string email)
        {
            return Application.Current.MainPage.DisplayAlert(AppResources.Instance.FuiTitleConfirmRecoverPassword, string.Format(AppResources.Instance.FuiConfirmRecoveryBody, email), AppResources.Instance.FuiCancel);
        }
    }
}