Feature: External Step Definitions
	In order to modularize my solution
	As a bdd enthusiast
	I want to use step definitions from other assemblies
	
Scenario: Steps defined in an external .NET (e.g. c# or VB.NET)
	Given I have external step definitions in a separate assembly referenced by this project
	When I call those steps
	Then the scenario should pass
