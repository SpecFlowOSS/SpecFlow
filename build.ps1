param (
 [string]$Configuration = "Debug"
)

$msbuildPath = "msbuild"

$_isWindows = $false
$_isMacOS = $false

if (($Env:HOME)) {
  $_isMacOS = $true
}
else {
  if (($Env:UserProfile))
  {
    $_isWindows = $true
  }
}

if ($_isWindows -eq $TRUE){
  Write-Host "Running on Windows"
}
else {
  if ($_isMacOS -eq $TRUE) {
    Write-Host "Running on Mac OS"
  }
  else {
    Write-Host "No idea which OS I am running"
  }  
}


if ($_isWindows){
  $vswherePath = [System.Environment]::ExpandEnvironmentVariables("%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe")
  $vswhereParameters = @("-latest", "-products", "*", "-requires", "Microsoft.Component.MSBuild",  "-property", "installationPath")
  
  $vsPath = & $vswherePath $vswhereParameters
  
  Write-Host $path
  
  if ($vsPath) {
    $msbuildPath = join-path $vsPath 'MSBuild\15.0\Bin\MSBuild.exe'
  }
  
  Write-Host $msbuildPath
}

& $msbuildPath /Restore ./TechTalk.SpecFlow.sln /property:Configuration=$Configuration /binaryLogger:msbuild.$Configuration.binlog /nodeReuse:false