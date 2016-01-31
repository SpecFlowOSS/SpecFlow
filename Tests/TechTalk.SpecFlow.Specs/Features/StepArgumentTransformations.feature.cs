﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (http://www.specflow.org/).
//      SpecFlow Version:1.9.0.77
//      SpecFlow Generator Version:1.9.0.0
//      Runtime Version:4.0.30319.42000
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace TechTalk.SpecFlow.Specs.Features
{
    using TechTalk.SpecFlow;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "1.9.0.77")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [NUnit.Framework.TestFixtureAttribute()]
    [NUnit.Framework.DescriptionAttribute("Step Argument Transformations")]
    public partial class StepArgumentTransformationsFeature
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "StepArgumentTransformations.feature"
#line hidden
        
        [NUnit.Framework.TestFixtureSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Step Argument Transformations", "In order to reduce the amount of code and repetitive tasks in my steps\r\nAs a prog" +
                    "rammer\r\nI want to define reusable transformations for my step arguments", ProgrammingLanguage.CSharp, ((string[])(null)));
            testRunner.OnFeatureStart(featureInfo);
        }
        
        [NUnit.Framework.TestFixtureTearDownAttribute()]
        public virtual void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        [NUnit.Framework.SetUpAttribute()]
        public virtual void TestInitialize()
        {
        }
        
        [NUnit.Framework.TearDownAttribute()]
        public virtual void ScenarioTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public virtual void ScenarioSetup(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioStart(scenarioInfo);
        }
        
        public virtual void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        public virtual void FeatureBackground()
        {
#line 6
#line hidden
#line 7
     testRunner.Given("the following binding class", @"
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
}		", ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Should be able to convert steps arguments to arbritrary class instances")]
        public virtual void ShouldBeAbleToConvertStepsArgumentsToArbritraryClassInstances()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Should be able to convert steps arguments to arbritrary class instances", ((string[])(null)));
#line 50
this.ScenarioSetup(scenarioInfo);
#line 6
this.FeatureBackground();
#line hidden
#line 51
 testRunner.Given("the following class", "public class User\r\n{\r\npublic string Name { get; set; }\r\n}", ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 58
 testRunner.And("the following step argument transformation", "[StepArgumentTransformation]\r\npublic User ConvertUser(string name)\r\n{\r\nreturn new" +
                    " User {Name = name};\r\n}", ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 66
 testRunner.And("the following step definition", "[When(@\"(.*) books a flight\")]\r\npublic void WhenSomebodyBooksAFlight(User user)\r\n" +
                    "{}", ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 72
 testRunner.And("a scenario \'Simple Scenario\' as", "When Bob books a flight", ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 76
 testRunner.When("I execute the tests", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 77
 testRunner.Then("the binding method \'WhenSomebodyBooksAFlight\' is executed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Should be able to convert step arguments to simple .NET types")]
        public virtual void ShouldBeAbleToConvertStepArgumentsToSimple_NETTypes()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Should be able to convert step arguments to simple .NET types", ((string[])(null)));
#line 79
this.ScenarioSetup(scenarioInfo);
#line 6
this.FeatureBackground();
#line hidden
#line 80
 testRunner.Given("the following step argument transformation", "[StepArgumentTransformation(@\"in (\\d+) days\")]\r\npublic DateTime ConvertInDays(int" +
                    " days)\r\n{\r\nreturn DateTime.Today.AddDays(days);\r\n}", ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 88
 testRunner.And("the following step definition", "[Given(@\"I have an appointment (.*)\")]\r\npublic void GivenIHaveAnAppointmentAt(Dat" +
                    "eTime time)\r\n{}", ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 94
 testRunner.And("a scenario \'Simple Scenario\' as", "Given I have an appointment in 2 days", ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 98
 testRunner.When("I execute the tests", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 99
 testRunner.Then("the binding method \'GivenIHaveAnAppointmentAt\' is executed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Multi-line text arguments can be converted")]
        public virtual void Multi_LineTextArgumentsCanBeConverted()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Multi-line text arguments can be converted", ((string[])(null)));
#line 101
this.ScenarioSetup(scenarioInfo);
#line 6
this.FeatureBackground();
#line hidden
#line 102
 testRunner.Given("the following step argument transformation", "[StepArgumentTransformation]\r\npublic XmlDocument TransformXml(string xml)\r\n{\r\n   " +
                    " XmlDocument result = new XmlDocument();\r\n    result.LoadXml(xml);\r\n    return r" +
                    "esult;\r\n}", ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 112
 testRunner.And("the following step definition", "[Given(@\"the following XML file\")]\r\npublic void GivenTheFollowingXMLFile(XmlDocum" +
                    "ent xmlDocument)\r\n{}", ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 118
 testRunner.And("a scenario \'Simple Scenario\' as", "Given the following XML file\r\n\'\'\'\r\n<Root>\r\n<Child attr=\"value\" />\r\n</Root>\r\n\'\'\'", ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 127
 testRunner.When("I execute the tests", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 128
 testRunner.Then("the binding method \'GivenTheFollowingXMLFile\' is executed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Multi argument step transformation can be converted")]
        public virtual void MultiArgumentStepTransformationCanBeConverted()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Multi argument step transformation can be converted", ((string[])(null)));
#line 130
 this.ScenarioSetup(scenarioInfo);
#line 6
this.FeatureBackground();
#line hidden
#line 131
 testRunner.Given("the following step argument transformation", "[StepArgumentTransformation]\r\npublic Manager TransformToManager(string name, Date" +
                    "Time dateOfBirth, string storeName)\r\n{\r\n     return new Manager(name, dateOfBirt" +
                    "h, storeName);\r\n}", ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 139
 testRunner.And("the following step definition", @"[Given(""I have a manager called '(.*)' born on (.*) who manages the store '(.*)'"")]
public void GivenIHaveAManager(Manager manager)
{
Console.WriteLine(string.Format(""Name:{0} DOB:{1} Store:{2}"",manager.Name, manager.DateOfBirth.ToString(""dd-MMM-yyyy""), manager.ManagedStore));
} ", ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 147
 testRunner.And("a scenario \'Simple Scenario\' as", "Given I have a manager called \'Bob\' born on 10-Dec-2015 who manages the store \'Sp" +
                    "ringfield\'", ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 151
 testRunner.When("I execute the tests", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 152
 testRunner.Then("the binding method \'GivenIHaveAManager\' is executed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 153
 testRunner.And("the execution log should contain text \'Name:Bob DOB:10-Dec-2015 Store:Springfield" +
                    "\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Multi argument step transformation which are part of a multiple argument step can" +
            " be converted")]
        public virtual void MultiArgumentStepTransformationWhichArePartOfAMultipleArgumentStepCanBeConverted()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Multi argument step transformation which are part of a multiple argument step can" +
                    " be converted", ((string[])(null)));
#line 155
this.ScenarioSetup(scenarioInfo);
#line 6
this.FeatureBackground();
#line hidden
#line 156
 testRunner.Given("the following step argument transformation", "[StepArgumentTransformation]\r\npublic Manager TransformToManager(string name, Date" +
                    "Time dateOfBirth, string storeName)\r\n{\r\n     return new Manager(name, dateOfBirt" +
                    "h, storeName);\r\n}", ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 164
 testRunner.And("the following step argument transformation", "[StepArgumentTransformation]\r\npublic HolidayBooking TransformToHolidayBooking(Dat" +
                    "eTime startDate, int numberOfDays)\r\n{\r\n     return new HolidayBooking(startDate," +
                    " numberOfDays);\r\n}", ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 172
 testRunner.And("the following step definition", @"[Given(""I have a manager called '(.*)' born on (.*) who manages the store '(.*)' who has booked holiday starting on (.*) for (.*) days"")]
public void GivenIHaveAManager(Manager manager, HolidayBooking holidayBooking)
{
Console.WriteLine(string.Format(""Name:{0} DOB:{1} Store:{2}"",manager.Name, manager.DateOfBirth.ToString(""dd-MMM-yyyy""), manager.ManagedStore));
Console.WriteLine(string.Format(""Holiday:{0} Days:{1}"",holidayBooking.StartDate.ToString(""dd-MMM-yyyy""),holidayBooking.NumberOfDays));
} ", ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 181
 testRunner.And("a scenario \'Simple Scenario\' as", "Given I have a manager called \'Bob\' born on 10-Dec-2015 who manages the store \'Sp" +
                    "ringfield\' who has booked holiday starting on 11-Dec-2015 for 8 days", ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 185
 testRunner.When("I execute the tests", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 186
 testRunner.Then("the binding method \'GivenIHaveAManager\' is executed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 187
 testRunner.And("the execution log should contain text \'Name:Bob DOB:10-Dec-2015 Store:Springfield" +
                    "\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 188
 testRunner.And("the execution log should contain text \'Holiday:11-Dec-2015 Days:8\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Multi argument step transformation which are part of a multiple argument step whi" +
            "ch includes tables can be converted")]
        public virtual void MultiArgumentStepTransformationWhichArePartOfAMultipleArgumentStepWhichIncludesTablesCanBeConverted()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Multi argument step transformation which are part of a multiple argument step whi" +
                    "ch includes tables can be converted", ((string[])(null)));
#line 190
this.ScenarioSetup(scenarioInfo);
#line 6
this.FeatureBackground();
#line hidden
#line 191
 testRunner.Given("the following step argument transformation", @"[StepArgumentTransformation]
public Manager TransformToManager(string name, DateTime dateOfBirth, string storeName, Table table)
{
     Manager manager = new Manager(name, dateOfBirth, storeName);
foreach (var row in table.Rows)
{
manager.Employees.Add(new Employee(row[0],row[1]));
}

return manager;
}", ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 205
 testRunner.And("the following step definition", @"[Given(""I have a manager called '(.*)' born on (.*) who manages the store '(.*)' who manages the following employees"")]
public void GivenIHaveAManager(Manager manager)
{
Console.WriteLine(string.Format(""Name:{0} DOB:{1} Store:{2}"",manager.Name, manager.DateOfBirth.ToString(""dd-MMM-yyyy""), manager.ManagedStore));
foreach(var employee in manager.Employees)
{
Console.WriteLine(string.Format(""Employee name:{0} {1}"",employee.FirstName, employee.LastName));
}
} ", ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 217
 testRunner.And("a scenario \'Simple Scenario\' as", "Given I have a manager called \'Bob\' born on 10-Dec-2015 who manages the store \'Sp" +
                    "ringfield\' who manages the following employees\r\n| FirstName | LastName |\r\n| Bob " +
                    "      | Bobbyson |\r\n| Tom       | Thomas   |", ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 224
 testRunner.When("I execute the tests", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 225
 testRunner.Then("the binding method \'GivenIHaveAManager\' is executed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 226
 testRunner.And("the execution log should contain text \'Name:Bob DOB:10-Dec-2015 Store:Springfield" +
                    "\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 227
 testRunner.And("the execution log should contain text \'Employee name:Bob Bobbyson\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 228
 testRunner.And("the execution log should contain text \'Employee name:Tom Thomas\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Multi argument step transformation which are part of a multiple argument step whi" +
            "ch includes tables and simple types can be converted")]
        public virtual void MultiArgumentStepTransformationWhichArePartOfAMultipleArgumentStepWhichIncludesTablesAndSimpleTypesCanBeConverted()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Multi argument step transformation which are part of a multiple argument step whi" +
                    "ch includes tables and simple types can be converted", ((string[])(null)));
#line 230
this.ScenarioSetup(scenarioInfo);
#line 6
this.FeatureBackground();
#line hidden
#line 231
 testRunner.Given("the following step argument transformation", "[StepArgumentTransformation]\r\npublic Manager TransformToManager(string name, Date" +
                    "Time dateOfBirth, string storeName)\r\n{\r\n     Manager manager = new Manager(name," +
                    " dateOfBirth, storeName);\r\nreturn manager;\r\n}", ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 240
 testRunner.Given("the following step argument transformation", "[StepArgumentTransformation]\r\npublic List<string> TransformToNotifications(Table " +
                    "table)\r\n{\r\n     List<string> notifications = new List<String>();\r\nforeach(var ro" +
                    "w in table.Rows)\r\n{\r\nnotifications.Add(row[0]);\r\n}\r\n\r\nreturn notifications;\r\n}", ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 254
 testRunner.And("the following step definition", @"[Given(""I have a manager called '(.*)' born on (.*) who manages the store '(.*)' who was suspended for (.*) days and has the following warning notices"")]
public void GivenIHaveAManager(Manager manager, int suspensionPeriod, List<string> warningNotices)
{
Console.WriteLine(string.Format(""Name:{0} DOB:{1} Store:{2}"",manager.Name, manager.DateOfBirth.ToString(""dd-MMM-yyyy""), manager.ManagedStore));
Console.WriteLine(string.Format(""suspension:{0}"", suspensionPeriod));
foreach(var warningNotice in warningNotices)
{
Console.WriteLine(string.Format(""warning notice:{0}"", warningNotice));
}
} ", ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 267
 testRunner.And("a scenario \'Simple Scenario\' as", "Given I have a manager called \'Bob\' born on 10-Dec-2015 who manages the store \'Sp" +
                    "ringfield\' who was suspended for 8 days and has the following warning notices\r\n|" +
                    " WarningNotices |\r\n| Stealing       | \r\n| Pilfering      | ", ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 274
 testRunner.When("I execute the tests", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 275
 testRunner.Then("the binding method \'GivenIHaveAManager\' is executed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 276
 testRunner.And("the execution log should contain text \'Name:Bob DOB:10-Dec-2015 Store:Springfield" +
                    "\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 277
 testRunner.And("the execution log should contain text \'suspension:8\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 278
 testRunner.And("the execution log should contain text \'warning notice:Pilfering\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 279
 testRunner.And("the execution log should contain text \'warning notice:Stealing\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Multi argument step transformation which contain regexes which consume multiple a" +
            "rguments can be converted")]
        public virtual void MultiArgumentStepTransformationWhichContainRegexesWhichConsumeMultipleArgumentsCanBeConverted()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Multi argument step transformation which contain regexes which consume multiple a" +
                    "rguments can be converted", ((string[])(null)));
#line 281
this.ScenarioSetup(scenarioInfo);
#line 6
this.FeatureBackground();
#line hidden
#line 282
 testRunner.Given("the following step argument transformation", "[StepArgumentTransformation(@\"(\\d*) hour(?:s)?\")]\r\npublic TimeSpan TransformToXml" +
                    "Document(int hours)\r\n{\r\n     return TimeSpan.FromHours(hours);\r\n}", ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 290
 testRunner.And("the following step argument transformation", "[StepArgumentTransformation]\r\npublic List<object> TransformToCollectionOfObjects(" +
                    "string name, string age)\r\n{\r\n     List<object> result = new  List<object>();\r\nre" +
                    "turn result;\r\n}", ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 299
 testRunner.And("the following step argument transformation", "[StepArgumentTransformation]\r\npublic List<int> TransformToCollectionOfObjects(Tab" +
                    "le table)\r\n{\r\n     List<int> result = new  List<int>();\r\nreturn result;\r\n}", ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 308
 testRunner.And("the following step definition", "[Given(\"A time of (.*) which contains the child \'(.*)\' aged (.*) birthday.\")]\r\npu" +
                    "blic void GivenIHaveATimeRangeAndAChild(TimeSpan timeRange, List<object> child, " +
                    "List<int> table)\r\n{\r\n} ", ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 315
 testRunner.And("a scenario \'Simple Scenario\' as", "Given A time of 8 hours which contains the child \'Tom\' aged 5 birthday.\r\n| Headin" +
                    "g1 | Heading2 |\r\n| val      | val2     |\r\n| val4     | val3     |", ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 322
 testRunner.When("I execute the tests", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 323
 testRunner.Then("the binding method \'GivenIHaveATimeRangeAndAChild\' is executed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
