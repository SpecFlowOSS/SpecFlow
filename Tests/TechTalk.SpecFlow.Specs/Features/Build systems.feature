@dotnetcore
Feature: Build systems



Scenario: Use MSBuild for compiling
	Given there is a scenario in a feature file
	And all steps are bound and pass
	
	Given MSBuild is used for compiling

	Then no compilation errors are reported

Scenario: Use dotnet build for compiling
	Given there is a scenario in a feature file
	And all steps are bound and pass
	
	Given dotnet build is used for compiling

	Then no compilation errors are reported


Scenario: Use dotnet msbuild for compiling
	Given there is a scenario in a feature file
	And all steps are bound and pass
	
	Given dotnet msbuild is used for compiling

	Then no compilation errors are reported



