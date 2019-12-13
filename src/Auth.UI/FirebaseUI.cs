using Firebase.Auth.Providers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Firebase.Auth.UI
{
    public class FirebaseUI
    {
        private static FirebaseUI firebaseUI;
        
        private FirebaseUI(FirebaseUIConfig config)
        {
            this.Config = config;
            this.Client = new FirebaseAuthClient(config);
            this.Providers = config.Providers.Select(p => p.ProviderType).ToArray();
        }

        public static FirebaseUI Initialize(FirebaseUIConfig config)
        {
            if (firebaseUI != null)
            {
                throw new InvalidOperationException("FirebaseUI has already been initialized");
            }

            return firebaseUI = new FirebaseUI(config);
        }

        public static FirebaseUI InitializeEmpty()
        {
            return firebaseUI ?? (firebaseUI = new FirebaseUI(new FirebaseUIConfig
            {
                PrivacyPolicyUrl = "https://github.com/step-up-labs/firebase-authentication-dotnet/",
                TermsOfServiceUrl = "https://github.com/step-up-labs/firebase-authentication-dotnet/",
                AuthDomain = "test",
                Providers = new FirebaseAuthProvider[]
                    {
                        new FacebookProvider(),
                        new GoogleProvider(),
                        new TwitterProvider(),
                    }
            }));
        }

        public FirebaseAuthClient Client { get; }

        public static FirebaseUI Instance => firebaseUI ?? throw new InvalidOperationException("FirebaseUI hasn't been initialized yet.");

        public IReadOnlyCollection<FirebaseProviderType> Providers { get; }

        public FirebaseUIConfig Config { get; }
    }
}
