dotnet.exe test "C:\work\SpecFlow\Tests\GeneratorTests\project.json"
if ERRORLEVEL 1 goto :eof
dotnet.exe test "C:\work\SpecFlow\Tests\RuntimeTests\project.json"
if ERRORLEVEL 1 goto :eof
dotnet.exe test "C:\work\SpecFlow\Tests\TechTalk.SpecFlow.Specs\project.json"
if ERRORLEVEL 1 goto :eof
dotnet.exe test "C:\work\SpecFlow\Tests\TechTalk.SpecFlow.IntegrationTests\project.json"
if ERRORLEVEL 1 goto :eof

echo "All tests passed!"