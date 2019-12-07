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
                ApiKey = "AIzaSyDFi3vESrBJ12BJovdEtj8jq0OmyktugaQ",
                AuthDomain = "torrid-inferno-3642.firebaseapp.com",
                ExternalSignInDelegate = uri =>
                {
                    WriteLine($"Go to \n{uri}\n and paste here the redirect uri after you finish signing in");
                    return Task.FromResult(ReadLine());
                },
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

            var client = new FirebaseAuthClient(config);

            WriteLine("How do you want to sign in?");
            config.Providers.Select((provider, i) => (provider, i)).ToList().ForEach(p => WriteLine($"[{p.i}]: {p.provider.AuthType}"));
            WriteLine($"[{config.Providers.Count()}]: Anonymously");

            var i = int.Parse(ReadLine());

            if (i == config.Providers.Count())
            {
                var user = await client.SignInAnonymouslyAsync();
                WriteLine($"You're anonymously signed with uid: {user.LocalId}");
            }
            else 
            {
                var provider = config.Providers[i].AuthType;
                var user = provider == FirebaseProviderType.EmailAndPassword
                    ? await SignInWithEmail(client)
                    : await client.SignInExternallyAsync(provider);

                WriteLine($"You're sign in as {user.DisplayName} | {user.Email} | {user.LocalId}");
            }
        }

        private static async Task<User> SignInWithEmail(FirebaseAuthClient client)
        {
            Write("Enter email: ");
            var email = ReadLine();

            try
            {
                var result = await client.CheckUserEmailExistsAsync(email);

                if (result.UserExists)
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
