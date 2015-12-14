Feature: Step Argument Transformations
	In order to reduce the amount of code and repetitive tasks in my steps
	As a programmer
	I want to define reusable transformations for my step arguments

Background: 
     Given the following binding class
        """

		public class Employee
		{
		     public Employee(string firstName, string lastName)
			 {
			    FirstName = firstName;
				LastName = lastName;
			 }
			 public string FirstName{get;set;}
			 public string LastName{get;set;}
		}

		public class HolidayBooking
		{
		    public HolidayBooking(DateTime startDate, int numberOfDays)
			{
			    StartDate = startDate;
				NumberOfDays = numberOfDays;
			}

			public DateTime StartDate{get;set;}
			public int NumberOfDays{get;set;}
		}

		public class Manager
		{
			public Manager(string name, DateTime dateOfBirth, string storeName)
			{
			    Name = name;
				DateOfBirth = dateOfBirth;
				ManagedStore = storeName;
				Employees = new List<Employee>();
			}

			public string Name{get;set;}
			public DateTime DateOfBirth{get;set;}
			public string ManagedStore{get;set;}
			public List<Employee> Employees {get;set;}
		}		
        """

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
        public Manager TransformToManager(string name, DateTime dateOfBirth, string storeName)
        {
             return new Manager(name, dateOfBirth, storeName);
        }
        """	
	And the following step definition
         """
		[Given("I have a manager called '(.*)' born on (.*) who manages the store '(.*)'")]
        public void GivenIHaveAManager(Manager manager)
        {
			Console.WriteLine(string.Format("Name:{0} DOB:{1} Store:{2}",manager.Name, manager.DateOfBirth.ToString("dd-MMM-yyyy"), manager.ManagedStore));
        } 
         """
	And a scenario 'Simple Scenario' as
         """
         Given I have a manager called 'Bob' born on 10-Dec-2015 who manages the store 'Springfield'
         """
	When I execute the tests
	Then the binding method 'GivenIHaveAManager' is executed
	And the execution log should contain text 'Name:Bob DOB:10-Dec-2015 Store:Springfield'

Scenario: Multi argument step transformation which are part of a multiple argument step can be converted
	Given the following step argument transformation
        """
        [StepArgumentTransformation]
        public Manager TransformToManager(string name, DateTime dateOfBirth, string storeName)
        {
             return new Manager(name, dateOfBirth, storeName);
        }
        """	
	And the following step argument transformation
        """
        [StepArgumentTransformation]
        public HolidayBooking TransformToHolidayBooking(DateTime startDate, int numberOfDays)
        {
             return new HolidayBooking(startDate, numberOfDays);
        }
        """
	And the following step definition
         """
		[Given("I have a manager called '(.*)' born on (.*) who manages the store '(.*)' who has booked holiday starting on (.*) for (.*) days")]
        public void GivenIHaveAManager(Manager manager, HolidayBooking holidayBooking)
        {
			Console.WriteLine(string.Format("Name:{0} DOB:{1} Store:{2}",manager.Name, manager.DateOfBirth.ToString("dd-MMM-yyyy"), manager.ManagedStore));
			Console.WriteLine(string.Format("Holiday:{0} Days:{1}",holidayBooking.StartDate.ToString("dd-MMM-yyyy"),holidayBooking.NumberOfDays));
        } 
         """
	And a scenario 'Simple Scenario' as
         """
         Given I have a manager called 'Bob' born on 10-Dec-2015 who manages the store 'Springfield' who has booked holiday starting on 11-Dec-2015 for 8 days
         """
	When I execute the tests
	Then the binding method 'GivenIHaveAManager' is executed
	And the execution log should contain text 'Name:Bob DOB:10-Dec-2015 Store:Springfield'
	And the execution log should contain text 'Holiday:11-Dec-2015 Days:8'

Scenario: Multi argument step transformation which are part of a multiple argument step which includes tables can be converted
	Given the following step argument transformation
        """
        [StepArgumentTransformation]
        public Manager TransformToManager(string name, DateTime dateOfBirth, string storeName, Table table)
        {
             Manager manager = new Manager(name, dateOfBirth, storeName);
			 foreach (var row in table.Rows)
			 {
			     manager.Employees.Add(new Employee(row[0],row[1]));
			 }

			 return manager;
        }
        """	
	And the following step definition
          """
		[Given("I have a manager called '(.*)' born on (.*) who manages the store '(.*)' who manages the following employees")]
        public void GivenIHaveAManager(Manager manager)
        {
			Console.WriteLine(string.Format("Name:{0} DOB:{1} Store:{2}",manager.Name, manager.DateOfBirth.ToString("dd-MMM-yyyy"), manager.ManagedStore));
			foreach(var employee in manager.Employees)
			{
			    Console.WriteLine(string.Format("Employee name:{0} {1}",employee.FirstName, employee.LastName));
			}
        } 
         """
	And a scenario 'Simple Scenario' as
         """
         Given I have a manager called 'Bob' born on 10-Dec-2015 who manages the store 'Springfield' who manages the following employees
		    | FirstName | LastName |
		    | Bob       | Bobbyson |
		    | Tom       | Thomas   |
         """
	When I execute the tests
	Then the binding method 'GivenIHaveAManager' is executed
	And the execution log should contain text 'Name:Bob DOB:10-Dec-2015 Store:Springfield'
	And the execution log should contain text 'Employee name:Bob Bobbyson'
	And the execution log should contain text 'Employee name:Tom Thomas'

Scenario: Multi argument step transformation which are part of a multiple argument step which includes tables and simple types can be converted
	Given the following step argument transformation
        """
        [StepArgumentTransformation]
        public Manager TransformToManager(string name, DateTime dateOfBirth, string storeName)
        {
             Manager manager = new Manager(name, dateOfBirth, storeName);
			 return manager;
        }
        """
	Given the following step argument transformation
        """
        [StepArgumentTransformation]
        public List<string> TransformToNotifications(Table table)
        {
             List<string> notifications = new List<String>();
			 foreach(var row in table.Rows)
			 {
			     notifications.Add(row[0]);
			 }

			 return notifications;
        }
        """	
	And the following step definition
          """
		[Given("I have a manager called '(.*)' born on (.*) who manages the store '(.*)' who was suspended for (.*) days and has the following warning notices")]
        public void GivenIHaveAManager(Manager manager, int suspensionPeriod, List<string> warningNotices)
        {
			Console.WriteLine(string.Format("Name:{0} DOB:{1} Store:{2}",manager.Name, manager.DateOfBirth.ToString("dd-MMM-yyyy"), manager.ManagedStore));
			Console.WriteLine(string.Format("suspension:{0}", suspensionPeriod));
			foreach(var warningNotice in warningNotices)
			{
			    Console.WriteLine(string.Format("warning notice:{0}", warningNotice));
			}
        } 
         """
	And a scenario 'Simple Scenario' as
         """
         Given I have a manager called 'Bob' born on 10-Dec-2015 who manages the store 'Springfield' who was suspended for 8 days and has the following warning notices
		    | WarningNotices |
		    | Stealing       | 
		    | Pilfering      | 
         """
	When I execute the tests
	Then the binding method 'GivenIHaveAManager' is executed
	And the execution log should contain text 'Name:Bob DOB:10-Dec-2015 Store:Springfield'
	And the execution log should contain text 'suspension:8'
	And the execution log should contain text 'warning notice:Pilfering'
	And the execution log should contain text 'warning notice:Stealing'

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



