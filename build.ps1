param (
 [string]$Configuration = "Debug",
 [string]$appInsightsInstrumentationKey = ""
)

$additionalOptions = ""

if ($IsLinux) {
  $additionalOptions = "-p:EnableSourceControlManagerQueries=false -p:EnableSourceLink=false -p:DeterministicSourcePaths=false"
}

if ($appInsightsInstrumentationKey) {
  if ($additionalOptions){
    $additionalOptions = "$($additionalOptions) -property:AppInsightsInstrumentationKey=$($appInsightsInstrumentationKey)"
  }
  else {
    $additionalOptions = "-property:AppInsightsInstrumentationKey=$($appInsightsInstrumentationKey)"
  }
}

$NuGetFolder = "./GeneratedNuGetPackages/$($Configuration)"

New-Item -ItemType Directory -Path $NuGetFolder

Write-Host "dotnet build ./TechTalk.SpecFlow.sln -property:Configuration=$Configuration -bl:msbuild.$Configuration.binlog -nodeReuse:false -v n --no-incremental $additionalOptions"

& dotnet build ./TechTalk.SpecFlow.sln -property:Configuration=$Configuration -bl:msbuild.$Configuration.binlog -nodeReuse:false -v n $additionalOptions
