param (
 [string]$Configuration = "Debug"
)

msbuild /Restore ./TechTalk.SpecFlow.sln

msbuild ./TechTalk.SpecFlow.sln /property:Configuration=$Configuration /binaryLogger:msbuild.$Configuration.binlog /nodeReuse:false