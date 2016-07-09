dotnet restore
dotnet build Tools
if ERRORLEVEL 1 goto :eof
dotnet build Tests/TechTalk.SpecFlow.GeneratorTests
if ERRORLEVEL 1 goto :eof
dotnet build Tests/TechTalk.SpecFlow.RuntimeTests
if ERRORLEVEL 1 goto :eof
dotnet build Tests/TechTalk.SpecFlow.Specs
if ERRORLEVEL 1 goto :eof
dotnet build Tests/TechTalk.SpecFlow.IntegrationTests
if ERRORLEVEL 1 goto :eof