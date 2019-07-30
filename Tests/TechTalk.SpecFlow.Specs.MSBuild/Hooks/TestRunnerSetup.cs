using TechTalk.SpecFlow.TestProjectGenerator.Driver;

namespace TechTalk.SpecFlow.Specs.MSBuild.Hooks
{
    [Binding]
    public class TestRunnerSetup
    {
        private readonly ConfigurationDriver _configurationDriver;

        public TestRunnerSetup(ConfigurationDriver configurationDriver)
        {
            _configurationDriver = configurationDriver;
        }

        [BeforeScenario]
        public void SetTestRunner()
        {
            _configurationDriver.SetUnitTestProvider("xunit");
        }
    }
}
