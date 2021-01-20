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
			{{                                                                                                                                                 
                global::Log.LogStep();  
            }}
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
			{{                                                                                                                                                    
                global::Log.LogStep(); 
            }}
         """
	And a scenario 'Simple Scenario' as
         """
         Given I have an appointment in 2 days
         """
	When I execute the tests
	Then the binding method 'GivenIHaveAnAppointmentAt' is executed

Scenario: Should be able to convert when multiple converters are returning the same target type: string and Table
	Given the following step argument transformation
         """
		 [StepArgumentTransformation()]
		 public System.Collections.Generic.List<DateTime> ConvertInDays(string days)
		 {
			return new System.Collections.Generic.List<DateTime> { DateTime.Today.AddDays(int.Parse(days)) };
		 }

		 [StepArgumentTransformation()]
		 public System.Collections.Generic.List<DateTime> ConvertInDays(Table table)
		 {
			System.Collections.Generic.List<DateTime> dates = new System.Collections.Generic.List<DateTime>();
            foreach (TableRow row in table.Rows)
            {
                dates.Add(DateTime.Today.AddDays(int.Parse(row["in days"])));
            }
            return dates;
		 }
         """
	And the following step definition
         """
			[Given(@"I have an appointment in (.*) days")]
			public void GivenIHaveAnAppointmentAtDays(System.Collections.Generic.List<DateTime> dates)
			{{                                                                                                                                                    
                global::Log.LogStep(); 
            }}

			[Given(@"I have the following appointments")]
			public void GivenIHaveAnAppointmentAtTable(System.Collections.Generic.List<DateTime> dates)
			{{                                                                                                                                                    
                global::Log.LogStep(); 
            }}
         """
	And a scenario 'Simple Scenario' as
         """
         Given I have an appointment in 2 days
		 Given I have the following appointments
		 | in days |
		 | 2       |
         """
	When I execute the tests
	Then the binding method 'GivenIHaveAnAppointmentAtDays' is executed
	Then the binding method 'GivenIHaveAnAppointmentAtTable' is executed
	

Scenario: Should be able to convert when multiple converters are returning the same target type: non string and Table
	Given the following step argument transformation
         """
		 [StepArgumentTransformation()]
		 public System.Collections.Generic.List<DateTime> ConvertInDays(Table table)
		 {
			System.Collections.Generic.List<DateTime> dates = new System.Collections.Generic.List<DateTime>();
            foreach (TableRow row in table.Rows)
            {
                dates.Add(DateTime.Today.AddDays(int.Parse(row["in days"])));
            }
            return dates;
		 }

		 [StepArgumentTransformation(@"(\d+)")]
		 public System.Collections.Generic.List<DateTime> ConvertInDays(int days)
		 {
			return new System.Collections.Generic.List<DateTime> { DateTime.Today.AddDays(days) };
		 }		 
         """
	And the following step definition
         """
			[Given(@"I have an appointment in (.*) days")]
			public void GivenIHaveAnAppointmentAtDays(System.Collections.Generic.List<DateTime> dates)
			{{                                                                                                                                                    
                global::Log.LogStep(); 
            }}

			[Given(@"I have the following appointments")]
			public void GivenIHaveAnAppointmentAtTable(System.Collections.Generic.List<DateTime> dates)
			{{                                                                                                                                                    
                global::Log.LogStep(); 
            }}
         """
	And a scenario 'Simple Scenario' as
         """
         Given I have an appointment in 2 days
		 Given I have the following appointments
		 | in days |
		 | 2       |
         """
	When I execute the tests
	Then the binding method 'GivenIHaveAnAppointmentAtDays' is executed
	Then the binding method 'GivenIHaveAnAppointmentAtTable' is executed	

Scenario: Multi-line text arguments can be converted
	Given the following step argument transformation
        """
        [StepArgumentTransformation]
        public XmlDocument TransformXml(string xml)
        {
            var result = new XmlDocument();
            result.LoadXml(xml);
            return result;
        }
        """
	And the following step definition
         """
			[Given(@"the following XML file")]
			public void GivenTheFollowingXMLFile(XmlDocument xmlDocument)
			{{                                                                                                                                                       
                global::Log.LogStep(); 
            }}
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
