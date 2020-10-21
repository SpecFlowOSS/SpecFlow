using NUnit.Framework;
using TechTalk.SpecFlow;

namespace Specs.Steps
{
    [Binding]
    public class RegistrationBindings
    {
        private readonly ScenarioContext _scenarioContext;

        private string _name;
        private string _email;
        private string _errorMessage = "";

        public RegistrationBindings(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }
        [Given(@"a visitor registering as ""(.*)"" with email (.*)")]
        public void GivenAVisitorRegisteringAsWithEmail(string name, string email)
        {
            this._email = email;
            this._name = name;
        }

        [When(@"the registration completes")]
        public void WhenTheRegistrationCompletes()
        {
            //Simulate some email validation
            if (!_email.Contains("@"))
            {
                _email = null;
                _errorMessage = "Invalid Email";
            }
        }

        [Then(@"the account system should record (.*) related to user ""(.*)""")]
        public void ThenTheAccountSystemShouldRecordEmailToUser(string email, string name)
        {
            Assert.AreEqual(name, _name);
            Assert.AreEqual(email, _email);
        }

        [Then(@"the account system should not record (.*)")]
        public void ThenTheAccountSystemShouldNotRecord(string p0)
        {
            Assert.IsNull(_email);
        }

        [Then(@"the error response should be ""(.*)""")]
        public void ThenTheErrorResponseShouldBe(string expectedErrorMessage)
        {
            Assert.AreEqual(expectedErrorMessage, _errorMessage);
        }

    }
}