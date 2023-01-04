namespace Firebase.Auth.UI
{
    /// <summary>
    /// Details of a user created via <see cref="FirebaseProviderType.EmailAndPassword"/>.
    /// </summary>
    public class EmailUser
    {
        public string DisplayName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }
    }
}
