using TechTalk.SpecFlow.Infrastructure;
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
            runtimePluginEvents.CustomizeTestThreadDependencies += RuntimePluginEventsOnCustomizeTestThreadDependencies;
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

        private void RuntimePluginEventsOnCustomizeTestThreadDependencies(object sender, CustomizeTestThreadDependenciesEventArgs e)
        {
            e.ObjectContainer.RegisterTypeAs<NUnitTraceListener, ITraceListener>();
            e.ObjectContainer.RegisterTypeAs<NUnitAttachmentHandler, ISpecFlowAttachmentHandler>();
        }
    }
}
