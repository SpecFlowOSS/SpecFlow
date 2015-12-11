Feature: Assist Using Service Injection

Background: 
	Given the following class
		"""
		public class Account
		{
			public string Name { get; set; }
			public DateTime Birthdate { get; set; }
			public decimal Balance { get; set; }
		}
		"""

Scenario: Should be able to use assist table services with injected service instance
	Given a scenario 'Simple Scenario' as
		"""
		Given the following accounts
			| Name         | Birthdate | Balance |
			| John Galt    | 2/2/1902  | 1234.56 |
			| Someone Else | 10/9/2009 | 45.6    |
		"""
	And the following binding class
		"""
		[Binding]
		public class AnotherStepsWithScenarioContext
		{
		    private readonly ITableServices tableServices;

			public AnotherStepsWithScenarioContext(ITableServices tableServices)
			{
				if (tableServices == null) throw new ArgumentNullException("tableServices");
				this.tableServices = tableServices;
			}

			[Given(@"the following accounts")]
			public void GivenTheFollowingAccounts(Table accountsTable)
			{
				var accounts = tableServices.CreateSet<Account>(accountsTable);
				foreach (var account in accounts)
					Console.WriteLine(account.Name);
			}
		}
		"""
	And the 'TechTalk.SpecFlow.Assist' namespace is added to the namespace usings
	When I execute the tests
	Then all tests should pass
	And the execution log should contain text 'John Galt'
	And the execution log should contain text 'Someone Else'
