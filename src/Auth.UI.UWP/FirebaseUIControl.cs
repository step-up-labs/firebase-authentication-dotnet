﻿using Firebase.Auth.UI.Pages;
using Firebase.Auth.UI.Resources;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation.Metadata;
using Windows.Security.Authentication.Web;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace Firebase.Auth.UI
{
    public class FirebaseUIControl : Control, IFirebaseUIFlow
    {
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(object), typeof(FirebaseUIControl), new PropertyMetadata(null));
        public static readonly DependencyProperty TitleTextBlockStyleProperty = DependencyProperty.Register("TitleTextBlockStyle", typeof(Style), typeof(FirebaseUIControl), new PropertyMetadata(Styles.Default.TitleStyle));
        public static readonly DependencyProperty HeaderTextBlockStyleProperty = DependencyProperty.Register("HeaderTextBlockStyle", typeof(Style), typeof(FirebaseUIControl), new PropertyMetadata(Styles.Default.HeaderStyle));
        public static readonly DependencyProperty ErrorTextBlockStyleProperty = DependencyProperty.Register("ErrorTextBlockStyle", typeof(Style), typeof(FirebaseUIControl), new PropertyMetadata(Styles.Default.ErrorStyle));
        public static readonly DependencyProperty BodyTextBlockStyleProperty = DependencyProperty.Register("BodyTextBlockStyle", typeof(Style), typeof(FirebaseUIControl), new PropertyMetadata(Styles.Default.BodyStyle));
        public static readonly DependencyProperty ConfirmButtonStyleProperty = DependencyProperty.Register("ConfirmButtonStyle", typeof(Style), typeof(FirebaseUIControl), new PropertyMetadata(Styles.Default.ConfirmButtonStyle));
        public static readonly DependencyProperty CancelButtonStyleProperty = DependencyProperty.Register("CancelButtonStyle", typeof(Style), typeof(FirebaseUIControl), new PropertyMetadata(Styles.Default.CancelButtonStyle));

        private Frame frame;

        public FirebaseUIControl()
        {
            this.DefaultStyleKey = typeof(FirebaseUIControl);
        }

        public event EventHandler<UserEventArgs> AuthStateChanged;

        public Style CancelButtonStyle
        {
            get { return (Style)GetValue(CancelButtonStyleProperty); }
            set { SetValue(CancelButtonStyleProperty, value); }
        }

        public Style ConfirmButtonStyle
        {
            get { return (Style)GetValue(ConfirmButtonStyleProperty); }
            set { SetValue(ConfirmButtonStyleProperty, value); }
        }

        public Style BodyTextBlockStyle
        {
            get { return (Style)GetValue(BodyTextBlockStyleProperty); }
            set { SetValue(BodyTextBlockStyleProperty, value); }
        }

        public Style ErrorTextBlockStyle
        {
            get { return (Style)GetValue(ErrorTextBlockStyleProperty); }
            set { SetValue(ErrorTextBlockStyleProperty, value); }
        }

        public Style HeaderTextBlockStyle
        {
            get { return (Style)GetValue(HeaderTextBlockStyleProperty); }
            set { SetValue(HeaderTextBlockStyleProperty, value); }
        }

        public Style TitleTextBlockStyle
        {
            get { return (Style)GetValue(TitleTextBlockStyleProperty); }
            set { SetValue(TitleTextBlockStyleProperty, value); }
        }

        public object Header
        {
            get { return (object)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.frame = (Frame)this.GetTemplateChild("Frame");

            if (!DesignMode.DesignModeEnabled && !FirebaseUI.IsInitialized)
            {
                var grid = (Grid)this.GetTemplateChild("LoginControl");
                grid.Children.Add(new TextBlock
                {
                    Text = "FirebaseUI has not been initialized yet. Make sure to initialize it during the startup of your application, e.g. in App.xaml.cs",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    TextWrapping = TextWrapping.Wrap
                });
                return;
            }

            this.DataContext = this;
            this.Loaded += this.ControlLoaded;
            this.Unloaded += this.ControlUnloaded;

            this.frame.Navigate(typeof(ProvidersPage), this, new DrillInNavigationTransitionInfo());
        }

        private void ControlUnloaded(object sender, RoutedEventArgs e)
        {
            FirebaseUI.Instance.Client.AuthStateChanged -= ClientAuthStateChanged;
        }

        private void ControlLoaded(object sender, RoutedEventArgs args)
        {
            FirebaseUI.Instance.Client.AuthStateChanged += ClientAuthStateChanged;
        }

        private async void ClientAuthStateChanged(object sender, UserEventArgs e)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                () => this.AuthStateChanged?.Invoke(sender, e));
        }

        void IFirebaseUIFlow.Reset()
        {
            while (this.frame.CanGoBack)
            {
                this.frame.GoBack();
            }
        }

        async Task<string> IFirebaseUIFlow.GetRedirectResponseUriAsync(FirebaseProviderType provider, string redirectUri)
        {
            var result = await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, new Uri(redirectUri), new Uri(FirebaseUI.Instance.Config.RedirectUri));
            return result.ResponseData;
        }

        Task<string> IFirebaseUIFlow.PromptForEmailAsync(string error)
        {
            return NavigateFrame<EmailPage, string>(tcs => (this.Styles, tcs, error));
        }

        Task<EmailUser> IFirebaseUIFlow.PromptForEmailPasswordNameAsync(string email, string error)
        {
            return NavigateFrame<SignUpPage, EmailUser>(tcs => (this.Styles, tcs, email, error));
        }

        Task<EmailPasswordResult> IFirebaseUIFlow.PromptForPasswordAsync(string email, bool oauthEmailAttempt, string error)
        {
            return NavigateFrame<SignInPage, EmailPasswordResult>(tcs => (this.Styles, tcs, email, oauthEmailAttempt, error));
        }

        Task<object> IFirebaseUIFlow.PromptForPasswordResetAsync(string email, string error)
        {
            return NavigateFrame<RecoverPasswordPage, object>(tcs => (this.Styles, tcs, email, error));
        }

        async Task IFirebaseUIFlow.ShowPasswordResetConfirmationAsync(string email)
        {
            await new MessageDialog(string.Format(AppResources.Instance.FuiConfirmRecoveryBody, email), AppResources.Instance.FuiTitleConfirmRecoverPassword).ShowAsync();
        }

        async Task<bool> IFirebaseUIFlow.ShowEmailProviderConflictAsync(string email, FirebaseProviderType providerType)
        {
            var ok = new UICommand("Ok");
            var cancel = new UICommand(AppResources.Instance.FuiCancel);
            var dialog = new MessageDialog(string.Format(AppResources.Instance.FuiWelcomeBackIdpPrompt, email, providerType), AppResources.Instance.FuiWelcomeBackIdpHeader);
            dialog.Commands.Add(ok);
            dialog.Commands.Add(cancel);
            var result = await dialog.ShowAsync();

            return result == ok;
        }

        private Styles Styles => new Styles(this.TitleTextBlockStyle, this.HeaderTextBlockStyle, this.ErrorTextBlockStyle, this.BodyTextBlockStyle, this.ConfirmButtonStyle, this.CancelButtonStyle);

        private Task<TResult> NavigateFrame<TPage, TResult>(Func<TaskCompletionSource<TResult>, object> param)
            where TPage: Page
        {
            var tcs = new TaskCompletionSource<TResult>();

            if (ApiInformation.IsPropertyPresent("Windows.UI.Xaml.Media.Animation.SlideNavigationTransitionInfo", "Effect"))
            {
                this.frame.Navigate(typeof(TPage), param(tcs), new SlideNavigationTransitionInfo { Effect = SlideNavigationTransitionEffect.FromRight });
            }
            else
            {
                this.frame.Navigate(typeof(TPage), param(tcs), new SlideNavigationTransitionInfo());
            }

            return tcs.Task;
        }
    }
}
