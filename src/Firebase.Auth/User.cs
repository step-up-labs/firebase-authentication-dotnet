namespace Firebase.Auth
{
    /// <summary>
    /// Basic information about the signed in user.
    /// </summary>
    public class User
    {
        public string LocalId { get; set; }

        public string FederatedId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string DisplayName { get; set; }

        public string Email { get; set; }

        public bool IsEmailVerified { get; set; }

        public string PhotoUrl { get; set; }
    }
}
