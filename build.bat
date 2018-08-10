@where /q msbuild

@if ERRORLEVEL 1 (
    echo  "MSBuild is not in your PATH. Please use a developer command prompt!"
) else (
    msbuild /t:Restore .\TechTalk.SpecFlow.sln
    
    MSBuild.exe .\TechTalk.SpecFlow.sln /property:Configuration=Debug /bl
)
