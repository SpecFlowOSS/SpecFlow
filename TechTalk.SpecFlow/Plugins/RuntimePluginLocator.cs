using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace TechTalk.SpecFlow.Plugins
{
    internal class RuntimePluginLocator : IRuntimePluginLocator
    {
        public IReadOnlyList<string> GetAllRuntimePlugins()
        {
            var allRuntimePlugins = new List<string>();

            //var currentAssembly = Assembly.GetCallingAssembly();
            //string currentDirectory = Path.GetDirectoryName(currentAssembly.Location);

            var pluginAssemblies = Directory.EnumerateFiles(Environment.CurrentDirectory, "*.SpecFlowPlugin.dll", SearchOption.TopDirectoryOnly).ToList();

            allRuntimePlugins.AddRange(pluginAssemblies.Select(Path.GetFullPath));

            return allRuntimePlugins;
        }
    }
}