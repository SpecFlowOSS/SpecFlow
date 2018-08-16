param (
 [string]$Configuration = "Debug"
)

dotnet msbuild /Restore ./TechTalk.SpecFlow.sln

dotnet msbuild ./TechTalk.SpecFlow.sln /property:Configuration=$Configuration /binaryLogger:msbuild.$Configuration.binlog /nodeReuse:false