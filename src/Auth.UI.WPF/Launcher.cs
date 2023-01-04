using System;
using System.Diagnostics;

namespace Firebase.Auth.UI
{
    internal static class Launcher
    {
        public static void LaunchUri(Uri uri)
        {
            LaunchUri(uri.ToString());
        }

        public static void LaunchUri(string uri)
        {
            var ps = new ProcessStartInfo(uri)
            {
                UseShellExecute = true,
                Verb = "open"
            };
            Process.Start(ps);
        }
    }
}
