﻿dotnet build --configuration release .\src\Auth\Auth.csproj
dotnet build --configuration release .\src\Auth.UI\Auth.UI.csproj
dotnet build --configuration release .\src\Auth.UI.WPF\Auth.UI.WPF.csproj
msbuild /t:restore .\src\Auth.UI.UWP\Auth.UI.UWP.csproj
msbuild /p:Configuration=Release .\src\Auth.UI.UWP\Auth.UI.UWP.csproj 