Feature: Tracing tests

Scenario: Preserves step keywords in trace
	Given there is a feature file in the project as
		"""
			#language: de-DE
			Funktionalität: German
			Szenario: Zwei Zahlen hinzufügen
				Angenommen ich Knopf 1 druecke
				Gegeben sei ich Knopf 2 druecke
		"""
	And all steps are bound and pass
	When I execute the tests
	Then the execution log should contain text 'Angenommen ich Knopf 1 druecke'
	And the execution log should contain text 'Gegeben sei ich Knopf 2 druecke'

Scenario: ReflectionTypeLoaderException in a step binding
	Given there is a feature file in the project as
		 """
			Feature: f
			Scenario: s
				When ...
		 """
	And the following step definition
		"""
		[When(@".*")]
		public void When___()
		{
			throw new System.Reflection.ReflectionTypeLoadException(
				new System.Type[3],
				new[]
				{
					new System.Exception("crash"),
					new System.Exception("boom"),
					new System.Exception("bang"),
				});
		}
		"""
	When I execute the tests
	Then the execution log should contain text '-> error: Type Loader exceptions:'
	And the execution log should contain text '-> error: LoaderException: System.Exception: crash'
	And the execution log should contain text '-> error: LoaderException: System.Exception: boom'
	And the execution log should contain text '-> error: LoaderException: System.Exception: bang'

Scenario: ReflectionTypeLoaderException in a static constructor
	Given the following class
		"""
			public class Class1
			{
				static Class1()
				{
					throw new System.Reflection.ReflectionTypeLoadException(
						new System.Type[3],
						new[]
						{
							new System.Exception("crash"),
							new System.Exception("boom"),
							new System.Exception("bang"),
						});
				}
			}
		"""
	And there is a feature file in the project as
		 """
			Feature: f
			Scenario: s
				When ...
		 """
	And the following step definition
		"""
		[When(@".*")]
		public void When___()
		{
			System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(Class1).TypeHandle);
		}
		"""
	When I execute the tests
	Then the execution log should contain text '-> error: Type Loader exceptions:'
	And the execution log should contain text '-> error: LoaderException: System.Exception: crash'
	And the execution log should contain text '-> error: LoaderException: System.Exception: boom'
	And the execution log should contain text '-> error: LoaderException: System.Exception: bang'
