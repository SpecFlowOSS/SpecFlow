using System;

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
    public class StepArgumentTransformationSteps
    {
        [Given("(.*) has been registered at (.*)")]
        public void RegistrationStep(User user, DateTime dateTime)
        { }       
        
        [Given("(.*) has been registered at (.*)")]
        public void RegistrationStep(User user, Terminal terminal)
        { }     

    }
}
