using System;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Plugins;
using TechTalk.SpecFlow.xUnit.SpecFlowPlugin;


[assembly: RuntimePlugin(typeof(RuntimePlugin))]


namespace TechTalk.SpecFlow.xUnit.SpecFlowPlugin
{
    public class RuntimePlugin : IRuntimePlugin
    {
        public void Initialize(RuntimePluginEvents runtimePluginEvents, RuntimePluginParameters runtimePluginParameters)
        {
            runtimePluginEvents.CustomizeScenarioDependencies += RuntimePluginEvents_CustomizeScenarioDependencies;
        }

        private void RuntimePluginEvents_CustomizeScenarioDependencies(object sender, CustomizeScenarioDependenciesEventArgs e)
        {
            var container = e.ObjectContainer;

            container.RegisterTypeAs<OutputHelper, ISpecFlowOutputHelper>();
        }
    }
}
