#!/bin/bash

dotnet msbuild /t:Restore ./TechTalk.SpecFlow.sln

dotnet msbuild ./TechTalk.SpecFlow.sln /property:Configuration=Debug /bl