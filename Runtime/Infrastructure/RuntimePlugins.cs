using System.Collections.Generic;

namespace TechTalk.SpecFlow.Infrastructure
{
    public interface IRuntimePlugins
    {
        IEnumerable<LoadedRuntimePlugin> LoadedRuntimePlugins { get; }
    }

    class RuntimePlugins : IRuntimePlugins
    {
        public RuntimePlugins(LoadedRuntimePlugin[] loadedRuntimePlugins)
        {
            LoadedRuntimePlugins = loadedRuntimePlugins;
        }

        public IEnumerable<LoadedRuntimePlugin> LoadedRuntimePlugins { get; }
    }
}