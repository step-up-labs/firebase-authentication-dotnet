dotnet build --configuration release .\samples\Console\Auth.Console.Sample.csproj
dotnet build --configuration release .\samples\WPF\Auth.WPF.Sample.csproj
msbuild /restore /p:Configuration=Debug .\samples\UWP\Auth.UWP.Sample.csproj 