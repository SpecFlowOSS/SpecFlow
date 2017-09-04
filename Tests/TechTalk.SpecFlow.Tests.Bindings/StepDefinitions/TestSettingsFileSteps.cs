using TechTalk.SpecFlow.Specs.Drivers;

namespace TechTalk.SpecFlow.Specs.StepDefinitions
{
    [Binding]
    public class TestSettingsFileSteps
    {
        private readonly MsTestTestExecutionDriver msTestTestExecutionDriver;

        public TestSettingsFileSteps(MsTestTestExecutionDriver msTestTestExecutionDriver)
        {
            this.msTestTestExecutionDriver = msTestTestExecutionDriver;
        }

        [Given(@"there is a test settings file '(.*)'")]
        public void GivenThereIsATestSettingsFile(string fileName, string fileContents)
        {
            this.msTestTestExecutionDriver.TestSettingsFile = new TestSettingsFileInput(fileName, fileContents);
        }
    }
}
