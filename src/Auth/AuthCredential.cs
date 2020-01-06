namespace Firebase.Auth
{
    /// <summary>
    /// Base class for provider-specific AuthCredentials.
    /// </summary>
    public abstract class AuthCredential
    {
        public FirebaseProviderType ProviderType { get; set; }
    }
}
