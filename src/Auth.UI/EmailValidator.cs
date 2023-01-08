using System;
using System.Net.Mail;

namespace Firebase.Auth.UI
{
    public static class EmailValidator
    {
        /// <summary>
        /// Checks whether given string is a valid email address.
        /// </summary>
        public static bool ValidateEmail(string email)
        {
            try
            {
                var m = new MailAddress(email);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
