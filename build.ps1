param (
 [string]$Configuration = "Debug",
 [string]$appInsightsInstrumentationKey = ""
)

$msbuildPath = "msbuild"
$additionalOptions = ""

Write-Host ($appInsightsInstrumentationKey -eq "")

if ($IsLinux) {
  $additionalOptions = "-p:EnableSourceControlManagerQueries=false -p:EnableSourceLink=false -p:DeterministicSourcePaths=false"
}

if ($appInsightsInstrumentationKey) {
  $additionalOptions = "$($additionalOptions) `"-property:AppInsightsInstrumentationKey=$($appInsightsInstrumentationKey)`""
}

& dotnet build ./TechTalk.SpecFlow.sln -property:Configuration=$Configuration -bl:msbuild.$Configuration.binlog /nodeReuse:false -v n --no-incremental $additionalOptions
