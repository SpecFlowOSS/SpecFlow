rem NuGet\custom\NUnit3-Runner\bin\nunit-console.exe Tests\GeneratorTests\bin\Debug\net45\TechTalk.SpecFlow.GeneratorTests.dll
rem if ERRORLEVEL 1 goto :eof
rem NuGet\custom\NUnit3-Runner\bin\nunit-console.exe Tests\RuntimeTests\bin\Debug\net45\TechTalk.SpecFlow.RuntimeTests.dll
rem if ERRORLEVEL 1 goto :eof
NuGet\custom\NUnit3-Runner\bin\nunit-console.exe Tests\TechTalk.SpecFlow.Specs\bin\Debug\net45\TechTalk.SpecFlow.Specs.dll
rem if ERRORLEVEL 1 goto :eof
rem NuGet\custom\NUnit3-Runner\bin\nunit-console.exe Tests\TechTalk.SpecFlow.IntegrationTests\bin\Debug\net45\TechTalk.SpecFlow.IntegrationTests.dll
rem if ERRORLEVEL 1 goto :eof

rem echo "All tests passed!"