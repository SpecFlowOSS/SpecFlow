using TechTalk.SpecFlow.Specs.Drivers.CucumberMessages;
using TechTalk.SpecFlow.TestProjectGenerator.Driver;

namespace TechTalk.SpecFlow.Specs.StepDefinitions.CucumberMessages
{
    [Binding]
    public class TestSuiteSteps
    {
        private readonly ProjectsDriver _projectsDriver;
        private readonly TestSuiteSetupDriver _testSuiteSetupDriver;

        public TestSuiteSteps(ProjectsDriver projectsDriver, TestSuiteSetupDriver testSuiteSetupDriver)
        {
            _projectsDriver = projectsDriver;
            _testSuiteSetupDriver = testSuiteSetupDriver;
        }

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
