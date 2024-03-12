using System.Diagnostics;

namespace Firebase.Auth.UI.MAUI
{
    internal static class Launcher
    {
        public static Task LaunchUriAsync(Uri uri)
        {
            return LaunchUriAsync(uri.ToString());
        }

        public static Task LaunchUriAsync(string uri)
        {
            var ps = new ProcessStartInfo(uri)
            {
                UseShellExecute = true,
                Verb = "open"
            };
            return Process.Start(ps).WaitForExitAsync();
        }
    }
}
