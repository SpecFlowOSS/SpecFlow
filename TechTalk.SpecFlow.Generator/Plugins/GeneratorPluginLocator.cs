using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.Plugins;

namespace TechTalk.SpecFlow.Generator.Plugins
{
    public interface IGeneratorPluginLocator
    {
        IEnumerable<string> GetGeneratorPluginAssemblies(PluginDescriptor pluginDescriptor);
    }

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
    public class GeneratorPluginLocator : IGeneratorPluginLocator
    {
        private readonly string generatorFolder;
        private readonly ProjectSettings projectSettings;
        private readonly IFileSystem fileSystem;
        private readonly IExecutingAssemblyInfo executingAssemblyInfo;

        public GeneratorPluginLocator(ProjectSettings projectSettings, IFileSystem fileSystem, IExecutingAssemblyInfo executingAssemblyInfo)
        {
            this.generatorFolder = Path.GetDirectoryName(new Uri(executingAssemblyInfo.GetCodeBase()).LocalPath);
            this.projectSettings = projectSettings;
            this.fileSystem = fileSystem;
            this.executingAssemblyInfo = executingAssemblyInfo;
        }

        public IEnumerable<string> GetGeneratorPluginAssemblies(PluginDescriptor pluginDescriptor)
        {
            foreach (var pluginGeneratorFolder in GetPluginGeneratorFolders(pluginDescriptor))
            {
                string generatorSpecificAssembly = Path.GetFullPath(Path.Combine(pluginGeneratorFolder, string.Format("{0}.Generator.SpecFlowPlugin.dll", pluginDescriptor.Name)));
                if (this.fileSystem.FileExists(generatorSpecificAssembly))
                    yield return generatorSpecificAssembly;

                string genericAssembly = Path.GetFullPath(Path.Combine(pluginGeneratorFolder, string.Format("{0}.SpecFlowPlugin.dll", pluginDescriptor.Name)));
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
            var nuGetPackagesFolders = GetNuGetPackagesFolders();

            return pluginPostfixes
                .Select(pluginPostfix => pluginDescriptor.Name + pluginPostfix)
                .SelectMany(packageName => nuGetPackagesFolders.Select(nuGetPackagesFolder => GetLatestPackage(nuGetPackagesFolder, packageName)))
                .Where(pluginFolder => pluginFolder != null);
        }

        private string GetLatestPackage(string nuGetPackagesFolder, string packageName)
        {
            return this.fileSystem.GetDirectories(nuGetPackagesFolder, packageName + ".*").OrderByDescending(d => d).FirstOrDefault();
        }

        private IEnumerable<string> GetSpecFlowVersionSpecifiers()
        {
            var version = this.executingAssemblyInfo.GetVersion();
            yield return string.Format(".{0}-{1}-{2}", version.Major, version.Minor, version.Revision);
            yield return string.Format(".{0}-{1}", version.Major, version.Minor);
            yield return "";
        }

        private IEnumerable<string> GetNuGetPackagesFolders()
        {
            // assuming that SpecFlow was installed with nuget, generator is in the "tools" folder



            // When packages are stored in the solution directory, they are stored in the following format:
            // /packages/[PackageName].[PackageVersion]/
            yield return Path.Combine(generatorFolder, @"..\..");

            // When packages are stored in the global nuget directory, they are stored in the following format:
            // /packages/[PackageName]/[PackageVersion]/
            yield return Path.Combine(generatorFolder, @"..\..\..");
        }
    }
}
