param([switch] $preview)

$output = Join-Path $pwd "artifacts"

if ($preview) {
	$suffix = "preview-$(Get-Date -format yyyyMMddHHmmss)"
	write "Creating preview packages with version suffix $suffix"
	dotnet pack --configuration release --output $output --version-suffix=$suffix .\src\Auth\Auth.csproj
	dotnet pack --configuration release --output $output --version-suffix=$suffix .\src\Auth.UI.WPF\Auth.UI.WPF.csproj
	dotnet pack --configuration release --output $output --version-suffix=$suffix .\src\Auth.UI.WinUI3\Auth.UI.WinUI3.csproj
	msbuild /t:restore,pack /p:Configuration=Release /p:PackageOutputPath=$output /p:VersionSuffix=$suffix .\src\Auth.UI.UWP\Auth.UI.UWP.csproj 
	dotnet pack --configuration release --output $output --version-suffix=$suffix .\src\Auth.UI.MAUI\Auth.UI.MAUI.csproj
} else {
	$version = $(git describe --tags --abbrev=0).substring(1)
	write "Creating packages with tag version $version"
	dotnet pack --configuration release --output $output -p:version=$version .\src\Auth\Auth.csproj
	dotnet pack --configuration release --output $output -p:version=$version .\src\Auth.UI.WPF\Auth.UI.WPF.csproj
	dotnet pack --configuration release --output $output -p:version=$version .\src\Auth.UI.WinUI3\Auth.UI.WinUI3.csproj
	msbuild /t:restore,pack /p:Configuration=Release /p:PackageOutputPath=$output /p:Version=$version .\src\Auth.UI.UWP\Auth.UI.UWP.csproj 
	dotnet pack --configuration release --output $output -p:version=$version .\src\Auth.UI.MAUI\Auth.UI.MAUI.csproj
}