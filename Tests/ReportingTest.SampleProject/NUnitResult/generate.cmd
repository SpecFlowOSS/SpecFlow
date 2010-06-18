
pushd %~dp0\..

..\..\lib\nunit\nunit-console.exe bin\Debug\ReportingTest.SampleProject.dll /labels /xml=NUnitResult\TestResult.xml /out:NUnitResult\TestResult.txt /err:NUnitResult\TestResult.err

popd