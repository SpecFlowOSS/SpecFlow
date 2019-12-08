using TechTalk.SpecFlow.TestProjectGenerator.CucumberMessages;

namespace TechTalk.SpecFlow.Specs.StepDefinitions.CucumberMessages
{
    [Binding]
    public class CucumberMessagesFileSteps
    {
        private readonly CucumberMessagesFileDriver _cucumberMessagesFileDriver;

        public CucumberMessagesFileSteps(CucumberMessagesFileDriver cucumberMessagesFileDriver)
        {
            _cucumberMessagesFileDriver = cucumberMessagesFileDriver;
        }

        [Then(@"no Cucumber-Messages file is created")]
        public void ThenNoCucumber_MessagesFileIsCreated()
        {
            _cucumberMessagesFileDriver.CucumberMessagesFileShouldNotExist();
        }

        [Then(@"the Cucumber-Messages file '(.*)' is created")]
        public void ThenTheCucumber_MessagesFileIsCreated(string expectedCucumberMessagesFile)
        {
            _cucumberMessagesFileDriver.CucumberMessagesFileShouldBe(expectedCucumberMessagesFile);
        }
    }
}
