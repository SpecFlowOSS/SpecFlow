using TechTalk.SpecFlow.TestProjectGenerator;

namespace TechTalk.SpecFlow.Specs.MSBuild.Hooks
{
    [Binding]
    public class DefaultProjectSetup
    {
        private readonly TestRunConfiguration _testRunConfiguration;

        public DefaultProjectSetup(TestRunConfiguration testRunConfiguration)
        {
            _testRunConfiguration = testRunConfiguration;
        }

        [BeforeScenario]
        public void ConfigureDefaultProject()
        {
            _testRunConfiguration.ProgrammingLanguage = TestProjectGenerator.ProgrammingLanguage.CSharp;
            _testRunConfiguration.ProjectFormat = TestProjectGenerator.Data.ProjectFormat.New;
            _testRunConfiguration.TargetFramework = TestProjectGenerator.Data.TargetFramework.Netcoreapp22;
        }
    }
}
