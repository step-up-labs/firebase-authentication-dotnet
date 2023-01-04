using Firebase.Auth.Providers;
using System;
using System.Linq;
using System.Threading.Tasks;
using static System.Console;

namespace Firebase.Auth.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            Run().Wait();
        }

        private static async Task Run()
        {
            var config = new FirebaseAuthConfig
            {
                ApiKey = "<YOUR API KEY>",
                AuthDomain = "<YOUR PROJECT DOMAIN>.firebaseapp.com",
                Providers = new FirebaseAuthProvider[]
                {
                    new GoogleProvider(),
                    new FacebookProvider(),
                    new TwitterProvider(),
                    new GithubProvider(),
                    new MicrosoftProvider(),
                    new EmailProvider()
                }
            };

            if (config.ApiKey == "<YOUR API KEY>" || config.AuthDomain == "<YOUR PROJECT DOMAIN>.firebaseapp.com")
            {
                WriteLine("You need to setup your API key and auth domain first in Program.cs");
                return;
            }

            var client = new FirebaseAuthClient(config);

            WriteLine("How do you want to sign in?");
            config.Providers.Select((provider, i) => (provider, i)).ToList().ForEach(p => WriteLine($"[{p.i}]: {p.provider.ProviderType}"));
            WriteLine($"[{config.Providers.Count()}]: Anonymously");

            var i = int.Parse(ReadLine());
            UserCredential userCredential;

            if (i == config.Providers.Count())
            {
                userCredential = await client.SignInAnonymouslyAsync();
                WriteLine($"You're anonymously signed with uid: {userCredential.User.Uid}. Link with email / redirect? [e/r/n]");
                var r = ReadLine().ToLower();
                if (r == "e")
                {
                    // link with email
                    Write("Enter email: ");
                    var email = ReadLine();
                    Write("Enter password: ");
                    var password = ReadPassword();

                    var credential = EmailProvider.GetCredential(email, password);
                    userCredential = await userCredential.User.LinkWithCredentialAsync(credential);
                } 
                else if (r == "r")
                {
                    // link with redirect
                    WriteLine("How do you want to link?");
                    config.Providers.Where(p => p.ProviderType != FirebaseProviderType.EmailAndPassword).Select((provider, i) => (provider, i)).ToList().ForEach(p => WriteLine($"[{p.i}]: {p.provider.ProviderType}"));

                    i = int.Parse(ReadLine());
                    userCredential = await userCredential.User.LinkWithRedirectAsync(config.Providers[i].ProviderType, uri =>
                    {
                        WriteLine($"Go to \n{uri}\n and paste here the redirect uri after you finish signing in");
                        return Task.FromResult(ReadLine());
                    });
                }

                if (r == "e" || r == "r")
                {
                    // if linked, offer unlink
                    WriteLine("Unlink? [y/n]");
                    if (ReadLine().ToLower() == "y")
                    {
                        userCredential.User = await userCredential.User.UnlinkAsync(FirebaseProviderType.EmailAndPassword);
                    }
                }
            }
            else
            {
                var provider = config.Providers[i].ProviderType;
                userCredential = provider == FirebaseProviderType.EmailAndPassword
                    ? await SignInWithEmail(client)
                    : await client.SignInWithRedirectAsync(provider, uri =>
                    {
                        WriteLine($"Go to \n{uri}\n and paste here the redirect uri after you finish signing in");
                        return Task.FromResult(ReadLine());
                    });

                WriteLine($"You're signed in as {userCredential.User.Uid} | {userCredential.User.Info.DisplayName} | {userCredential.User.Info.Email}");
            }

            WriteLine($"New password (empty to skip):");
            var pwd = ReadPassword();

            if (!string.IsNullOrWhiteSpace(pwd))
            {
                await userCredential.User.ChangePasswordAsync(pwd);
            }

            WriteLine($"Trying to force refresh the idToken {userCredential.User.Credential.IdToken}");
            var token = await userCredential.User.GetIdTokenAsync(true);
            WriteLine($"Success, new token: {token}");

            WriteLine("Delete this account? [y/n]");
            if (ReadLine().ToLower() == "y")
            {
                await userCredential.User.DeleteAsync();
            }            
        }

        private static async Task<UserCredential> SignInWithEmail(FirebaseAuthClient client)
        {
            Write("Enter email: ");
            var email = ReadLine();

            try
            {
                var result = await client.FetchSignInMethodsForEmailAsync(email);

                if (result.UserExists && result.AllProviders.Contains(FirebaseProviderType.EmailAndPassword))
                {
                    Write("User exists, enter password: ");
                    var password = ReadPassword();
                    var emailUser = await client.SignInWithEmailAndPasswordAsync(email, password);

                    return emailUser;
                }
                else
                {
                    Write("User not found, let's create him/her. Enter password: ");
                    var password = ReadPassword();
                    Write("Enter display name (optional): ");
                    var displayName = ReadLine();
                    return await client.CreateUserWithEmailAndPasswordAsync(email, password, displayName);                    
                }
            }
            catch (FirebaseAuthException ex)
            {
                WriteLine($"Exception thrown: {ex.Reason}");
                throw;
            }
        }

        private static string ReadPassword()
        {
            var pass = "";
            do
            {
                ConsoleKeyInfo key = ReadKey(true);
                // Backspace Should Not Work
                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    pass += key.KeyChar;
                    Write("*");
                }
                else
                {
                    if (key.Key == ConsoleKey.Backspace && pass.Length > 0)
                    {
                        pass = pass.Substring(0, (pass.Length - 1));
                        Write("\b \b");
                    }
                    else if (key.Key == ConsoleKey.Enter)
                    {
                        break;
                    }
                }
            } while (true);

            WriteLine();

            return pass;
        }
    }
}
