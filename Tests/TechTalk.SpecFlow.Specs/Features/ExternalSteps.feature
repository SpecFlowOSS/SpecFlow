Feature: External Step Definitions
	In order to modularize my solution
	As a bdd enthusiast
	I want to use step definitions from other assemblies
	
Scenario: Steps can defined in an external .NET (e.g. c# or VB.NET) project
	Given there is an external class library project 'ExternalSteps'
	And the following step definition in the external library
        """
		[When(@"I do something")]
		public void WhenIDoSomething()
		{
		}
        """
	And there is a SpecFlow project with a reference to the external library
	And a scenario 'Simple Scenario' as
         """
         When I do something
         """
	And the specflow configuration is
        """
		<specFlow>
			<stepAssemblies>
				<stepAssembly assembly="ExternalSteps" />
			</stepAssemblies>
		</specFlow>
        """
	When I execute the tests
	Then all tests should pass

@fsharp
Scenario: Steps can defined in an external F# project
	Given there is an external F# class library project 'ExternalSteps_FSharp'
	And the following binding class
        """
		[<TechTalk.SpecFlow.Binding>]
		module Bindings

		let [<TechTalk.SpecFlow.When(@"I do something")>] WhenIDoSomething() = ()
        """
	And there is a SpecFlow project with a reference to the external library
	And a scenario 'Simple Scenario' as
         """
         When I do something
         """
	And the specflow configuration is
        """
		<specFlow>
			<stepAssemblies>
				<stepAssembly assembly="ExternalSteps_FSharp" />
			</stepAssemblies>
		</specFlow>
        """
	When I execute the tests
	Then all tests should pass
