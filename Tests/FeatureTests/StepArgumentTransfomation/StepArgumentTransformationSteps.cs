using System;
using System.Threading;
using System.Xml;
using NUnit.Framework;

namespace TechTalk.SpecFlow.FeatureTests.StepArgumentTransfomation
{
    public class User
    {
        public string Name { get; set; }
    }

    public class Terminal
    {
        public string Id { get; set; }
    }

    public class Booking
    {
        public string Origin;
        public string Destination;
        public DateTime DepartureDate;
    }

    [Binding]
    public class UserLookup 
    {
        [StepArgumentTransformation]
        public User Transform(string name)
        {
            return new User {Name = name};
        }
    }

    [Binding]
    public class DateConverter 
    {
        [StepArgumentTransformation("date (.*)")]
        public DateTime Transform(string dateString)
        {
            return DateTime.Parse(dateString);
        }
    }

    [Binding]
    public class TerminalConverter 
    {
        [StepArgumentTransformation("terminal (.*)")]
        public Terminal Transform(string terminalId)
        {
            return new Terminal { Id = terminalId };
        }
    }

    [Binding]
    public class BookingConverter
    {
        [StepArgumentTransformation]
        public Booking Transform(Table table)
        {
            return new Booking
            {
                Origin = table.Rows[0]["Origin"],
                Destination = table.Rows[0]["Destination"],
                DepartureDate = DateTime.Parse(table.Rows[0]["Departure Date"])
            };
        }
    }

    [Binding]
    public class XmlDocumentConverter
    {
        [StepArgumentTransformation]
        public XmlDocument Transform(string xml)
        {
            XmlDocument result = new XmlDocument();
            result.LoadXml(xml);
            return result;
        }
    }

    [Binding]
    public class StepArgumentTransformationSteps
    {
        [Given("(.*) has been registered at (.*)")]
        public void RegistrationStep(User user, DateTime dateTime)
        { }       
        
        [Given("(.*) has been registered at (.*)")]
        public void RegistrationStep(User user, Terminal terminal)
        { }

        [Given("(.*) has booked a flight")]
        public void FlightBookingStep(User user, Booking booking)
        { }

        [When("in App.config die bindingCulture auf 'en-US' konfiguriert ist")]
        public void AppConfig()
        {
            // check App.config of project
        }       
        
        [Then("ist (.*) kleiner als (.*)")]
        public void SmallerThan(double val1, int val2)
        {
            Assert.Less(val1, val2);
        }

        [Then("die CurrentCulture während der Ausführung des Steps ist '(.*)'")]
        public void RegistrationStep(string culture)
        {
            Assert.AreEqual(culture, Thread.CurrentThread.CurrentCulture.Name);
        }

        [Given(@"I have a step binding with an XmlDocument parameter")]
        [Given(@"a step argument converter from string to XmlDocument")]
        public void GivenIHaveAStepBindingWithAnXmlDocumentParameter()
        {
            //nop
        }

        private XmlDocument savedXml = null;

        [When(@"a step is executed that macthes to the binding with a long XML argument")]
        public void WhenAStepIsExecutedThatMacthesToTheBindingWithALongXMLArgument(XmlDocument xml)
        {
            Assert.IsNotNull(xml);
            savedXml = xml;
        }

        [Then(@"the parsed XML is passed to the step binding")]
        public void ThenTheParsedXMLIsPassedToTheStepBinding()
        {
            Assert.IsNotNull(savedXml);
            StringAssert.Contains("<Root>", savedXml.OuterXml);
        }
    }
}
