dotnet build --configuration release .\samples\Console\Auth.Console.Sample.csproj
dotnet build --configuration release .\samples\WPF\Auth.WPF.Sample.csproj
msbuild /t:restore .\samples\UWP\Auth.UWP.Sample.csproj 
msbuild /p:Configuration=Debug .\samples\UWP\Auth.UWP.Sample.csproj 