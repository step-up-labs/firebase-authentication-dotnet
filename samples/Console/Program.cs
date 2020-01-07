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
                Console.WriteLine("You need to setup your API key and auth domain first in Program.cs");
                return;
            }

            var client = new FirebaseAuthClient(config);

            WriteLine("How do you want to sign in?");
            config.Providers.Select((provider, i) => (provider, i)).ToList().ForEach(p => WriteLine($"[{p.i}]: {p.provider.ProviderType}"));
            WriteLine($"[{config.Providers.Count()}]: Anonymously");

            var i = int.Parse(ReadLine());
            User user;

            if (i == config.Providers.Count())
            {
                user = await client.SignInAnonymouslyAsync();
                WriteLine($"You're anonymously signed with uid: {user.Uid}. Link with email? [y/n]");
                if (ReadLine().ToLower() == "y")
                {
                    Write("Enter email: ");
                    var email = ReadLine();
                    Write("Enter password: ");
                    var password = ReadPassword();

                    var credential = EmailProvider.GetCredential(email, password);
                    user = await user.LinkWithCredentialAsync(credential);
                }
            }
            else 
            {
                var provider = config.Providers[i].ProviderType;
                user = provider == FirebaseProviderType.EmailAndPassword
                    ? await SignInWithEmail(client)
                    : await client.SignInWithRedirectAsync(provider, uri =>
                    {
                        WriteLine($"Go to \n{uri}\n and paste here the redirect uri after you finish signing in");
                        return Task.FromResult(ReadLine());
                    });

                WriteLine($"You're signed in as {user.Uid} | {user.Info.DisplayName} | {user.Info.Email}");
            }

            WriteLine($"New password (empty to skip):");
            var pwd = ReadPassword();

            if (!string.IsNullOrWhiteSpace(pwd))
            {
                await user.ChangePasswordAsync(pwd);
            }

            WriteLine($"Trying to force refresh the idToken {user.Credential.IdToken}");
            var token = await user.GetIdTokenAsync(true);
            WriteLine($"Success, new token: {token}");

            WriteLine("Delete this account? [y/n]");
            if (ReadLine().ToLower() == "y")
            {
                await user.DeleteAsync();
            }            
        }

        private static async Task<User> SignInWithEmail(FirebaseAuthClient client)
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
