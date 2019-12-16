@dotnetcore
Feature: Build systems


@WindowsOnly
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



