using Firebase.Auth.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Firebase.Auth.UI
{
    /// <summary>
    /// Singleton entrypoint to all things related to Firebase UI. Call <see cref="Initialize(FirebaseUIConfig)"/> 
    /// during application startup to initialize FirebaseUI. It will also initialize <see cref="Client"/> you can use 
    /// to call other Firebase methods and subscribe to <see cref="FirebaseAuthClient.AuthStateChanged"/> event to be
    /// notified about auth changes.
    /// </summary>
    public class FirebaseUI
    {
        private static FirebaseUI firebaseUI;
        
        private FirebaseUI(FirebaseUIConfig config)
        {
            if (string.IsNullOrWhiteSpace(config.PrivacyPolicyUrl) || string.IsNullOrWhiteSpace(config.TermsOfServiceUrl))
            {
                throw new ArgumentException($"Both {nameof(config.PrivacyPolicyUrl)} and {nameof(config.TermsOfServiceUrl)} must be set.");
            }

            this.Client = new FirebaseAuthClient(config);
            this.Providers = config.Providers.Select(p => p.ProviderType).ToArray();
            this.Config = config;
        }

        /// <summary>
        /// Initializes a singleton instance of <see cref="FirebaseUI"/>. Call this method only once during application startup.
        /// </summary>
        public static FirebaseUI Initialize(FirebaseUIConfig config)
        {
            if (firebaseUI != null)
            {
                throw new InvalidOperationException("FirebaseUI has already been initialized");
            }

            return firebaseUI = new FirebaseUI(config);
        }

        /// <summary>
        /// Do not call this method in your code directly. Initializes an empty, dummy instance of FirebaseUI.
        /// </summary>
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

        /// <summary>
        /// Firebase client which can interact which remote firebase servers.
        /// </summary>
        public FirebaseAuthClient Client { get; }

        /// <summary>
        /// Singleton instance of <see cref="FirebaseUI"/>. Available only after <see cref="Initialize(FirebaseUIConfig)"/> has been called;
        /// </summary>
        public static FirebaseUI Instance => firebaseUI ?? throw new InvalidOperationException("FirebaseUI hasn't been initialized yet.");

        /// <summary>
        /// Callection of registered auth providers.
        /// </summary>
        public IReadOnlyCollection<FirebaseProviderType> Providers { get; }

        /// <summary>
        /// Config supplied via <see cref="Initialize(FirebaseUIConfig)"/>.
        /// </summary>
        public FirebaseUIConfig Config { get; }

        /// <summary>
        /// Do not call this method in your code directly, it is called automatically from a UI Control after user clicks a button to sign in.
        /// Performs sign in using given flow and provider.
        /// </summary>
        /// <param name="flow"> Sign in flow which contains platform specific UI actions. </param>
        /// <param name="provider"> Provider to use for sign in. </param>
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
                var exists = await this.RetryAction(e => flow.PromptForEmailAsync(e), email => this.Client.CheckUserEmailExistsAsync(email));

                if (exists == null)
                {
                    return null;
                }

                var email = exists.Email;

                if (exists.UserExists)
                {
                    Func<Task<User>> signInUserFunc = null;
                    
                    // This func can recursively call itself in case user asks to recover email password
                    // - it shows reset page
                    // - after reset can try to enter password again
                    signInUserFunc = () =>
                    {
                        return this.RetryAction(
                            e => flow.PromptForPasswordAsync(email, e),
                            async result =>
                            {
                                if (result.ResetPassword)
                                {
                                    await this.RetryAction(
                                        e => flow.PromptForPasswordResetAsync(email, e),
                                        async res => 
                                        {
                                            await this.Client.ResetEmailPasswordAsync(email);
                                            await flow.ShowPasswordResetConfirmationAsync(email);
                                            return true;
                                        });

                                    return await signInUserFunc();
                                }

                                return await this.Client.SignInWithEmailAndPasswordAsync(email, result.Password);
                            });
                    };

                    return await signInUserFunc();
                }

                return await this.RetryAction(
                    e => flow.PromptForEmailPasswordNameAsync(email, e),
                    user => this.Client.CreateUserWithEmailAndPasswordAsync(user.Email, user.Password, user.DisplayName));
            }
            finally
            {
                flow.Reset();
            }
        }

        private async Task<TOut> RetryAction<TIn, TOut>(Func<string, Task<TIn>> func, Func<TIn, Task<TOut>> func2)
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
