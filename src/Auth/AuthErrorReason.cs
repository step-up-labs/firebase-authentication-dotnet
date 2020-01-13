namespace Firebase.Auth
{
    public enum AuthErrorReason
    {
        /// <summary>
        /// Request didn't even complete, possibly due to network issue.
        /// </summary>
        Undefined,
        /// <summary>
        /// Unknown error reason.
        /// </summary>
        Unknown,
        /// <summary>
        /// The sign in method is not enabled.
        /// </summary>
        OperationNotAllowed,
        /// <summary>
        /// The user was disabled and is not granted access anymore.
        /// </summary>
        UserDisabled,
        /// <summary>
        /// The user was not found
        /// </summary>
        UserNotFound,
        /// <summary>
        /// Third-party Auth Providers: PostBody does not contain or contains invalid Authentication Provider string.
        /// </summary>
        InvalidProviderID,
        /// <summary>
        /// Third-party Auth Providers: PostBody does not contain or contains invalid Access Token string obtained from Auth Provider.
        /// </summary>
        InvalidAccessToken,
        /// <summary>
        /// Changes to user's account has been made since last log in. User needs to login again.
        /// </summary>
        LoginCredentialsTooOld,
        /// <summary>
        /// Third-party Auth Providers: Request does not contain a value for parameter: requestUri.
        /// </summary>
        MissingRequestURI,
        /// <summary>
        /// Third-party Auth Providers: Request does not contain a value for parameter: postBody.
        /// </summary>
        SystemError,
        /// <summary>
        /// Email/Password Authentication: Email address is not in valid format.
        /// </summary>
        InvalidEmailAddress,
        /// <summary>
        /// Email/Password Authentication: No password provided!
        /// </summary>
        MissingPassword,
        /// <summary>
        /// Email/Password Signup: Password must be more than 6 characters.  This error could also be caused by attempting to create a user account using Set Account Info without supplying a password for the new user.
        /// </summary>
        WeakPassword,
        /// <summary>
        /// Email/Password Signup: Email address already connected to another account. This error could also be caused by attempting to create a user account using Set Account Info and an email address already linked to another account.
        /// </summary>
        EmailExists,
        /// <summary>
        /// Email/Password Signin: No email provided! This error could also be caused by attempting to create a user account using Set Account Info without supplying an email for the new user.
        /// </summary>
        MissingEmail,
        /// <summary>
        /// Email/Password Signin: No user with a matching email address is registered.
        /// </summary>
        UnknownEmailAddress,
        /// <summary>
        /// Email/Password Signin: The supplied password is not valid for the email address.
        /// </summary>
        WrongPassword,
        /// <summary>
        /// Email/Password Signin: Too many password login have been attempted. Try again later.
        /// </summary>
        TooManyAttemptsTryLater,
        /// <summary>
        /// Password Recovery: Request does not contain a value for parameter: requestType or supplied value is invalid.
        /// </summary>
        MissingRequestType,
        /// <summary>
        /// Password Recovery: Reset password limit exceeded.
        /// </summary>
        ResetPasswordExceedLimit,
        /// <summary>
        /// Account Linking: Authenticated User ID Token is invalid!
        /// </summary>
        InvalidIDToken,
        /// <summary>
        /// Linked Accounts: Request does not contain a value for parameter: identifier.
        /// </summary>
        MissingIdentifier,
        /// <summary>
        /// Linked Accounts: Request contains an invalid value for parameter: identifier.
        /// </summary>
        InvalidIdentifier,
        /// <summary>
        /// Linked accounts: account to link has already been linked.
        /// </summary>
        AlreadyLinked,
        /// <summary>
        /// Specified API key is not valid.
        /// </summary>
        InvalidApiKey,
        /// <summary>
        /// The email user tried to sign in with is already registered under a different provider.
        /// </summary>
        AccountExistsWithDifferentCredential
    }
}
