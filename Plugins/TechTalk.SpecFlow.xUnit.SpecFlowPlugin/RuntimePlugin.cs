using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Plugins;
using TechTalk.SpecFlow.Tracing;
using TechTalk.SpecFlow.UnitTestProvider;
using TechTalk.SpecFlow.xUnit.SpecFlowPlugin;


[assembly: RuntimePlugin(typeof(RuntimePlugin))]


namespace TechTalk.SpecFlow.xUnit.SpecFlowPlugin
{
    public class RuntimePlugin : IRuntimePlugin
    {
        public void Initialize(RuntimePluginEvents runtimePluginEvents, RuntimePluginParameters runtimePluginParameters, UnitTestProviderConfiguration unitTestProviderConfiguration)
        {
            runtimePluginEvents.RegisterGlobalDependencies += RuntimePluginEvents_RegisterGlobalDependencies;
            runtimePluginEvents.CustomizeTestThreadDependencies += RuntimePluginEvents_CustomizeTestThreadDependencies;
            unitTestProviderConfiguration.UseUnitTestProvider("xunit");
        }

        private void RuntimePluginEvents_RegisterGlobalDependencies(object sender, RegisterGlobalDependenciesEventArgs e)
        {
            e.ObjectContainer.RegisterTypeAs<XUnitRuntimeProvider, IUnitTestRuntimeProvider>("xunit");
        }

        private void RuntimePluginEvents_CustomizeTestThreadDependencies(object sender, CustomizeTestThreadDependenciesEventArgs e)
        {
            var container = e.ObjectContainer;

            container.RegisterTypeAs<XUnitTraceListener, ITraceListener>();
        }
    }
}
