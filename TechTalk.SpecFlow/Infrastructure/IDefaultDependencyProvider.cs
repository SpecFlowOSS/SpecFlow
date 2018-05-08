using BoDi;

namespace TechTalk.SpecFlow.Infrastructure
{
    public interface IDefaultDependencyProvider
    {
        void RegisterGlobalContainerDefaults(ObjectContainer container);
        void RegisterTestThreadContainerDefaults(ObjectContainer testThreadContainer);
        void RegisterScenarioContainerDefaults(ObjectContainer scenarioContainer);
    }
}