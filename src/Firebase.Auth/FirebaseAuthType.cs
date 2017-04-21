using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Firebase.Auth
{
    /// <summary>
    /// The type of authentication. 
    /// </summary>
    public enum FirebaseAuthType
    {
        /// <summary>
        /// The facebook auth.
        /// </summary>
        [EnumMember(Value = "facebook.com")]
        Facebook,

        /// <summary>
        /// The google auth.
        /// </summary>
        [EnumMember(Value = "google.com")]
        Google,

        /// <summary>
        /// The github auth.
        /// </summary>
        [EnumMember(Value = "github.com")]
        Github,

        /// <summary>
        /// The twitter auth. 
        /// </summary> 
        [EnumMember(Value = "twitter.com")]
        Twitter,

        /// <summary>
        /// Auth using email and password.
        /// </summary>
        [EnumMember(Value = "password")]
        EmailAndPassword
    } 
}
