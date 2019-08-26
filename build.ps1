param (
 [string]$Configuration = "Debug"
)

$msbuildPath = "msbuild"
$additionalOptions = ""

if ($IsWindows){
  $vswherePath = [System.Environment]::ExpandEnvironmentVariables("%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe")
  $vswhereParameters = @("-latest", "-products", "*", "-requires", "Microsoft.Component.MSBuild",  "-property", "installationPath")
  
  $vsPath = & $vswherePath $vswhereParameters
  
  Write-Host $path
  
  if ($vsPath) {
    $msbuildPath = join-path $vsPath 'MSBuild\Current\Bin\MSBuild.exe'
  }
  
  Write-Host $msbuildPath
}
if ($IsLinux) {
  $additionalOptions = "/p:EnableSourceControlManagerQueries=false /p:EnableSourceLink=false /p:DeterministicSourcePaths=false"
}

& $msbuildPath /Restore ./TechTalk.SpecFlow.sln /property:Configuration=$Configuration /binaryLogger:msbuild.$Configuration.binlog /nodeReuse:false $additionalOptions