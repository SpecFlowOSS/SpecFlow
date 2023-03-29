Feature: Multiple Specs Projects

Scenario Outline: Two projects with the same unit test provider
	Given I have Specs.Project.A and Specs.Project.B using the same unit test provider
	And Specs.Project.B references Specs.Project.A
	When I build the solution using '<Build Tool>'
	Then the build should succeed

	@globalusingdirective #MSBuild for VS2019 and Mono throws error CS8652: The feature 'global using directive' is currently in Preview and unsupported.
	Examples: 
	| Build Tool    |
	| MSBuild       |

	Examples:
	| Build Tool     |
	| dotnet build   |
	| dotnet msbuild |
