using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.Plugins;
using TechTalk.SpecFlow.Utils;

namespace TechTalk.SpecFlow.Generator.Plugins
{
    /// <summary>
    /// Loading logic:
    /// <plugin-folder> = @path | <generator-folder> | <nuget-plugin-folder>
    /// 
    /// <configured-path> = @path | <project-folder>\@path
    /// 
    /// <nuget-plugin-folder> = <nuget-packages>\<plugin-name>.SpecFlowPlugin.<nuget-package-version> | <nuget-packages>\<plugin-name>.SpecFlow.<nuget-package-version> | <nuget-packages>\<plugin-name>.<nuget-package-version> // match first for the one with <specflow-version>
    /// 
    /// <specflow-version> = n-n[-n]  // e.g. 1-8-1
    /// 
    /// <nuget-packages> = <generator-folder>\..\..      // assuming that SpecFlow was installed with nuget, generator is in the "tools" folder
    /// 
    /// <nuget-package-version> = latest-of: n(.n)*[-tag]
    /// 
    /// <plugin-generator-folder> = <plugin-folder> | <plugin-folder>\tools\SpecFlowPlugin[.<specflow-version>] | <plugin-folder>\tools | <plugin-folder>\lib\net45 | <plugin-folder>\lib\net40 | <plugin-folder>\lib\net35 | <plugin-folder>\lib
    /// 
    /// <generator-plugin-assembly> = <plugin-generator-folder>\<plugin-name>.Generator.SpecFlowPlugin.dll | <generator-plugin-folder>\<plugin-name>.SpecFlowPlugin.dll
    /// </summary>
    public class GeneratorPluginLocator : IGeneratorPluginLocator
    {
        private readonly string generatorFolder;
        private readonly ProjectSettings projectSettings;
        private readonly IFileSystem fileSystem;

        public GeneratorPluginLocator(ProjectSettings projectSettings, IFileSystem fileSystem)
            : this(projectSettings, null, fileSystem)
        {
            this.generatorFolder = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
        }

        internal GeneratorPluginLocator(ProjectSettings projectSettings, string generatorFolder, IFileSystem fileSystem)
        {
            this.projectSettings = projectSettings;
            this.generatorFolder = generatorFolder;
            this.fileSystem = fileSystem;
        }

        public String LocatePluginAssembly(PluginDescriptor pluginDescriptor)
        {
            return
                GetGeneratorPluginAssemblies(pluginDescriptor).FirstOrDefault() ??
                throw new SpecFlowException($"Unable to find plugin in the plugin search path: {pluginDescriptor.Name}. Please check http://go.specflow.org/doc-plugins for details.");
        }

        private IEnumerable<string> GetGeneratorPluginAssemblies(PluginDescriptor pluginDescriptor)
        {
            if (pluginDescriptor.Path != null)
            {
                var pluginGeneratorFolder = Environment.ExpandEnvironmentVariables(pluginDescriptor.Path);

                string generatorSpecificAssembly = Path.GetFullPath(Path.Combine(pluginGeneratorFolder, string.Format("{0}.Generator.SpecFlowPlugin.dll", pluginDescriptor.Name)));
                if (this.fileSystem.FileExists(generatorSpecificAssembly))
                    yield return generatorSpecificAssembly;

                string genericAssembly = Path.GetFullPath(Path.Combine(pluginGeneratorFolder, string.Format("{0}.SpecFlowPlugin.dll", pluginDescriptor.Name)));
                if (this.fileSystem.FileExists(genericAssembly))
                    yield return genericAssembly;
            }

            foreach (var pluginGeneratorFolder in GetPluginGeneratorFolders(pluginDescriptor))
            {
                string generatorSpecificAssembly = Path.GetFullPath(Path.Combine(pluginGeneratorFolder, string.Format("{0}.Generator.SpecFlowPlugin.dll", pluginDescriptor.Name)));
                generatorSpecificAssembly = Environment.ExpandEnvironmentVariables(generatorSpecificAssembly);

                if (this.fileSystem.FileExists(generatorSpecificAssembly))
                    yield return generatorSpecificAssembly;

                string genericAssembly = Path.GetFullPath(Path.Combine(pluginGeneratorFolder, string.Format("{0}.SpecFlowPlugin.dll", pluginDescriptor.Name)));
                genericAssembly = Environment.ExpandEnvironmentVariables(genericAssembly);
                if (this.fileSystem.FileExists(genericAssembly))
                    yield return genericAssembly;
            }
        }

        private IEnumerable<string> GetPluginGeneratorFolders(PluginDescriptor pluginDescriptor)
        {
            var pluginGeneratorFolders = (new[] { @"" })
                .Concat(GetSpecFlowVersionSpecifiers().Select(v => @"tools\SpecFlowPlugin" + v))
                .Concat(new[] { @"tools", @"lib\net45", @"lib\net40", @"lib\net35", @"lib" });

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

        private string GetLatestPackage(string nuGetPackagesFolder, string packageName)
        {
            if (!this.fileSystem.DirectoryExists(nuGetPackagesFolder))
            {
                return null;
            }

            var directory = this.fileSystem.GetDirectories(nuGetPackagesFolder, packageName + ".*").OrderByDescending(d => d).FirstOrDefault();
            if (directory != null)
            {
                return directory;
            }

            var path = Path.Combine(nuGetPackagesFolder, packageName);
            if (!this.fileSystem.DirectoryExists(path))
            {
                return null;
            }

            return this.fileSystem.GetDirectories(path).OrderByDescending(d => d).FirstOrDefault();
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
            var directory = new DirectoryInfo(this.generatorFolder);

            do
            {
                if (directory.Name.Equals("packages", StringComparison.OrdinalIgnoreCase))
                {
                    return directory.FullName;
                }

                directory = directory.Parent;
            }
            while (directory?.Parent != null);

            return Path.Combine(this.generatorFolder, "..", ".."); // assuming that SpecFlow was installed with nuget, generator is in the "tools" folder
        }
    }
}