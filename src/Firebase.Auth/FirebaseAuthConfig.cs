using Firebase.Auth.Providers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Firebase.Auth
{
    /// <summary>
    /// Configuration of firebase auth.
    /// </summary>
    public class FirebaseAuthConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FirebaseAuthConfig"/> class with your API key.
        /// </summary>
        /// <param name="apiKey"> The api key of your Firebase app. </param>
        /// <param name="authDomain"> Auth domain of your firebase app, e.g. hello.firebaseapp.com. </param>
        /// <param name="externalSignInDelegate"> Delegate invoked to perform in-browser sign in. </param>
        public FirebaseAuthConfig(string apiKey, string authDomain, ExternalSignInDelegate externalSignInDelegate, params FirebaseAuthProvider[] providers)
            : this()
        {
            this.ApiKey = apiKey;
            this.AuthDomain = authDomain;
            this.ExternalSignInDelegate = externalSignInDelegate;
            this.Providers = providers;
        }

        public FirebaseAuthConfig()
        {
            this.HttpClient = new HttpClient();
            this.JsonSettings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
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
        /// Delegate invoked to perform in-browser sign in.
        /// </summary>
        public ExternalSignInDelegate ExternalSignInDelegate 
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
    }
}
