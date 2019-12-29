namespace Firebase.Auth.UI
{
    /// <summary>
    /// Result of the prompt for password.
    /// </summary>
    public class EmailPasswordResult
    {
        /// <summary>
        /// Password entered by user. Undefinied if <see cref="ResetPassword"/> it true.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// If true it means user asked to reset password.
        /// </summary>
        public bool ResetPassword { get; set; }

        public static EmailPasswordResult WithPassword(string password) => new EmailPasswordResult { Password = password };
        
        public static EmailPasswordResult Reset() => new EmailPasswordResult { ResetPassword = true };
    }
}
