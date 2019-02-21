
$errorCode = dotnet test --no-build ./Tests/TechTalk.SpecFlow.RuntimeTests/TechTalk.SpecFlow.RuntimeTests.csproj -f net471

if ($errorCode -gt 0) {
    Write-Host "Runtime Tests for .NET 4.7.1 failed"
    exit
}

$errorCode = dotnet test --no-build ./Tests/TechTalk.SpecFlow.RuntimeTests/TechTalk.SpecFlow.RuntimeTests.csproj -f netcoreapp2.0

if ($errorCode -gt 0) {
    Write-Host "Runtime Tests for .NET Core 2.0 failed"
    exit
}