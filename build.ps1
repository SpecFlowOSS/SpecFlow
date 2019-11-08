param (
 [string]$Configuration = "Debug",
 [string]$appInsightsInstrumentationKey = ""
)

$msbuildPath = "msbuild"

Write-Host ($appInsightsInstrumentationKey -eq "")

if ($IsWindows){
  $vswherePath = [System.Environment]::ExpandEnvironmentVariables("%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe")
  $vswhereParameters = @("-latest", "-products", "*", "-requires", "Microsoft.Component.MSBuild",  "-property", "installationPath", "-prerelease")
  
  $vsPath = & $vswherePath $vswhereParameters
  
  Write-Host $path
  
  if ($vsPath) {
    $msbuildPath = join-path $vsPath 'MSBuild\Current\Bin\MSBuild.exe'
  }
  
  Write-Host $msbuildPath
}

if ($appInsightsInstrumentationKey) {
  $extraArgs = "-property:AppInsightsInstrumentationKey=$appInsightsInstrumentationKey"
}

& $msbuildPath /Restore ./TechTalk.SpecFlow.sln /property:Configuration=$Configuration /binaryLogger:msbuild.$Configuration.binlog /nodeReuse:false $extraArgs