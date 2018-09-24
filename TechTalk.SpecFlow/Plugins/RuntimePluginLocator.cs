using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TechTalk.SpecFlow.Plugins
{
    internal class RuntimePluginLocator : IRuntimePluginLocator
    {
        private readonly IRuntimePluginLocationMerger _runtimePluginLocationMerger;
        private readonly string _pathToFolderWithSpecFlowAssembly;

        public RuntimePluginLocator(IRuntimePluginLocationMerger runtimePluginLocationMerger)
        {
            _runtimePluginLocationMerger = runtimePluginLocationMerger;
            _pathToFolderWithSpecFlowAssembly = Path.GetDirectoryName(typeof(RuntimePluginLocator).Assembly.Location);
        }

        public IReadOnlyList<string> GetAllRuntimePlugins()
        {
            var allRuntimePlugins = new List<string>();

            allRuntimePlugins.AddRange(SearchPluginsInFolder(Environment.CurrentDirectory));
            allRuntimePlugins.AddRange(SearchPluginsInFolder(_pathToFolderWithSpecFlowAssembly));



            return _runtimePluginLocationMerger.Merge(allRuntimePlugins.Distinct().ToList());
        }

        private List<string> SearchPluginsInFolder(string folder)
        {
            var pluginAssemblies = Directory.EnumerateFiles(folder, "*.SpecFlowPlugin.dll", SearchOption.TopDirectoryOnly).ToList();
            return pluginAssemblies.Select(Path.GetFullPath).ToList();
        }
    }
}