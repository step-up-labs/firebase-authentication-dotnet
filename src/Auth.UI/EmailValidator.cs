using System.Text.RegularExpressions;

namespace Firebase.Auth.UI
{
    public static class EmailValidator
    {
        /// <summary>
        /// Checks whether given string is a valid email address.
        /// </summary>
        public static bool ValidateEmail(string email)
        {
            var regx = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            var match = regx.Match(email);
            return match.Success;
        }
    }
}
