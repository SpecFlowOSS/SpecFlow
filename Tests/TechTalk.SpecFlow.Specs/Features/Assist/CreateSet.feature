Feature: CreateSet

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

Scenario: Should be able to convert a table to a list of entities
	Given a scenario 'Simple Scenario' as
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


Scenario: Should be able to convert a table to a list of entities with default values
	Given a scenario 'Simple Scenario' as
		"""
		Given the following accounts
			| Name         | Balance |
			| John Galt    | 1234.56 |
			| Someone Else | 45.6    |
		"""
	And the following step definition
		"""
		[Given(@"the following accounts")]
		public void GivenTheFollowingAccounts(Table accountsTable)
		{
			var accounts = accountsTable.CreateSet(() => new Account { Birthdate = new DateTime(2000,1,1), Name = "John Doe" });
			foreach (var account in accounts)
				Console.WriteLine(account.Birthdate);
		}
		"""
	And the 'TechTalk.SpecFlow.Assist' namespace is added to the namespace usings
	When I execute the tests
	Then the execution summary should contain
         | Succeeded |
         | 1         |
	And the execution log should contain text '1/1/2000'

