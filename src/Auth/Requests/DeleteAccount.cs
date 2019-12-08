namespace Firebase.Auth.Requests
{
    /// <summary>
    /// Deletes user's account.
    /// </summary>
    public class DeleteAccount : FirebaseRequestBase<IdTokenRequest, object>
    {
        public DeleteAccount(FirebaseAuthConfig config) : base(config)
        {
        }

        protected override string UrlFormat => Endpoints.GoogleDeleteUserUrl;
    }
}
