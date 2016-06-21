#!/bin/bash
set -e

mono NuGet/custom/NUnit3-Runner/bin/nunit-console.exe Tests/GeneratorTests/bin/Debug/net45/TechTalk.SpecFlow.GeneratorTests.dll
mono NuGet/custom/NUnit3-Runner/bin/nunit-console.exe Tests/RuntimeTests/bin/Debug/net45/TechTalk.SpecFlow.RuntimeTests.dll

mono NuGet/custom/NUnit3-Runner/bin/nunit-console.exe Tests/TechTalk.SpecFlow.Specs/bin/Debug/net45/TechTalk.SpecFlow.Specs.dll

mono NuGet/custom/NUnit3-Runner/bin/nunit-console.exe Tests/TechTalk.SpecFlow.IntegrationTests/bin/Debug/net45/TechTalk.SpecFlow.IntegrationTests.dll

