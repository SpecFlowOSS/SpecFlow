using TechTalk.SpecFlow.Specs.MSBuild.Support;
using TechTalk.SpecFlow.TestProjectGenerator;

namespace TechTalk.SpecFlow.Specs.MSBuild.Hooks
{
    [Binding]
    public class DependencySetup
    {
        private readonly ScenarioContext _scenarioContext;

        public DependencySetup(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [BeforeScenario]
        public void RegisterDependencies()
        {
            _scenarioContext.ScenarioContainer.RegisterTypeAs<OutputConnector, IOutputWriter>();
        }
    }
}
