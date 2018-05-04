using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpecFlow.TestProjectGenerator.NewApi;

namespace TechTalk.SpecFlow.Specs.Support
{
    [Binding]
    public class Hooks
    {
        private readonly ScenarioContext _scenarioContext;

        public Hooks(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [BeforeScenario()]
        public void BeforeScenario()
        {
            _scenarioContext.ScenarioContainer.RegisterTypeAs<OutputConnector, IOutputWriter>();
        }
    }
}
