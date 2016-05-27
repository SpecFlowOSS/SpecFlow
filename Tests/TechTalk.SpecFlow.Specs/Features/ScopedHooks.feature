Feature: ScopedHooks

Scenario: Scope on Binding class and method triggers hook only once
	Given there is a feature file in the project as
         """
			Feature: Simple Feature

			@mytag
			Scenario: Simple Scenario
			When I do something
         """
	And all steps are bound and pass
	#And a hook 'HookForBeforeScenarioHook' for 'BeforeScenario("mytag")' exists in a binding class scoped with Tag 'mytag'
	And the following binding class
		"""
		[Binding]
		[Scope(Tag = "mytag")]
		public class Hooks
		{
			[BeforeScenario("mytag")]
			public void BeforeScenarioHook()
			{
				Console.WriteLine("HookForBeforeScenarioHook");
				System.IO.File.AppendAllText(System.IO.Path.Combine(NUnit.Framework.TestContext.CurrentContext.TestDirectory, "hooks.log"), "-> hook: HookForBeforeScenarioHook");
			}
		}
		"""
	When I execute the tests
	Then the hook 'HookForBeforeScenarioHook' is executed once
	