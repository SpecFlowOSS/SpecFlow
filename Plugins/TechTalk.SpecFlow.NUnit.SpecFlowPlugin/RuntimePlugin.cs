using TechTalk.SpecFlow.NUnit.SpecFlowPlugin;
using TechTalk.SpecFlow.Plugins;
using TechTalk.SpecFlow.TestFramework;
using TechTalk.SpecFlow.Tracing;
using TechTalk.SpecFlow.UnitTestProvider;

[assembly: RuntimePlugin(typeof(RuntimePlugin))]

namespace TechTalk.SpecFlow.NUnit.SpecFlowPlugin
{
    public class RuntimePlugin : IRuntimePlugin
    {
        public void Initialize(RuntimePluginEvents runtimePluginEvents, RuntimePluginParameters runtimePluginParameters, UnitTestProviderConfiguration unitTestProviderConfiguration)
        {
            runtimePluginEvents.RegisterGlobalDependencies += RuntimePluginEventsOnRegisterGlobalDependencies;
            runtimePluginEvents.CustomizeGlobalDependencies += RuntimePluginEvents_CustomizeGlobalDependencies;
            runtimePluginEvents.CustomizeScenarioDependencies += RuntimePluginEvents_CustomizeScenarioDependencies;
            unitTestProviderConfiguration.UseUnitTestProvider("nunit");
        }

        private void RuntimePluginEventsOnRegisterGlobalDependencies(object sender, RegisterGlobalDependenciesEventArgs e)
        {
            e.ObjectContainer.RegisterTypeAs<NUnitRuntimeProvider, IUnitTestRuntimeProvider>("nunit");
        }

        private void RuntimePluginEvents_CustomizeGlobalDependencies(object sender, CustomizeGlobalDependenciesEventArgs e)
        {
#if NETFRAMEWORK
            e.ObjectContainer.RegisterTypeAs<NUnitNetFrameworkTestRunContext, ITestRunContext>();
#endif
        }

        private void RuntimePluginEvents_CustomizeScenarioDependencies(object sender, CustomizeScenarioDependenciesEventArgs e)
        {
            var container = e.ObjectContainer;
            
            container.RegisterTypeAs<NUnitTraceListener, ITraceListener>();
        }
    }
}
