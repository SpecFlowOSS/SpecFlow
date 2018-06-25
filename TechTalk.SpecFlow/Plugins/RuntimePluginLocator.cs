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

            var pluginAssemblies = Directory.EnumerateFiles(Environment.CurrentDirectory, "*.SpecFlowPlugin.dll", SearchOption.TopDirectoryOnly);


            foreach (var referencedAssembly in pluginAssemblies)
            {
                if (IsRuntimePlugin(referencedAssembly))
                {
                    allRuntimePlugins.Add(Path.GetFullPath(referencedAssembly));
                }
            }

            return allRuntimePlugins;
        }


        public bool IsRuntimePlugin(string assemblyFile)
        {
            Assembly.ReflectionOnlyLoadFrom(GetType().Assembly.Location);
            var assembly = Assembly.ReflectionOnlyLoadFrom(assemblyFile);

            return assembly.GetCustomAttributesData().Where(a => a.AttributeType.FullName == "TechTalk.SpecFlow.Plugins.RuntimePluginAttribute").Any();
        }
    }
}