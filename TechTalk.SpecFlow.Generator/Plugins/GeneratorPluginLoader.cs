using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Plugins;

namespace TechTalk.SpecFlow.Generator.Plugins
{
    public class GeneratorPluginLoader : IGeneratorPluginLoader
    {
        /*
         * Loading logic:
         * <plugin-folder> = @path | <generator-folder> | <nuget-plugin-folder>
         * 
         * <configured-path> = @path | <project-folder>\@path
         * 
         * <nuget-plugin-folder> = <nuget-packages>\<plugin-name>.SpecFlowPlugin.<nuget-package-version> | <nuget-packages>\<plugin-name>.SpecFlow.<nuget-package-version> | <nuget-packages>\<plugin-name>.<nuget-package-version> // match first for the one with <specflow-version>
         * 
         * <specflow-version> = n-n[-n]  // e.g. 1-8-1
         * 
         * <nuget-packages> = <generator-folder>\..\..      // assuming that SpecFlow was installed with nuget, generator is in the "tools" folder
         * 
         * <nuget-package-version> = latest-of: n(.n)*[-tag]
         * 
         * <plugin-generator-folder> = <plugin-folder> | <plugin-folder>\tools\SpecFlowPlugin[.<specflow-version>] | <plugin-folder>\tools | <plugin-folder>\lib\net45 | <plugin-folder>\lib\net40 | <plugin-folder>\lib\net35 | <plugin-folder>\lib
         * 
         * <generator-plugin-assembly> = <plugin-generator-folder>\<plugin-name>.Generator.SpecFlowPlugin.dll | <generator-plugin-folder>\<plugin-name>.SpecFlowPlugin.dll
         */

        private readonly string generatorFolder;
        private readonly ProjectSettings projectSettings;

        public GeneratorPluginLoader(ProjectSettings projectSettings)
        {
            this.projectSettings = projectSettings;
            this.generatorFolder = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
        }

        public IGeneratorPlugin LoadPlugin(PluginDescriptor pluginDescriptor)
        {
            var generatorPluginAssemblyPath = GetGeneratorPluginAssemblies(pluginDescriptor).FirstOrDefault();
            if (generatorPluginAssemblyPath == null)
                throw new SpecFlowException(string.Format("Unable to find plugin in the plugin search path: {0}. Please check http://go.specflow.org/doc-plugins for details.", pluginDescriptor.Name));

            Assembly pluginAssembly;
            try
            {
                pluginAssembly = Assembly.LoadFrom(generatorPluginAssemblyPath);
            }
            catch(Exception ex)
            {
                throw new SpecFlowException(string.Format("Unable to load plugin assembly: {0}. Please check http://go.specflow.org/doc-plugins for details.", generatorPluginAssemblyPath), ex);
            }

            var pluginAttribute = (GeneratorPluginAttribute)Attribute.GetCustomAttribute(pluginAssembly, typeof(GeneratorPluginAttribute));
            if (pluginAttribute == null)
                throw new SpecFlowException("Missing [assembly:GeneratorPlugin] attribute in " + generatorPluginAssemblyPath);

            if (!typeof(IGeneratorPlugin).IsAssignableFrom((pluginAttribute.PluginType)))
                throw new SpecFlowException(string.Format("Invalid plugin attribute in {0}. Plugin type must implement IGeneratorPlugin. Please check http://go.specflow.org/doc-plugins for details.", generatorPluginAssemblyPath));

            IGeneratorPlugin plugin;
            try
            {
                plugin = (IGeneratorPlugin)Activator.CreateInstance(pluginAttribute.PluginType);
            }
            catch (Exception ex)
            {
                throw new SpecFlowException(string.Format("Invalid plugin in {0}. Plugin must have a default constructor that does not throw exception. Please check http://go.specflow.org/doc-plugins for details.", generatorPluginAssemblyPath), ex);
            }

            return plugin;
        }

        private IEnumerable<string> GetGeneratorPluginAssemblies(PluginDescriptor pluginDescriptor)
        {
            foreach (var pluginGeneratorFolder in GetPluginGeneratorFolders(pluginDescriptor))
            {
                string generatorSpecificAssembly = Path.GetFullPath(Path.Combine(pluginGeneratorFolder, string.Format("{0}.Generator.SpecFlowPlugin.dll", pluginDescriptor.Name)));
                generatorSpecificAssembly = Environment.ExpandEnvironmentVariables(generatorSpecificAssembly);

                if (File.Exists(generatorSpecificAssembly))
                    yield return generatorSpecificAssembly;

                string genericAssembly = Path.GetFullPath(Path.Combine(pluginGeneratorFolder, string.Format("{0}.SpecFlowPlugin.dll", pluginDescriptor.Name)));
                genericAssembly = Environment.ExpandEnvironmentVariables(genericAssembly);
                if (File.Exists(genericAssembly))
                    yield return genericAssembly;
            }
        }

        private IEnumerable<string> GetPluginGeneratorFolders(PluginDescriptor pluginDescriptor)
        {
            var pluginGeneratorFolders = (new[] { @"" })
                .Concat(GetSpecFlowVersionSpecifiers().Select(v => @"tools\SpecFlowPlugin" + v))
                .Concat(new[] { @"tools", @"lib\net45", @"lib\net40", @"lib\net35", @"lib"});

            return GetPluginFolders(pluginDescriptor).SelectMany(pluginFolder => pluginGeneratorFolders, Path.Combine);
        }

        private IEnumerable<string> GetPluginFolders(PluginDescriptor pluginDescriptor)
        {
            if (pluginDescriptor.Path != null)
            {
                yield return Path.Combine(projectSettings.ProjectFolder, pluginDescriptor.Path);
                yield break;
            }

            yield return generatorFolder;

            foreach (var nuGetPluginFolder in GetNuGetPluginFolders(pluginDescriptor))
                yield return nuGetPluginFolder;
        }

        private static readonly string[] pluginPostfixes = new[] { @".SpecFlowPlugin", @".SpecFlow", @"" };

        private IEnumerable<string> GetNuGetPluginFolders(PluginDescriptor pluginDescriptor)
        {
            string nuGetPackagesFolder = GetNuGetPackagesFolder();

            return pluginPostfixes
                .Select(pluginPostfix => pluginDescriptor.Name + pluginPostfix)
                .Select(packageName => GetLatestPackage(nuGetPackagesFolder, packageName))
                .Where(pluginFolder => pluginFolder != null);
        }

        private static string GetLatestPackage(string nuGetPackagesFolder, string packageName)
        {
            return Directory.GetDirectories(nuGetPackagesFolder, packageName + ".*").OrderByDescending(d => d).FirstOrDefault();
        }

        private IEnumerable<string> GetSpecFlowVersionSpecifiers()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            yield return string.Format(".{0}-{1}-{2}", version.Major, version.Minor, version.Revision);
            yield return string.Format(".{0}-{1}", version.Major, version.Minor);
            yield return "";
        }

        private string GetNuGetPackagesFolder()
        {
            return Path.Combine(generatorFolder, @"..\.."); // assuming that SpecFlow was installed with nuget, generator is in the "tools" folder
        }
    }
}