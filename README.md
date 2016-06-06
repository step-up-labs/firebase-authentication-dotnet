# FirebaseDatabase.net
[![AppVeyor Build status](https://ci.appveyor.com/api/projects/status/rwmdgqcb7is2clqp?svg=true)](https://ci.appveyor.com/project/bezysoftware/firebase-authentication-dotnet)

Firebase authentication library. It can generate Firebase auth token based on given OAuth token (issued by Google, Facebook...). This Firebase token can then be used with REST queries against Firebase endpoints.

## Installation
```csharp
// Install release version
Install-Package FirebaseAuth.net
```

## Supported frameworks
* .NET 4.5+
* Windows 8.x
* UWP
* Windows Phone 8.1
* CoreCLR

## Usage

```csharp
var authProvider = new FirebaseAuthProvider(new FirebaseConfig(FirebaseApiKey));
var facebookAccessToken = "<login with facebook and get oauth access token>";

var auth = await authProvider.SignInWithOAuth(FirebaseAuthType.Facebook, facebookAccessToken);

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
