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
                ApiKey = "Empty",
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
        /// Specifies whether <see cref="Initialize(FirebaseUIConfig)"/> has been called yet.
        /// </summary>
        public static bool IsInitialized => firebaseUI != null;

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
        public virtual async Task<UserCredential> SignInAsync(IFirebaseUIFlow flow, FirebaseProviderType provider)
        {
            try
            {
                switch (provider)
                {
                    case FirebaseProviderType.EmailAndPassword:
                        return await this.SignInWithEmailAsync(flow);
                    case FirebaseProviderType.Anonymous:
                        return await this.Client.SignInAnonymouslyAsync();
                    default:
                        return await this.SignInWithRedirectAsync(flow, provider);
                }
            }
            catch (FirebaseAuthLinkConflictException ex)
            {
                return await this.HandleConflictAsync(flow, ex);
            }
            finally
            {
                flow.Reset();
            }
        }

        protected virtual async Task<UserCredential> SignInWithRedirectAsync(IFirebaseUIFlow flow, FirebaseProviderType provider)
        {
            try
            {
                if (this.UpgradeAnonymousUser)
                {
                    return await this.Client.User.LinkWithRedirectAsync(provider, uri => flow.GetRedirectResponseUriAsync(provider, uri));
                }

                return await this.Client.SignInWithRedirectAsync(provider, uri => flow.GetRedirectResponseUriAsync(provider, uri));
            }
            catch (FirebaseAuthWithCredentialException e) when (e.Reason == AuthErrorReason.AlreadyLinked)
            {
                return await this.Config.RaiseUpgradeConflictAsync(this.Client, e.Credential);
            }
            catch (FirebaseAuthWithCredentialException e) when (e.Reason == AuthErrorReason.EmailExists)
            {
                // trigger email login (password screen)
                // link credential with email
                // link result with anonymous
                return await this.SignInWithEmailAsync(flow, e.Email, e.Credential);
            }
        }

        protected virtual async Task<UserCredential> SignInWithEmailAsync(IFirebaseUIFlow flow)
        {
            var fetchResult = await this.RetryAction(e => flow.PromptForEmailAsync(e), email => this.Client.FetchSignInMethodsForEmailAsync(email));

            if (fetchResult == null)
            {
                return null;
            }

            if (fetchResult.SignInProviders.Any() && !fetchResult.SignInProviders.Contains(FirebaseProviderType.EmailAndPassword))
            {
                throw new FirebaseAuthLinkConflictException(fetchResult.Email, fetchResult.SignInProviders);
            }

            var email = fetchResult.Email;

            if (fetchResult.UserExists)
            {
                return await this.SignInWithEmailAsync(flow, email);
            }

            return await this.RetryAction(
                e => flow.PromptForEmailPasswordNameAsync(email, e),
                async user =>
                {
                    if (this.UpgradeAnonymousUser)
                    {
                        var userCredential = await this.Client.User.LinkWithCredentialAsync(EmailProvider.GetCredential(user.Email, user.Password));

                        if (!string.IsNullOrWhiteSpace(user.DisplayName))
                        {
                            await userCredential.User.ChangeDisplayNameAsync(user.DisplayName);
                        }

                        return userCredential;
                    }

                    return await this.Client.CreateUserWithEmailAndPasswordAsync(user.Email, user.Password, user.DisplayName);
                });
        }

        protected virtual Task<UserCredential> SignInWithEmailAsync(IFirebaseUIFlow flow, string email, AuthCredential pendingCredential = null)
        {
            var provider = (EmailProvider)this.Config.GetAuthProvider(FirebaseProviderType.EmailAndPassword);

            // This method can recursively call itself in case user asks to recover email password
            // - it shows reset page
            // - after reset can try to enter password again
            return this.RetryAction(
                e => flow.PromptForPasswordAsync(email, pendingCredential != null, e),
                async result =>
                {
                    if (result.ResetPassword)
                    {
                        var reset = await this.RetryAction(
                            e => flow.PromptForPasswordResetAsync(email, e),
                            async res =>
                            {
                                await this.Client.ResetEmailPasswordAsync(email);
                                await flow.ShowPasswordResetConfirmationAsync(email);
                                return true;
                            });

                        if (!reset)
                        {
                            return null;
                        }

                        return await SignInWithEmailAsync(flow, email);
                    }

                    var userCredential = await provider.SignInUserAsync(email, result.Password);

                    if (pendingCredential != null)
                    {
                        // pending credential should be linked with the existing email credential
                        userCredential = await userCredential.User.LinkWithCredentialAsync(pendingCredential);
                    }

                    if (this.UpgradeAnonymousUser)
                    {
                        userCredential = await this.Config.RaiseUpgradeConflictAsync(this.Client, userCredential.AuthCredential);
                    }

                    return userCredential;
                });
        }

        protected virtual async Task<UserCredential> HandleConflictAsync(IFirebaseUIFlow flow, FirebaseAuthLinkConflictException exception)
        {
            var provider = exception.Providers.First();
            if (!await flow.ShowEmailProviderConflictAsync(exception.Email, provider))
            {
                return null;
            }

            return await this.SignInAsync(flow, provider);
        }

        protected async Task<TOut> RetryAction<TIn, TOut>(Func<string, Task<TIn>> func, Func<TIn, Task<TOut>> func2)
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
        
        private bool UpgradeAnonymousUser
        {
            get => this.Config.AutoUpgradeAnonymousUsers && (this.Client.User?.Info.IsAnonymous ?? false);
        }

    }
}
