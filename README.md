# FirebaseAuthentication.net
![](https://github.com/step-up-labs/firebase-authentication-dotnet/workflows/build/badge.svg)
[![latest version](https://img.shields.io/nuget/v/FirebaseAuthentication.net)](https://www.nuget.org/packages/FirebaseAuthentication.net)
[![feedz.io](https://img.shields.io/badge/endpoint.svg?url=https%3A%2F%2Ff.feedz.io%2Fstep-up-labs%2Ffirebase%2Fshield%2FFirebaseAuthentication.net%2Flatest)](https://f.feedz.io/step-up-labs/firebase/packages/FirebaseAuthentication.net/latest/download)

FirebaseAuthentication.net is an unofficial C# implementation of [Firebase Authentication](https://firebase.google.com/docs/auth)
and [FirebaseUI](https://firebase.google.com/docs/auth). 

The solutions consists of 4 libraries, a base one and 3 platform specific ones:
* FirebaseAuthentication**.net** targets .NET Standard 2.0
* FirebaseAuthentication**.WPF** targets WPF on .NET Core 3.1
* FirebaseAuthentication**.UWP** targets UWP
* FirebaseAuthentication**.Xamarin** targets Xamarin.Forms

## Installation
```powershell
# base package
dotnet add package FirebaseAuthentication.net

# Platform specific FirebaseUI 
dotnet add package FirebaseAuthentication.WPF
dotnet add package FirebaseAuthentication.UWP
dotnet add package FirebaseAuthentication.Xamarin
```

Daily builds are also available on [feedz.io](https://feedz.io). Just add the following Package Source to your Visual Studio:

```
https://f.feedz.io/step-up-labs/firebase/nuget/index.json
```

TODO