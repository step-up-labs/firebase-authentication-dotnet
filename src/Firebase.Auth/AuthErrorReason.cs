namespace Firebase.Auth
{
    public enum AuthErrorReason
    {
        /// <summary>
        /// Unknown error reason.
        /// </summary>
        Undefined,
        /// <summary>
        /// No user with a matching email address is registered.
        /// </summary>
        UnknownEmailAddress,
        /// <summary>
        /// The supplied ID is not a valid email address.
        /// </summary>
        InvalidEmailAddress,
        /// <summary>
        /// Wrong password.
        /// </summary>
        WrongPassword,
        /// <summary>
        /// The user was disabled and is not granted access anymore.
        /// </summary>
        UserDisabled

    }
}