using Firebase.Auth.Providers;
using Firebase.Auth.Repository;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Net.Http;

namespace Firebase.Auth
{
    /// <summary>
    /// Configuration of firebase auth.
    /// </summary>
    public class FirebaseAuthConfig
    {
        public FirebaseAuthConfig()
        {
            this.HttpClient = new HttpClient();
            this.UserRepository = InMemoryRepository.Instance;
            this.Providers = Array.Empty<FirebaseAuthProvider>();
            this.JsonSettings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                },
                DefaultValueHandling = DefaultValueHandling.Ignore
            };
            this.JsonSettings.Converters.Add(new StringEnumConverter { CamelCaseText = true });
        }

        /// <summary>
        /// The api key of your Firebase app. 
        /// </summary>
        public string ApiKey
        {
            get;
            set;
        }

        /// <summary>
        /// Repository of firebase tokens. Default is in-memory.
        /// </summary>
        public IUserRepository UserRepository 
        { 
            get;
            set;
        }

        /// <summary>
        /// Collection of auth providers (e.g. Google, Facebook etc.)
        /// </summary>
        public FirebaseAuthProvider[] Providers 
        { 
            get;
            set;
        }

        /// <summary>
        /// Json.net serializer settings.
        /// </summary>
        public JsonSerializerSettings JsonSettings 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// HttpClient to be used for web requests.
        /// </summary>
        public HttpClient HttpClient 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// Auth domain of your firebase app, e.g. hello.firebaseapp.com
        /// </summary>
        public string AuthDomain
        {
            get;
            set;
        }

        /// <summary>
        /// Specifies the uri that oauth provider will navigate to to finish auth.
        /// </summary>
        public string RedirectUri => $"https://{this.AuthDomain}/__/auth/handler";
    }
}
