param (
 [string]$Configuration = "Debug",
 [string]$appInsightsInstrumentationKey = ""
)

$msbuildPath = "msbuild"
$additionalOptions = ""

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
if ($IsLinux) {
  $additionalOptions = "-p:EnableSourceControlManagerQueries=false -p:EnableSourceLink=false -p:DeterministicSourcePaths=false"
}

if ($appInsightsInstrumentationKey) {
  $additionalOptions = "$($additionalOptions) -p:AppInsightsInstrumentationKey=$($appInsightsInstrumentationKey)"
}

& dotnet build ./TechTalk.SpecFlow.sln /m:1 -p:Configuration=$Configuration /bl:msbuild.$Configuration.binlog /nodeReuse:false -v n --force $additionalOptions
