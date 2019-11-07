Feature: Multiple Specs Projects

Scenario Outline: Two projects with the same unit test provider
	Given I have Specs.Project.A and Specs.Project.B using the same unit test provider
	And Specs.Project.B references Specs.Project.A
	When I build the solution using <BuildTool> with treat warnings as errors enabled
	Then the build should succeed

	Examples: 
	| BuildTool     |
	| MSBuild       |
	| DotnetBuild   |
	| DotnetMSBuild |
