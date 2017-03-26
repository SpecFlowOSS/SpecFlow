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
    * // match first for the one with <specflow-version>
    * <nuget-plugin-folder> =
    *   <nuget-packages>\<plugin-name>.SpecFlowPlugin.<nuget-package-version> |
    *   <nuget-packages>\<plugin-name>.SpecFlow.<nuget-package-version> |
    *   <nuget-packages>\<plugin-name>.<nuget-package-version>
    * 
    * <specflow-version> = n-n[-n]  // e.g. 1-8-1
    * 
    * // assuming that SpecFlow was installed with nuget, go up n directory levels until we are in the packages directory.
    * <nuget-packages> = <generator-folder>[\..](n)\packages      
    * 
    * <nuget-package-version> = latest-of: n(.n)*[-tag]
    * 
    * <plugin-generator-folder> =
    *   <plugin-folder> |
    *   <plugin-folder>\tools\SpecFlowPlugin[.<specflow-version>] |
    *   <plugin-folder>\tools |
    *   <plugin-folder>\lib\net45 |
    *   <plugin-folder>\lib\net40 |
    *   <plugin-folder>\lib\net35 |
    *   <plugin-folder>\lib
    * 
    * <generator-plugin-assembly> =
    *   <plugin-generator-folder>\<plugin-name>.Generator.SpecFlowPlugin.dll |
    *   <plugin-generator-folder>\<plugin-name>.SpecFlowPlugin.dll
    */
    public class GeneratorPluginLocator : IGeneratorPluginLocator
    {
        private readonly string generatorFolder;
        private readonly bool isGlobalNuget;
        private readonly ProjectSettings projectSettings;
        private readonly IFileSystem fileSystem;
        private readonly IExecutingAssemblyInfo executingAssemblyInfo;

        public GeneratorPluginLocator(ProjectSettings projectSettings, IFileSystem fileSystem, IExecutingAssemblyInfo executingAssemblyInfo)
        {
            this.generatorFolder = Path.GetDirectoryName(new Uri(executingAssemblyInfo.GetCodeBase()).LocalPath);
            this.isGlobalNuget = IsUsingGlobalNuget();
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
            var nuGetPackagesFolder = GetNuGetPackagesFolder();

            return pluginPostfixes
                .Select(pluginPostfix => pluginDescriptor.Name + pluginPostfix)
                .Select(packageName => GetLatestPackage(nuGetPackagesFolder, packageName))
                .Where(pluginFolder => pluginFolder != null);
        }

        private string GetLatestPackage(string nuGetPackagesFolder, string packageName)
        {
            var specFlowVersion = this.executingAssemblyInfo.GetVersion();

            return this.isGlobalNuget
                ? this.GetLatestGlobalNugetPackage(nuGetPackagesFolder, packageName, specFlowVersion)
                : this.GetLatestLocalNugetPackage(nuGetPackagesFolder, packageName, specFlowVersion);
        }

        private string GetLatestLocalNugetPackage(string nuGetPackagesFolder, string packageName, Version specFlowVersion)
        {
            // First search for a package with the naming scheme "<package-name>.major-minor*"
            var searchPattern = packageName + "." + specFlowVersion.Major + "-" + +specFlowVersion.Minor + "*";

            var specificVersionPackage = this.fileSystem.GetDirectories(nuGetPackagesFolder, searchPattern).OrderByDescending(d => d).FirstOrDefault();
            if (specificVersionPackage != null)
            {
                return specificVersionPackage;
            }

            // Otherwise search for a non-specific version package with the naming scheme "<package-name>.*.*"
            var nonSpecificVersionPackage = this.fileSystem.GetDirectories(nuGetPackagesFolder, packageName + ".*.*").OrderByDescending(d => d).FirstOrDefault();
            if (nonSpecificVersionPackage != null && !nonSpecificVersionPackage.Contains("-"))
            {
                //TODO: This should be looking for more than just a dash because package names can have dashes in them.
                return nonSpecificVersionPackage;
            }

            return null;
        }

        private string GetLatestGlobalNugetPackage(string nuGetPackagesFolder, string packageName, Version specFlowVersion)
        {
            // First search for a package with the naming scheme "<package-name>.major-minor*"
            var searchPattern = packageName + "." + specFlowVersion.Major + "-" + +specFlowVersion.Minor + "*";
            var specificVersionPackages = this.fileSystem.GetDirectories(nuGetPackagesFolder, searchPattern).OrderByDescending(d => d).FirstOrDefault();
            if (specificVersionPackages != null)
            {
                var specificVersionPackage = this.fileSystem.GetDirectories(specificVersionPackages, "*").OrderByDescending(d => d).FirstOrDefault();

                if (specificVersionPackage != null)
                {
                    return specificVersionPackage;
                }
            }
            
            // Otherwise search for a non-specific version package with the naming scheme "<package-name>"
            var nonSpecificVersionPackages = this.fileSystem.GetDirectories(nuGetPackagesFolder, packageName).OrderByDescending(d => d).FirstOrDefault();
            if (nonSpecificVersionPackages != null && !nonSpecificVersionPackages.Contains("-"))
            {
                //TODO: This should be looking for more than just a dash because package names can have dashes in them.
                var nonSpecificVersionPackage = this.fileSystem.GetDirectories(nonSpecificVersionPackages, "*").OrderByDescending(d => d).FirstOrDefault();

                if (nonSpecificVersionPackage != null)
                {
                    return nonSpecificVersionPackage;
                }
            }

            // Else, we couldnt find a package...
            return null;
        }

        private IEnumerable<string> GetSpecFlowVersionSpecifiers()
        {
            var version = this.executingAssemblyInfo.GetVersion();
            yield return string.Format(@".{0}-{1}-{2}", version.Major, version.Minor, version.Revision);
            yield return string.Format(@".{0}-{1}", version.Major, version.Minor);
            yield return "";
        }

        private string GetNuGetPackagesFolder()
        {
            // assuming that SpecFlow was installed with nuget, generator is in the "tools" folder

            var directory = new DirectoryInfo(generatorFolder);

            while (!directory.Name.Equals("packages", StringComparison.OrdinalIgnoreCase))
            {
                directory = directory.Parent;
            }

            return directory.FullName;
        }

        private Boolean IsUsingGlobalNuget()
        {
            var directory = new DirectoryInfo(this.generatorFolder);
            var isGlobalNuget = false;

            while (!(isGlobalNuget = directory.Name.Equals(".nuget", StringComparison.OrdinalIgnoreCase)))
            {
                if (Path.GetPathRoot(this.generatorFolder) == directory.FullName)
                {
                    return false;
                }

                directory = directory.Parent;
            }

            return isGlobalNuget;
        }
    }
}
