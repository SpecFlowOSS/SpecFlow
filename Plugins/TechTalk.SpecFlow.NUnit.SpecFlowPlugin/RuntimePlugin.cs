using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.NUnit.SpecFlowPlugin;
using TechTalk.SpecFlow.Plugins;
using TechTalk.SpecFlow.Tracing;
using TechTalk.SpecFlow.UnitTestProvider;


[assembly: RuntimePlugin(typeof(RuntimePlugin))]

namespace TechTalk.SpecFlow.NUnit.SpecFlowPlugin
{
    public class RuntimePlugin : IRuntimePlugin
    {
        public void Initialize(RuntimePluginEvents runtimePluginEvents, RuntimePluginParameters runtimePluginParameters, UnitTestProviderConfiguration unitTestProviderConfiguration)
        {
            runtimePluginEvents.CustomizeScenarioDependencies += RuntimePluginEvents_CustomizeScenarioDependencies;
            unitTestProviderConfiguration.UseUnitTestProvider("nunit");
        }

        private void RuntimePluginEvents_CustomizeScenarioDependencies(object sender, CustomizeScenarioDependenciesEventArgs e)
        {
            var container = e.ObjectContainer;
            
            container.RegisterTypeAs<NUnitTraceListener, ITraceListener>();
        }
    }
}
