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

	Scenario: Multi argument step transformation can be converted
	Given the following step argument transformation
        """
        [StepArgumentTransformation]
        public XmlDocument TransformToTimeRange(DateTime start, DateTime end)
        {
             XmlDocument result = new XmlDocument();
			 return result;
        }
        """
	And the following step definition
         """
		[Given("I have a time range from (.*) to (.*)")]
        public void GivenIHaveATimeRange(XmlDocument timeRange)
        {
        } 
         """
	And a scenario 'Simple Scenario' as
         """
         Given I have a time range from 10-Dec-2015 to 11-Dec-2015
         """
	When I execute the tests
	Then the binding method 'GivenIHaveATimeRange' is executed

Scenario: Multi argument step transformation which are part of a multiple argument step can be converted
	Given the following step argument transformation
        """
        [StepArgumentTransformation]
        public XmlDocument TransformToXmlDocument(DateTime start, DateTime end)
        {
             XmlDocument result = new XmlDocument();
			 return result;
        }
        """
	And the following step argument transformation
        """
        [StepArgumentTransformation]
        public List<object> TransformToCollectionOfObjects(string name, string age)
        {
             List<object> result = new  List<object>();
			 return result;
        }
        """
	And the following step definition
         """
		[Given("I have a time range from (.*) to (.*) which contains the child '(.*)' aged (.*) birthday.")]
        public void GivenIHaveATimeRangeAndAChild(XmlDocument timeRange, List<object> child)
        {
        } 
         """
	And a scenario 'Simple Scenario' as
         """
         Given I have a time range from 10-Dec-2011 to 11-Dec-2011 which contains the child 'Tom' aged 5 birthday.
         """
	When I execute the tests
	Then the binding method 'GivenIHaveATimeRangeAndAChild' is executed

Scenario: Multi argument step transformation which are part of a multiple argument step which includes tables can be converted
	Given the following step argument transformation
        """
        [StepArgumentTransformation]
        public XmlDocument TransformToXmlDocument(DateTime start, DateTime end)
        {
             XmlDocument result = new XmlDocument();
			 return result;
        }
        """
	And the following step argument transformation
        """
        [StepArgumentTransformation]
        public List<object> TransformToCollectionOfObjects(string name, string age)
        {
             List<object> result = new  List<object>();
			 return result;
        }
        """
	And the following step argument transformation
        """
        [StepArgumentTransformation]
        public List<int> TransformToCollectionOfObjects(Table table)
        {
             List<int> result = new  List<int>();
			 return result;
        }
        """
	And the following step definition
         """
		[Given("I have a time range from (.*) to (.*) which contains the child '(.*)' aged (.*) birthday.")]
        public void GivenIHaveATimeRangeAndAChild(XmlDocument timeRange, List<object> child, List<int> table)
        {
        } 
         """
	And a scenario 'Simple Scenario' as
         """
         Given I have a time range from 10-Dec-2011 to 11-Dec-2011 which contains the child 'Tom' aged 5 birthday.
		    | Heading1 | Heading2 |
		    | val      | val2     |
		    | val4     | val3     |
         """
	When I execute the tests
	Then the binding method 'GivenIHaveATimeRangeAndAChild' is executed

Scenario: Multi argument step transformation which contain regexes which consume multiple arguments can be converted	
	Given the following step argument transformation
        """
        [StepArgumentTransformation(@"(\d*) hour(?:s)?")]
        public TimeSpan TransformToXmlDocument(int hours)
        {
             return TimeSpan.FromHours(hours);
        }
        """
	And the following step argument transformation
        """
        [StepArgumentTransformation]
        public List<object> TransformToCollectionOfObjects(string name, string age)
        {
             List<object> result = new  List<object>();
			 return result;
        }
        """
	And the following step argument transformation
        """
        [StepArgumentTransformation]
        public List<int> TransformToCollectionOfObjects(Table table)
        {
             List<int> result = new  List<int>();
			 return result;
        }
        """
	And the following step definition
         """
		[Given("A time of (.*) which contains the child '(.*)' aged (.*) birthday.")]
        public void GivenIHaveATimeRangeAndAChild(TimeSpan timeRange, List<object> child, List<int> table)
        {
        } 
         """
	And a scenario 'Simple Scenario' as
         """
         Given A time of 8 hours which contains the child 'Tom' aged 5 birthday.
		    | Heading1 | Heading2 |
		    | val      | val2     |
		    | val4     | val3     |
         """
	When I execute the tests
	Then the binding method 'GivenIHaveATimeRangeAndAChild' is executed



