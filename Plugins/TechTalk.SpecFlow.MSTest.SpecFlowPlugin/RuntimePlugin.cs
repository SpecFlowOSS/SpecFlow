using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.MSTest.SpecFlowPlugin;
using TechTalk.SpecFlow.Plugins;
using TechTalk.SpecFlow.Tracing;
using TechTalk.SpecFlow.UnitTestProvider;


[assembly: RuntimePlugin(typeof(RuntimePlugin))]

namespace TechTalk.SpecFlow.MSTest.SpecFlowPlugin
{
    public class RuntimePlugin : IRuntimePlugin
    {
        public void Initialize(RuntimePluginEvents runtimePluginEvents, RuntimePluginParameters runtimePluginParameters, UnitTestProviderConfiguration unitTestProviderConfiguration)
        {
            unitTestProviderConfiguration.UseUnitTestProvider("mstest");
            runtimePluginEvents.RegisterGlobalDependencies += RuntimePluginEvents_RegisterGlobalDependencies;
            runtimePluginEvents.CustomizeTestThreadDependencies += RuntimePluginEventsOnCustomizeTestThreadDependencies;
        }

        private void RuntimePluginEvents_RegisterGlobalDependencies(object sender, RegisterGlobalDependenciesEventArgs e)
        {
            e.ObjectContainer.RegisterTypeAs<MsTestRuntimeProvider, IUnitTestRuntimeProvider>("mstest");
        }

        private void RuntimePluginEventsOnCustomizeTestThreadDependencies(object sender, CustomizeTestThreadDependenciesEventArgs e)
        {
            e.ObjectContainer.RegisterTypeAs<MSTestTraceListener, ITraceListener>();
            e.ObjectContainer.RegisterTypeAs<MSTestAttachmentHandler, ISpecFlowAttachmentHandler>();
            e.ObjectContainer.RegisterTypeAs<MSTestTestContextProvider, IMSTestTestContextProvider>();
        }
    }
}
