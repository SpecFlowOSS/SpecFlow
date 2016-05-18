Feature: Step Argument Transformations
	In order to reduce the amount of code and repetitive tasks in my steps
	As a programmer
	I want to define reusable transformations for my step arguments

Scenario: Should be able to convert steps arguments to arbritrary class instances
	Given the following class
         """
         public class User
		 {
			public string Name { get; set; }
		 }
         """
	And the following step argument transformation
         """
         [StepArgumentTransformation]
		 public User ConvertUser(string name)
		 {
			return new User {Name = name};
		 }
         """
	And the following step definition
         """
			[When(@"(.*) books a flight")]
			public void WhenSomebodyBooksAFlight(User user)
			{}
         """
	And a scenario 'Simple Scenario' as
         """
         When Bob books a flight
         """
	When I execute the tests
	Then the binding method 'WhenSomebodyBooksAFlight' is executed

Scenario: Should be able to convert step arguments to simple .NET types
	Given the following step argument transformation
         """
         [StepArgumentTransformation(@"in (\d+) days")]
		 public DateTime ConvertInDays(int days)
		 {
			return DateTime.Today.AddDays(days);
		 }
         """
	And the following step definition
         """
			[Given(@"I have an appointment (.*)")]
			public void GivenIHaveAnAppointmentAt(DateTime time)
			{}
         """
	And a scenario 'Simple Scenario' as
         """
         Given I have an appointment in 2 days
         """
	When I execute the tests
	Then the binding method 'GivenIHaveAnAppointmentAt' is executed
	
Scenario: Multi-line text arguments can be converted
	Given the following step argument transformation
        """
        [StepArgumentTransformation]
        public XmlDocument TransformXml(string xml)
        {
            XmlDocument result = new XmlDocument();
            result.LoadXml(xml);
            return result;
        }
        """
	And the following step definition
         """
			[Given(@"the following XML file")]
			public void GivenTheFollowingXMLFile(XmlDocument xmlDocument)
			{}
         """
	And a scenario 'Simple Scenario' as
         """
         Given the following XML file
			'''
				<Root>
					<Child attr="value" />
				</Root>
			'''
         """
	When I execute the tests
	Then the binding method 'GivenTheFollowingXMLFile' is executed
