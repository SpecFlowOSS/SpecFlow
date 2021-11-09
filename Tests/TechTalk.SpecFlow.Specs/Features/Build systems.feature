@dotnetcore
Feature: Build systems

@globalusingdirective #MSBuild for VS2019 and Mono throws error CS8652: The feature 'global using directive' is currently in Preview and unsupported.
Scenario: Use MSBuild for compiling
	Given there is a scenario in a feature file
	And all steps are bound and pass
	
	When I compile the solution using 'MSBuild'

	Then no compilation errors are reported

Scenario: Use dotnet build for compiling
	Given there is a scenario in a feature file
	And all steps are bound and pass
	
	When I compile the solution using 'dotnet build'

	Then no compilation errors are reported


Scenario: Use dotnet msbuild for compiling
	Given there is a scenario in a feature file
	And all steps are bound and pass
	
	When I compile the solution using 'dotnet msbuild'

	Then no compilation errors are reported



