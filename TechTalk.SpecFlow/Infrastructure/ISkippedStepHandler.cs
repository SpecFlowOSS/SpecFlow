using TechTalk.SpecFlow.Bindings;

namespace TechTalk.SpecFlow.Infrastructure
{
    public interface ISkippedStepHandler
    {
        void Handle(ScenarioContext scenarioContext);
    }
}