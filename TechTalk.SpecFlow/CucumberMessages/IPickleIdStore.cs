using System;

namespace TechTalk.SpecFlow.CucumberMessages
{
    public interface IPickleIdStore
    {
        Guid GetPickleIdForScenario(ScenarioInfo scenarioInfo);
    }
}
