using System.Linq;
using TechTalk.SpecFlow.TestProjectGenerator.CucumberMessages;

namespace TechTalk.SpecFlow.Specs.StepDefinitions.CucumberMessages
{
    [Binding]
    public class TestSuiteSteps
    {
        private readonly TestSuiteSetupDriver _testSuiteSetupDriver;
        private readonly ScenarioContext _scenarioContext;

        public TestSuiteSteps(TestSuiteSetupDriver testSuiteSetupDriver, ScenarioContext scenarioContext)
        {
            _testSuiteSetupDriver = testSuiteSetupDriver;
            _scenarioContext = scenarioContext;
        }

        [Given(@"there are '(\d+)' feature files")]
        [Given(@"there are (\d+) feature files")]
        public void GivenThereAreFeatureFiles(int featureFilesCount)
        {
            if (_scenarioContext.ScenarioInfo.Tags.Contains("SpecFlow"))
            {
                return;
            }

            _testSuiteSetupDriver.AddFeatureFiles(featureFilesCount);
        }



        [Given(@"the cucumber implementation '(.*)'")]
        public void GivenTheCucumberImplementationIs(string cucumberImplementation)
        {
            if (_scenarioContext.ScenarioInfo.Tags.Contains("SpecFlow"))
            {
                return;
            }

            _testSuiteSetupDriver.AddFeatureFiles(1);
            _testSuiteSetupDriver.AddGenericWhenStepBinding();
        }



        [Given(@"the test runner is '(.*)'")]
        [Scope(Tag = "SpecFlow")]
        public void GivenTheTestRunnerIs(string testRunner)
        {
            
        }

        [When(@"the test suite is executed with a testThreadCount of '(.*)'")]
        [Scope(Tag = "SpecFlow")]
        public void WhenTheTestSuiteIsExecutedWithATestThreadCountOf(int p0)
        {
            
        }


    }
}
