using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TechTalk.SpecFlow.Plugins
{
    internal class RuntimePluginLocator : IRuntimePluginLocator
    {
        private readonly IRuntimePluginLocationMerger _runtimePluginLocationMerger;
        private readonly ISpecFlowPath _specFlowPath;

        public RuntimePluginLocator(IRuntimePluginLocationMerger runtimePluginLocationMerger, ISpecFlowPath specFlowPath)
        {
            _runtimePluginLocationMerger = runtimePluginLocationMerger;
            _specFlowPath = specFlowPath;
        }

        public IReadOnlyList<string> GetAllRuntimePlugins()
        {
            return GetAllRuntimePlugins(null);
        }

        public IReadOnlyList<string> GetAllRuntimePlugins(string testAssemblyLocation)
        {
            var allRuntimePlugins = new List<string>();

            allRuntimePlugins.AddRange(SearchPluginsInFolder(Environment.CurrentDirectory));
            string specFlowAssemblyFolder = Path.GetDirectoryName(_specFlowPath.GetPathToSpecFlowDll());
            allRuntimePlugins.AddRange(SearchPluginsInFolder(specFlowAssemblyFolder));

            if (testAssemblyLocation.IsNotNullOrWhiteSpace())
            {
                allRuntimePlugins.AddRange(SearchPluginsInFolder(testAssemblyLocation));
            }

            return _runtimePluginLocationMerger.Merge(allRuntimePlugins.Distinct().ToList());
        }

        private List<string> SearchPluginsInFolder(string folder)
        {
            var pluginAssemblies = Directory.EnumerateFiles(folder, "*.SpecFlowPlugin.dll", SearchOption.TopDirectoryOnly).ToList();
            return pluginAssemblies.Select(Path.GetFullPath).ToList();
        }
    }
}