namespace Firebase.Auth
{
    public enum OperationType
    {
        SignIn,
        Link,
        Reauthenticate
    }

    /// <summary>
    /// A structure containing a <see cref="User"/>, an <see cref="AuthCredential"/> and <see cref="OperationType"/>.
    /// </summary>
    public class UserCredential
    {
        public UserCredential(User user, AuthCredential authCredential, OperationType op)
        {
            this.User = user;
            this.AuthCredential = authCredential;
            this.OperationType = op;
        }

        public User User { get; set; }

        public AuthCredential AuthCredential { get; set; }

        public OperationType OperationType { get; set; }
    }
}
