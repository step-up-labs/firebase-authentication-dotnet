# FirebaseAuthentication.net
[![AppVeyor Build status](https://ci.appveyor.com/api/projects/status/rwmdgqcb7is2clqp?svg=true)](https://ci.appveyor.com/project/bezysoftware/firebase-authentication-dotnet)

Firebase authentication library. It can generate Firebase auth token based on given OAuth token (issued by Google, Facebook...). This Firebase token can then be used with REST queries against Firebase Database endpoints. See [FirebaseDatabase.net](https://github.com/step-up-labs/firebase-database-dotnet) for a C# library wrapping the Firebase Database REST queries.

## Installation
```csharp
// Install release version
Install-Package FirebaseAuthentication.net
```

## Supported frameworks
* .NET Standard 1.1 - see https://github.com/dotnet/standard/blob/master/docs/versions.md for compatibility matrix

## Supported scenarios
* Login with Google / Facebook / Github / Twitter OAuth tokens
* Anonymous login
* Login with email + password
* Create new user with email + password
* Send a password reset email
* Link two accounts together

## Usage

```csharp
var authProvider = new FirebaseAuthProvider(new FirebaseConfig(FirebaseApiKey));
var facebookAccessToken = "<login with facebook and get oauth access token>";

var auth = await authProvider.SignInWithOAuthAsync(FirebaseAuthType.Facebook, facebookAccessToken);

var firebase = new FirebaseClient("https://dinosaur-facts.firebaseio.com/");
var dinos = await firebase
  .Child("dinosaurs")
  .WithAuth(auth.FirebaseToken)
  .OnceAsync<Dinosaur>();
  
foreach (var dino in dinos)
{
  Console.WriteLine($"{dino.Key} is {dino.Object.Height}m high.");
}
```

## Facebook setup

Under [Facebook developers page for your app](https://developers.facebook.com/) make sure you have a similar setup:

![Logo](/art/FacebookSetup.png)


## Google setup

In the [developer console](https://console.developers.google.com/apis/credentials) make sure you have an OAuth client (set it either as iOS or Android app, that should work).
