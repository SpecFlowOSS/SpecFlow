echo %1

set targetDir=%1

cd ..\..\
mkdir "%targetDir%\bin\Debug\net45\SpecFlow\Tools"
mkdir "%targetDir%\bin\Debug\net45\SpecFlow\lib"

mkdir "%targetDir%\bin\Debug\net45\NUnit3\""
copy .\Tools\MsBuild\TechTalk.SpecFlow.targets "%targetDir%\bin\Debug\net45\SpecFlow\Tools"
copy .\Tools\MsBuild\TechTalk.SpecFlow.tasks "%targetDir%\bin\Debug\net45\SpecFlow\Tools"
copy .\Tools\bin\Debug\net45\specflow.* "%targetDir%\bin\Debug\net45\SpecFlow\Tools"
copy .\Runtime\bin\Debug\net45\*.* "%targetDir%\bin\Debug\net45\SpecFlow\Tools"
copy .\Utils\bin\Debug\net45\*.* "%targetDir%\bin\Debug\net45\SpecFlow\Tools"
copy .\Reporting\bin\Debug\net45\*.* "%targetDir%\bin\Debug\net45\SpecFlow\Tools"
copy .\Parser\bin\Debug\net45\*.* "%targetDir%\bin\Debug\net45\SpecFlow\Tools"
copy .\Generator\bin\Debug\net45\*.* "%targetDir%\bin\Debug\net45\SpecFlow\Tools"
copy "%targetDir%\bin\Debug\net45\Gherkin.dll" "%targetDir%\bin\Debug\net45\SpecFlow\Tools"
copy "%targetDir%\bin\Debug\net45\Gherkin.dll" "%targetDir%\bin\Debug\net45\SpecFlow\Tools"

copy .\Tools\MsBuild\TechTalk.SpecFlow.targets "%targetDir%\bin\Debug\net45\SpecFlow\lib"
copy .\Tools\MsBuild\TechTalk.SpecFlow.tasks "%targetDir%\bin\Debug\net45\SpecFlow\lib"
copy .\Tools\bin\Debug\net45\specflow.* "%targetDir%\bin\Debug\net45\SpecFlow\lib"
copy .\Runtime\bin\Debug\net45\*.* "%targetDir%\bin\Debug\net45\SpecFlow\lib"
copy .\Utils\bin\Debug\net45\*.* "%targetDir%\bin\Debug\net45\SpecFlow\lib"
copy .\Reporting\bin\Debug\net45\*.* "%targetDir%\bin\Debug\net45\SpecFlow\lib"
copy .\Parser\bin\Debug\net45\*.* "%targetDir%\bin\Debug\net45\SpecFlow\lib"
copy .\Generator\bin\Debug\net45\*.* "%targetDir%\bin\Debug\net45\SpecFlow\lib"
copy "%targetDir%\bin\Debug\net45\Gherkin.dll" "%targetDir%\bin\Debug\net45\SpecFlow\lib"
copy "%targetDir%\bin\Debug\net45\Gherkin.dll" "%targetDir%\bin\Debug\net45\SpecFlow\lib"

xcopy "%USERPROFILE%\.nuget\packages\NUnit\3.2.1\*" "%targetDir%\bin\Debug\net45\NUnit3\" /s /y
xcopy ".\NuGet\custom\NUnit3-Runner\*" "%targetDir%\bin\Debug\net45\NUnit3-Runner\" /s /y
xcopy ".\lib\xunit.2.0.0\*.*" "%targetDir%\bin\Debug\net45\xUnit2\" /s /y
xcopy ".\lib\Microsoft F#\*.*" "%targetDir%\bin\Debug\net45\FSharp\" /s /y

xcopy "%USERPROFILE%\.nuget\packages\NUnit\2.6.0.12054\*" "%targetDir%\bin\Debug\net45\NUnit\" /s /y



goto :eof
xcopy "$(SolutionDir)Installer\SpecFlowBinPackage\bin\package\*.*" "%project:Directory%\bin\Debug\net45\SpecFlow\" /s /y
xcopy "$(SolutionDir)packages\NUnit.2.6.4\*.*" "$(TargetDir)NUnit\" /s /y
xcopy "$(SolutionDir)packages\NUnit.Runners.2.6.4\*.*" "$(TargetDir)NUnit.Runners\" /s /y
xcopy "$(SolutionDir)packages\xunit.1.9.2\lib\net20\*.*" "$(TargetDir)xunit\lib\" /s /y
xcopy "$(SolutionDir)packages\xunit.extensions.1.9.2\lib\net20\*.*" "$(TargetDir)xunit.extensions\lib\" /s /y
xcopy "$(SolutionDir)packages\xunit.runner.console.2.0.0\*.*" "$(TargetDir)xunit.runner.console\" /s /y
xcopy "$(SolutionDir)lib\xunit.2.0.0\*.*" "$(TargetDir)xUnit2\" /s /y
xcopy "$(SolutionDir)lib\Microsoft F#\*.*" "$(TargetDir)FSharp\" /s /y
xcopy "$(SolutionDir)packages\NUnit.2.6.0.12054\*.*" "$(TargetDir)NUnit\" /s /y
xcopy "$(SolutionDir)packages\NUnit.3.2.1\*.*" "$(TargetDir)NUnit3\" /s /y
xcopy "$(SolutionDir)packages\NUnit.Extension.NUnitV2ResultWriter.3.2.1\*.*" "$(TargetDir)NUnit.Extension.3.2.1\" /s /y
xcopy "$(SolutionDir)packages\NUnit.ConsoleRunner.3.2.1\*.*" "$(TargetDir)NUnit3-Runner\" /s /y
xcopy "$(SolutionDir)lib\mbunit.3.3.442.0\*.*" "$(TargetDir)mbUnit3\" /s /y