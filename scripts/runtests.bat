@pushd %~dp0

@where /q msbuild

@IF ERRORLEVEL 1 (
	echo "MSBuild is not in your PATH. Please use a developer command prompt!"
	goto :end
)

cd ..

dotnet test Tests\TechTalk.SpecFlow.RuntimeTests --no-build

@IF ERRORLEVEL 1 (
	echo "error while running runtime unit tests"
	goto :end
)

dotnet test Tests\TechTalk.SpecFlow.GeneratorTests --no-build

@IF ERRORLEVEL 1 (
    echo "error while running generator unit tests"
	goto :end
)

:end

@popd
