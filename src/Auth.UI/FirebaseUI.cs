using Firebase.Auth.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Firebase.Auth.UI
{
    public class FirebaseUI
    {
        private static FirebaseUI firebaseUI;
        
        private FirebaseUI(FirebaseUIConfig config)
        {
            this.Client = new FirebaseAuthClient(config);
            this.Providers = config.Providers.Select(p => p.ProviderType).ToArray();
            this.Config = config;
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

        public virtual Task<User> SignInAsync(IFirebaseUIFlow flow, FirebaseProviderType provider)
        {
            switch (provider)
            {
                case FirebaseProviderType.EmailAndPassword:
                    return this.SignInWithEmailAsync(flow);
                case FirebaseProviderType.Anonymous:
                    return this.Client.SignInAnonymouslyAsync();
                default:
                    return flow.SignInExternallyAsync(provider);
            }
        }

        private async Task<User> SignInWithEmailAsync(IFirebaseUIFlow flow)
        {
            try
            {
                var exists = await this.RetryAction(flow, e => flow.PromptForEmailAsync(e), email => this.Client.CheckUserEmailExistsAsync(email));

                if (exists == null)
                {
                    return null;
                }

                var email = exists.Email;

                if (exists.UserExists)
                {
                    return await this.RetryAction(flow,
                        e => flow.PromptForPasswordAsync(email, e),
                        password => this.Client.SignInWithEmailAndPasswordAsync(email, password));
                }

                return await this.RetryAction(flow,
                    e => flow.PromptForEmailPasswordNameAsync(email, e),
                    user => this.Client.CreateUserWithEmailAndPasswordAsync(user.Email, user.Password, user.DisplayName));
            }
            finally
            {
                flow.Reset();
            }
        }

        private async Task<TOut> RetryAction<TIn, TOut>(IFirebaseUIFlow flow, Func<string, Task<TIn>> func, Func<TIn, Task<TOut>> func2)
        {
            var error = "";

            while (true)
            {
                try
                {
                    var result = await func(error);

                    if (result == null)
                    {
                        return default;
                    }

                    return await func2(result);
                }
                catch (FirebaseAuthException ex)
                {
                    error = FirebaseErrorLookup.LookupError(ex);
                }
            }
        }
    }
}
