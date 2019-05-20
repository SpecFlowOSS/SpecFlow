using TechTalk.SpecFlow.CommonModels;

namespace TechTalk.SpecFlow.CucumberMessages
{
    public interface IPickleIdStore
    {
        Result GetPickleIdForScenario(ScenarioInfo scenarioInfo);
    }
}
