set targetDir=%1
set solutionDir=%2

set targetDir=%targetDir:"=%
set solutionDir=%solutionDir:"=%

echo %targetDir%
echo %solutionDir%


xcopy "%solutionDir%\Installer\SpecFlowBinPackage\bin\package\*.*" "%targetDir%\SpecFlow\" /s /y
xcopy "%solutionDir%\packages\NUnit.2.6.4\*.*" "%targetDir%\NUnit\" /s /y
xcopy "%solutionDir%\packages\NUnit.Runners.2.6.4\*.*" "%targetDir%\NUnit.Runners\" /s /y
xcopy "%solutionDir%\packages\xunit.1.9.2\lib\net20\*.*" "%targetDir%\xunit\lib\" /s /y
xcopy "%solutionDir%\packages\xunit.extensions.1.9.2\lib\net20\*.*" "%targetDir%\xunit.extensions\lib\" /s /y
xcopy "%solutionDir%\packages\xunit.runner.console.2.0.0\*.*" "%targetDir%\xunit.runner.console\" /s /y
xcopy "%solutionDir%\lib\xunit.2.0.0\*.*" "%targetDir%\xUnit2\" /s /y
xcopy "%solutionDir%\lib\Microsoft F#\*.*" "%targetDir%\FSharp\" /s /y
xcopy "%solutionDir%\packages\NUnit.2.6.0.12054\*.*" "%targetDir%\NUnit\" /s /y
xcopy "%solutionDir%\packages\NUnit.3.2.1\*.*" "%targetDir%\NUnit3\" /s /y
xcopy "%solutionDir%\packages\NUnit.Extension.NUnitV2ResultWriter.3.2.1\*.*" "%targetDir%\NUnit.Extension.3.2.1\" /s /y
xcopy "%solutionDir%\packages\NUnit.ConsoleRunner.3.2.1\*.*" "%targetDir%\NUnit3-Runner\" /s /y
xcopy "%solutionDir%\lib\mbunit.3.3.442.0\*.*" "%targetDir%\mbUnit3\" /s /y
