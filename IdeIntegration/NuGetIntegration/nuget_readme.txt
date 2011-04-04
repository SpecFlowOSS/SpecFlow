Create a SpecFlow nuget package
1) Build release configuration
2) open command promt in this directory
3) Execute the following command
"..\..\lib\NuGet\NuGet.exe" pack "..\..\IdeIntegration\NuGetIntegration\specflow.nuspec" -o "..\..\IdeIntegration\NuGetIntegration\"
4) A new package is created like: SpecFlow.1.4.0.nupkg
5) Publish it on nuget.org