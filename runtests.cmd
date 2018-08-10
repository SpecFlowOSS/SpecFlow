@where /q vstest.console.exe

@if ERRORLEVEL 1 (
    echo  "MSBuild is not in your PATH. Please use a developer command prompt!"
) else (
    vstest.console.exe Tests\TechTalk.SpecFlow.RuntimeTests\bin\Debug\net471\TechTalk.SpecFlow.RuntimeTests.dll
    @if ERRORLEVEL 1 (
        echo ".NET 471 Runtime Tests failed"
        goto :eof
    )

    vstest.console.exe Tests\TechTalk.SpecFlow.RuntimeTests\bin\Debug\netcoreapp2.0\TechTalk.SpecFlow.RuntimeTests.dll
    @if ERRORLEVEL 1 (
        echo ".NET CORE 2.0 Runtime Tests failed"
        goto :eof
    )

    vstest.console.exe Tests\TechTalk.SpecFlow.GeneratorTests\bin\Debug\net471\TechTalk.SpecFlow.GeneratorTests.dll
    @if ERRORLEVEL 1 (
        echo ".NET 471 GeneratorTests failed"
        goto :eof
    )

    vstest.console.exe Tests\TechTalk.SpecFlow.GeneratorTests\bin\Debug\netcoreapp2.0\TechTalk.SpecFlow.GeneratorTests.dll
    @if ERRORLEVEL 1 (
        echo ".NET CORE 2.0 GeneratorTests failed"
        goto :eof
    )

    vstest.console.exe Tests\TechTalk.SpecFlow.Specs\bin\Debug\net471\TechTalk.SpecFlow.Specs.dll
    @if ERRORLEVEL 1 (
        echo "Specs failed"
        goto :eof
    )
)
