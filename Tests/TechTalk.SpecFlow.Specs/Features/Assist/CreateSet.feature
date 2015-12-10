Feature: CreateSet

Scenario: Should be able to convert a table to a list of entities
	Given the following class
		"""
		public class Account
		{
			public string Name { get; set; }
			public DateTime Birthdate { get; set; }
			public decimal Balance { get; set; }
		}
		"""
	And a scenario 'Simple Scenario' as
		"""
		Given the following accounts
			| Name         | Birthdate | Balance |
			| John Galt    | 2/2/1902  | 1234.56 |
			| Someone Else | 10/9/2009 | 45.6    |
		"""
	And the following step definition
		"""
		[Given(@"the following accounts")]
		public void GivenTheFollowingAccounts(Table accountsTable)
		{
			var accounts = accountsTable.CreateSet<Account>();
			foreach (var account in accounts)
				Console.WriteLine(account.Name);
		}
		"""
	And the 'TechTalk.SpecFlow.Assist' namespace is added to the namespace usings
	When I execute the tests
	Then the execution summary should contain
         | Succeeded |
         | 1         |
	And the execution log should contain text 'John Galt'
	And the execution log should contain text 'Someone Else'

