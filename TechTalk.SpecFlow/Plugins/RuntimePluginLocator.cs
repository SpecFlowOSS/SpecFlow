using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using TechTalk.SpecFlow.Infrastructure;

namespace TechTalk.SpecFlow.Plugins
{
    internal sealed class RuntimePluginLocator : IRuntimePluginLocator
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

            var currentDirectory = Environment.CurrentDirectory;
            allRuntimePlugins.AddRange(SearchPluginsInFolder(currentDirectory));

            // Check to not search the same directory twice
            var specFlowAssemblyDirectory = Path.GetDirectoryName(Path.GetFullPath(_specFlowPath.GetPathToSpecFlowDll()));
            if (currentDirectory != specFlowAssemblyDirectory)
            {
                allRuntimePlugins.AddRange(SearchPluginsInFolder(specFlowAssemblyDirectory));
            }

            var assemblyLocation = _testAssembly != null && !_testAssembly.IsDynamic ? _testAssembly.Location : null;
            if (assemblyLocation.IsNotNullOrWhiteSpace() && !allRuntimePlugins.Contains(assemblyLocation))
            {
                allRuntimePlugins.Add(assemblyLocation);

                // Check to not search the same directory twice
                var assemblyDirectory = Path.GetDirectoryName(assemblyLocation);
                if (currentDirectory != assemblyDirectory && specFlowAssemblyDirectory != assemblyDirectory)
                {
                    allRuntimePlugins.AddRange(SearchPluginsInFolder(assemblyDirectory));
                }
            }

            return _runtimePluginLocationMerger.Merge(allRuntimePlugins);
        }

        private static IEnumerable<string> SearchPluginsInFolder(string folder)
        {
            return Directory.EnumerateFiles(folder, "*.SpecFlowPlugin.dll", SearchOption.TopDirectoryOnly);
        }
    }
}