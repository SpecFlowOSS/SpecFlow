#!/bin/bash
set -e
dotnet restore
dotnet build Tools
dotnet build Tests/GeneratorTests
dotnet build Tests/RuntimeTests
dotnet build Tests/TechTalk.SpecFlow.Specs
dotnet build Tests/TechTalk.SpecFlow.IntegrationTests
