using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using TechTalk.SpecFlow.Infrastructure;

namespace TechTalk.SpecFlow.Plugins
{
    internal class RuntimePluginLocator : IRuntimePluginLocator
    {
        private readonly IRuntimePluginLocationMerger _runtimePluginLocationMerger;
        private readonly ISpecFlowPath _specFlowPath;
        private readonly Assembly _testAssembly;

        public RuntimePluginLocator(IRuntimePluginLocationMerger runtimePluginLocationMerger, ISpecFlowPath specFlowPath, ITestAssemblyProvider testAssemblyProvider)
        {
            _runtimePluginLocationMerger = runtimePluginLocationMerger;
            _specFlowPath = specFlowPath;
            _testAssembly = testAssemblyProvider.TestAssembly;
        }

        public IReadOnlyList<string> GetAllRuntimePlugins()
        {
            var allRuntimePlugins = new List<string>();

            allRuntimePlugins.AddRange(SearchPluginsInFolder(Environment.CurrentDirectory));
            string specFlowAssemblyFolder = Path.GetDirectoryName(_specFlowPath.GetPathToSpecFlowDll());
            allRuntimePlugins.AddRange(SearchPluginsInFolder(specFlowAssemblyFolder));

            var assemblyLocation = _testAssembly != null && !_testAssembly.IsDynamic ? _testAssembly.Location : null;
            if (assemblyLocation.IsNotNullOrWhiteSpace())
            {
                allRuntimePlugins.Add(assemblyLocation);
                allRuntimePlugins.AddRange(SearchPluginsInFolder(Path.GetDirectoryName(assemblyLocation)));
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