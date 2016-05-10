using System.Collections.Generic;
using System.Linq;
using BoDi;
using TechTalk.SpecFlow.Configuration;

namespace TechTalk.SpecFlow.Infrastructure
{
    public interface IRuntimePluginsLoader
    {
        IRuntimePlugins LoadRuntimePlugins(IRuntimeConfigurationProvider configurationProvider);

    }

    public class RuntimePluginsLoader : IRuntimePluginsLoader
    {
        private readonly ObjectContainer _objectContainer;
        private readonly IRuntimePluginLoader _runtimePluginLoader;

        public RuntimePluginsLoader(ObjectContainer objectContainer, IRuntimePluginLoader runtimePluginLoader)
        {
            _objectContainer = objectContainer;
            _runtimePluginLoader = runtimePluginLoader;
        }

        public IRuntimePlugins LoadRuntimePlugins(IRuntimeConfigurationProvider configurationProvider)
        {
            var plugins = _objectContainer.Resolve<IDictionary<string, IRuntimePlugin>>().Values.AsEnumerable();

            plugins = plugins.Concat(configurationProvider.GetPlugins().Where(pd => (pd.Type & PluginType.Runtime) != 0).Select(_runtimePluginLoader.LoadPlugin));

            var runtimePluginEvents = new List<LoadedRuntimePlugin>();
            foreach (var runtimePlugin in plugins)
            {
                var loadedPlugin = new LoadedRuntimePlugin(runtimePlugin, new RuntimePluginEvents());

                runtimePlugin.Initialize(loadedPlugin.RuntimePluginEvents, new RuntimePluginParameters());

                runtimePluginEvents.Add(loadedPlugin);
            }

            return new RuntimePlugins(runtimePluginEvents.ToArray());
        }
    }
}