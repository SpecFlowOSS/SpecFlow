using TechTalk.SpecFlow.UnitTestProvider;

namespace TechTalk.SpecFlow.Plugins
{
    public interface IRuntimePlugin
    {
        void Initialize(RuntimePluginEvents runtimePluginEvents, RuntimePluginParameters runtimePluginParameters, UnitTestProviderConfiguration unitTestProviderConfiguration);
    }
}
