using Firebase.Auth.UI.MAUI.Pages;

namespace Firebase.Auth.UI.MAUI
{
    public static class Router
    {
        public enum NavigationModeEnum { Shell, Stack, Direct, StackModal }
        public static NavigationModeEnum NavigationMode { get; private set; } = NavigationModeEnum.Stack;
        public static string MainPath { get; private set; }
        private static bool IsCustomMainRegister { get; set; }
        public static Type MainType { get; private set; } = typeof(FirebaseMainPage);

        public static void RegisterMainType<TMainPage>(NavigationModeEnum navigationMode) where TMainPage : Page
        {
            NavigationMode = navigationMode;
            MainType = typeof(TMainPage);
        }

        public static void RegisterShellRoutes<TMainPage>(string mainPath = "Login") where TMainPage : Page
        {
            MainType = typeof(TMainPage);
            IsCustomMainRegister = true;
            RegisterShellRoutes(mainPath);
            Routing.RegisterRoute(mainPath, typeof(TMainPage));
        }

        public static void RegisterShellRoutes(string mainPath = "Login")
        {
            MainPath = mainPath;
            NavigationMode = NavigationModeEnum.Shell;
            Routing.RegisterRoute(GetPath<FirebaseMainPage>(), typeof(FirebaseMainPage));
            Routing.RegisterRoute(GetPath<ProvidersPage>(), typeof(ProvidersPage));
            Routing.RegisterRoute(GetPath<EmailPage>(), typeof(EmailPage));
            Routing.RegisterRoute(GetPath<SignUpPage>(), typeof(SignUpPage));
            Routing.RegisterRoute(GetPath<SignInPage>(), typeof(SignInPage));
            Routing.RegisterRoute(GetPath<RecoverPasswordPage>(), typeof(RecoverPasswordPage));
        }

        public static Task NavigateToMain()
        {
            return UIFLow.Instance.NavigateToMain();
        }

        public static string GetMainPath()
        {
            return IsCustomMainRegister ? MainPath : GetPath<FirebaseMainPage>();
        }

        public static string GetPath<T>() where T : class
        {
            return GetPath(typeof(T));
        }

        public static string GetPath(Type type)
        {
            return $"{MainPath}/{type.Name}";
        }
    }
}
