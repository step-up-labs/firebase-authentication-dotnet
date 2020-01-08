using Newtonsoft.Json;
using System;
using System.Diagnostics;

namespace Firebase.Auth
{
    /// <summary>
    /// Parser of HTTP response errors into <see cref="AuthErrorReason"/> enum.
    /// </summary>
    internal static class FirebaseFailureParser
    {
        /// <summary>
        /// Resolves failure reason flags based on the returned error code.
        /// </summary>
        public static AuthErrorReason GetFailureReason(string responseData)
        {
            if (string.IsNullOrEmpty(responseData) || responseData == "N/A")
            {
                return AuthErrorReason.Undefined;
            }

            try
            {
                //create error data template and try to parse JSON
                var errorData = new { error = new { code = 0, message = "errorid" } };
                errorData = JsonConvert.DeserializeAnonymousType(responseData, errorData);

                //errorData is just null if different JSON was received
                switch (errorData?.error?.message)
                {
                    //general errors
                    case "invalid access_token, error code 43.":
                        return AuthErrorReason.InvalidAccessToken;
                    case "CREDENTIAL_TOO_OLD_LOGIN_AGAIN":
                        return AuthErrorReason.LoginCredentialsTooOld;
                    case "OPERATION_NOT_ALLOWED":
                        return AuthErrorReason.OperationNotAllowed;
                    case "API key not valid. Please pass a valid API key.":
                        return AuthErrorReason.InvalidApiKey;

                    //possible errors from Third Party Authentication using GoogleIdentityUrl
                    case "INVALID_PROVIDER_ID : Provider Id is not supported.":
                        return AuthErrorReason.InvalidProviderID;
                    case "MISSING_REQUEST_URI":
                        return AuthErrorReason.MissingRequestURI;
                    case "A system error has occurred - missing or invalid postBody":
                        return AuthErrorReason.SystemError;

                    //possible errors from Email/Password Account Signup (via signupNewUser or setAccountInfo) or Signin
                    case "INVALID_EMAIL":
                        return AuthErrorReason.InvalidEmailAddress;
                    case "MISSING_PASSWORD":
                        return AuthErrorReason.MissingPassword;

                    //possible errors from Email/Password Account Signup (via signupNewUser or setAccountInfo)
                    case "EMAIL_EXISTS":
                        return AuthErrorReason.EmailExists;

                    //possible errors from Account Delete
                    case "USER_NOT_FOUND":
                        return AuthErrorReason.UserNotFound;

                    //possible errors from Email/Password Signin
                    case "INVALID_PASSWORD":
                        return AuthErrorReason.WrongPassword;
                    case "EMAIL_NOT_FOUND":
                        return AuthErrorReason.UnknownEmailAddress;
                    case "USER_DISABLED":
                        return AuthErrorReason.UserDisabled;

                    //possible errors from Email/Password Signin or Password Recovery or Email/Password Sign up using setAccountInfo
                    case "MISSING_EMAIL":
                        return AuthErrorReason.MissingEmail;
                    case "RESET_PASSWORD_EXCEED_LIMIT":
                        return AuthErrorReason.ResetPasswordExceedLimit;
                        
                    //possible errors from Password Recovery
                    case "MISSING_REQ_TYPE":
                        return AuthErrorReason.MissingRequestType;

                    //possible errors from Account Linking
                    case "INVALID_ID_TOKEN":
                        return AuthErrorReason.InvalidIDToken;

                    //possible errors from Getting Linked Accounts
                    case "INVALID_IDENTIFIER":
                        return AuthErrorReason.InvalidIdentifier;
                    case "MISSING_IDENTIFIER":
                        return AuthErrorReason.MissingIdentifier;
                    case "FEDERATED_USER_ID_ALREADY_LINKED":
                        return AuthErrorReason.AlreadyLinked;
                }

                if (errorData?.error?.message?.StartsWith("WEAK_PASSWORD :") ?? false)
                {
                    // possible errors from Email/Password Account Signup (via signupNewUser or setAccountInfo)
                    return AuthErrorReason.WeakPassword;
                }
                else if (errorData?.error?.message?.StartsWith("TOO_MANY_ATTEMPTS_TRY_LATER :") ?? false)
                {
                    // possible errors from Email/Password Signin
                    return AuthErrorReason.TooManyAttemptsTryLater;
                }
                else if (errorData?.error?.message?.Contains("Bad access token") ?? false)
                {
                    return AuthErrorReason.InvalidAccessToken;
                }
                
            }
            catch (JsonReaderException)
            {
                //the response wasn't JSON - no data to be parsed
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Unexpected error trying to parse the response: {e}");
            }

            return AuthErrorReason.Unknown;
        }

    }
}
