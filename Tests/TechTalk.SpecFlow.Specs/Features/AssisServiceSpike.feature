Feature: AssisServiceSpike

Background: 
	Given there is a feature file in the project as
		"""
			Feature: Simple Feature
			Scenario: Simple Scenario
				Given the following books
					| Author      | Title                          | ReadingTime |
					| Gojko Adzic | Bridging the Communication Gap | 11 hours    |
		"""
	And the following binding class
        """
		public class Book
		{
			public string Author {get;set;}
			public string Title {get;set;}
			public TimeSpan ReadingTime {get;set;}
		}
        """
	Given the following step definitions
		 """
			[StepArgumentTransformation(@"(\d+) hours")]
			public TimeSpan ConvertFromHours(int hours)
			{
				return TimeSpan.FromHours(hours);
			}
		 """


Scenario: Should be able to execute a simple passing scenario
	Given the following step definitions
		 """
			//[Given("the following books")]
			public void GivenTheFollowingBooks(Table booksTable)
			{
				var book = booksTable.CreateSet<Book>().Single();
				if (book.ReadingTime != TimeSpan.FromHours(11))
					throw new Exception("wrong reading time");
			}
		 """
	And the following binding class
        """
		[Binding]
		public class StepsWithAssistService
		{
			private readonly AssistService assistService;

			public StepsWithAssistService(AssistService assistService)
			{
				 this.assistService = assistService;
			}

			//[Given("the following books")]
			public void GivenTheFollowingBooks_UsageA(Table booksTable)
			{
				var book = assistService.CreateSet2<Book>(booksTable).Single();
				if (book.ReadingTime != TimeSpan.FromHours(11))
					throw new Exception("wrong reading time");
			}
		}
        """
	And the following step definitions
		 """
			[Given("the following books")]
			public void GivenTheFollowingBooks_UsageB(Table booksTable)
			{
				var book = booksTable.CreateSet2<Book>().Single();
				if (book.ReadingTime != TimeSpan.FromHours(11))
					throw new Exception("wrong reading time");
			}
		 """
	When I execute the tests
	Then the execution summary should contain
		| Total | Succeeded |
		| 1     | 1         |
