namespace Firebase.Auth
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Base class for all FirebaseAuth exceptions.
    /// </summary>
    public class FirebaseAuthException : Exception
    {
        public FirebaseAuthException(string message, AuthErrorReason reason)
            : base(message)
        {
            this.Reason = reason;
        }

        public FirebaseAuthException(string message, Exception innerException, AuthErrorReason reason)
            : base(message, innerException)
        {
            this.Reason = reason;
        }
        
        public FirebaseAuthException(Exception innerException, AuthErrorReason reason)
            : this($"Firebase exception occured: {reason}", innerException, reason)
        {
        }

        /// <summary>
        /// Indicates why a login failed. If not resolved, defaults to <see cref="AuthErrorReason.Undefined"/>.
        /// </summary>
        public AuthErrorReason Reason
        {
            get;
        }
    }

    /// <summary>
    /// Exception thrown when user tries to login with email that is already associated with a different Auth provider
    /// (and creating multiple accounts using the same email address with different authentication providers is not allowed in Firebase Console)
    /// </summary>
    public class FirebaseAuthLinkConflictException : FirebaseAuthException
    {
        public FirebaseAuthLinkConflictException(string email, IEnumerable<FirebaseProviderType> providers) 
            : base($"An account already exists with the same email address but different sign-in credentials. Sign in using a provider associated with this email address: {email}", AuthErrorReason.AccountExistsWithDifferentCredential)
        {
            this.Email = email;
            this.Providers = providers;
        }

        public string Email { get; }

        public IEnumerable<FirebaseProviderType> Providers { get; }
    }

    public class FirebaseAuthWithCredentialException : FirebaseAuthException
    {
        public FirebaseAuthWithCredentialException(string message, AuthCredential credential, AuthErrorReason reason)
            : base(message, reason)
        {
            this.Credential = credential;
        }

        public FirebaseAuthWithCredentialException(string message, string email, AuthCredential credential, AuthErrorReason reason)
            : base(message, reason)
        {
            this.Credential = credential;
            this.Email = email;
        }

        public AuthCredential Credential { get; }

        public string Email { get; set; }
    }

    /// <summary>
    /// Exception thrown during invocation of an HTTP request.
    /// </summary>
    public class FirebaseAuthHttpException : FirebaseAuthException
    {
        public FirebaseAuthHttpException(Exception innerException, string requestUrl, string requestData, string responseData, AuthErrorReason reason = AuthErrorReason.Undefined) 
            : base(GenerateExceptionMessage(requestUrl, requestData, responseData, reason), innerException, reason)
        {
            this.RequestUrl = requestUrl;
            this.RequestData = requestData;
            this.ResponseData = responseData;
        }

        /// <summary>
        /// Json data passed to the authentication service.
        /// </summary>
        public string RequestData
        {
            get;
        }
        
        /// <summary>
        /// Url for which the request failed.
        /// </summary>
        public string RequestUrl
        {
            get;
        }

        /// <summary>
        /// Response from the authentication service.
        /// </summary>
        public string ResponseData
        {
            get;
        }

        private static string GenerateExceptionMessage(string requestUrl, string requestData, string responseData, AuthErrorReason errorReason)
        {
            return $"Exception occured during Firebase Http request.\nUrl: {requestUrl}\nRequest Data: {requestData}\nResponse: {responseData}\nReason: {errorReason}";
        }
    }
}
