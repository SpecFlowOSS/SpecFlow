Feature: SingleFileGenerator Checks


Scenario: Build Error when using the SingleFileGenerator and SpecFlow.Tools.MSBuild.Generator

	Given there is a SpecFlow project
	And it is using SpecFlow.Tools.MSBuild.Generator
	And has a feature file with the SingleFileGenerator configured

	When the project is compiled

	Then is a compilation error