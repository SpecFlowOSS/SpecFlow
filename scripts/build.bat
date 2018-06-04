@pushd %~dp0

@where /q msbuild

@IF ERRORLEVEL 1 (
	echo "MSBuild is not in your PATH. Please use a developer command prompt!"
	goto :end
)

cd ..

msbuild /bl TechTalk.SpecFlow.sln

:end

@popd
