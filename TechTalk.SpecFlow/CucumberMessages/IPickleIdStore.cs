using System;
using TechTalk.SpecFlow.CommonModels;

namespace TechTalk.SpecFlow.CucumberMessages
{
    public interface IPickleIdStore
    {
        Guid GetPickleIdForScenario(ScenarioInfo scenarioInfo);
    }
}
