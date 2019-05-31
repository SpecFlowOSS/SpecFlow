using TechTalk.SpecFlow.Specs.Drivers.CucumberMessages;

namespace TechTalk.SpecFlow.Specs.StepDefinitions.CucumberMessages
{
    [Binding]
    public class TestSuiteSteps
    {
        private readonly TestSuiteSetupDriver _testSuiteSetupDriver;

        public TestSuiteSteps(TestSuiteSetupDriver testSuiteSetupDriver)
        {
            _testSuiteSetupDriver = testSuiteSetupDriver;
        }

        [Given(@"there are '(\d+)' feature files")]
        [Given(@"there are (\d+) feature files")]
        public void GivenThereAreFeatureFiles(int featureFilesCount)
        {
            _testSuiteSetupDriver.AddFeatureFiles(featureFilesCount);
        }

        [Given(@"the cucumber implementation is (.*)")]
        public void GivenTheCucumberImplementationIs(string cucumberImplementation)
        {
            _testSuiteSetupDriver.AddFeatureFiles(1);
        }
    }
}
