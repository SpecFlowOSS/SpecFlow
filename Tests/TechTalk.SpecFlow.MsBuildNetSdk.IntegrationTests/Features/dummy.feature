Feature: Dummy feature file to test MSBuild netsdk codebehind file generation

Scenario: Dummy scenario
	Given I have a net.sdk style project
	When the project is build
	Then msbuild integration will generate code behind files
	And nest generated codebehind files under corresponding feature file
