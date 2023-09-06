dotnet build --configuration release .\samples\Console\Auth.Console.Sample.csproj
dotnet build --configuration release .\samples\WPF\Auth.WPF.Sample.csproj
dotnet build --configuration release .\samples\WinUI3\Auth.WinUI3.Sample.csproj
msbuild /restore /p:Configuration=Debug .\samples\UWP\Auth.UWP.Sample.csproj 