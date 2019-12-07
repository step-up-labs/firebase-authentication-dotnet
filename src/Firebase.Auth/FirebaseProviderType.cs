using System.Runtime.Serialization;

namespace Firebase.Auth
{
    /// <summary>
    /// Type of authentication provider.
    /// </summary>
    public enum FirebaseProviderType
    {
        [EnumMember(Value = "facebook.com")]
        Facebook,

        [EnumMember(Value = "google.com")]
        Google,

        [EnumMember(Value = "github.com")]
        Github,

        [EnumMember(Value = "twitter.com")]
        Twitter,

        [EnumMember(Value = "microsoft.com")]
        Microsoft,

        [EnumMember(Value = "password")]
        EmailAndPassword,
        
    } 
}
