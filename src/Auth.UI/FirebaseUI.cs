using System;
using System.Collections.Generic;
using System.Linq;

namespace Firebase.Auth.UI
{
    public class FirebaseUI
    {
        private static FirebaseUI firebaseUI;
        private readonly FirebaseUIConfig config;

        private FirebaseUI(FirebaseUIConfig config)
        {
            this.config = config;
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

        public FirebaseAuthClient Client { get; }

        public static FirebaseUI Instance => firebaseUI ?? throw new InvalidOperationException("FirebaseUI hasn't been initialized yet.");

        public IReadOnlyCollection<FirebaseProviderType> Providers { get; }

        public string TermsOfServiceUrl => this.config.TermsOfServiceUrl;
        
        public string PrivacyPolicyUrl => this.config.PrivacyPolicyUrl;

        public bool IsAnonymousAllowed => this.config.IsAnonymousAllowed;
    }
}
