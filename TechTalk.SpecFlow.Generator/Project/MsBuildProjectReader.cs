using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.Generator.Project
{
    public class MsBuildProjectReader : ISpecFlowProjectReader
    {
        private readonly IGeneratorConfigurationProvider _configurationLoader;
        private readonly IMSBuildRelativePathParser _msBuildRelativePathParser;

        public MsBuildProjectReader(IGeneratorConfigurationProvider configurationLoader, IMSBuildRelativePathParser msBuildRelativePathParser)
        {
            _configurationLoader = configurationLoader;
            _msBuildRelativePathParser = msBuildRelativePathParser;
        }

        public SpecFlowProject ReadSpecFlowProject(string projectFilePath)
        {
            var projectFolder = Path.GetDirectoryName(projectFilePath);

            using (var filestream = new FileStream(projectFilePath, FileMode.Open))
            {
                var xDocument = XDocument.Load(filestream);

                var newProjectSystem = IsNewProjectSystem(xDocument);

                var specFlowProject = new SpecFlowProject();
                specFlowProject = LoadProjectSettings(specFlowProject, xDocument, projectFolder, projectFilePath, newProjectSystem);
                specFlowProject = LoadFeatureFiles(specFlowProject, xDocument, projectFolder, newProjectSystem);
                specFlowProject = LoadAppConfig(specFlowProject, xDocument, projectFolder, newProjectSystem);

                return specFlowProject;
            }
        }

        private SpecFlowProject LoadProjectSettings(SpecFlowProject specFlowProject, XDocument xDocument, string projectFolder, string projectFilePath,
            bool newProjectSystem)
        {
            var projectSettings = specFlowProject.ProjectSettings;

            projectSettings.ProjectFolder = projectFolder;
            projectSettings.ProjectName = Path.GetFileNameWithoutExtension(projectFilePath);
            projectSettings.DefaultNamespace = GetMsBuildProperty(xDocument, newProjectSystem, "RootNamespace");
            projectSettings.AssemblyName = GetMsBuildProperty(xDocument, newProjectSystem, "AssemblyName");

            if (newProjectSystem)
            {
                if (string.IsNullOrWhiteSpace(projectSettings.AssemblyName))
                {
                    projectSettings.AssemblyName = projectSettings.ProjectName;
                }

                if (string.IsNullOrWhiteSpace(projectSettings.DefaultNamespace))
                {
                    projectSettings.DefaultNamespace = projectSettings.ProjectName;
                }
            }

            projectSettings.ProjectPlatformSettings.Language = GetLanguage(xDocument, newProjectSystem);

            return specFlowProject;
        }

        private string GetMsBuildProperty(XDocument xDocument, bool newProjectSystem, string propertyName)
        {
            return xDocument.Descendants(GetNameWithNamespace(propertyName, newProjectSystem)).SingleOrDefault()?.Value;
        }

        private SpecFlowProject LoadAppConfig(SpecFlowProject specFlowProject, XDocument xDocument, string projectFolder, bool newProjectSystem)
        {
            var appConfigFile = GetAppConfigFile(xDocument, newProjectSystem, projectFolder);

            SpecFlowConfigurationHolder configurationHolder = null;
            if (!string.IsNullOrWhiteSpace(appConfigFile))
            {
                var configFilePath = Path.Combine(projectFolder, appConfigFile);
                var configFileContent = File.ReadAllText(configFilePath);
                configurationHolder = GetConfigurationHolderFromFileContent(configFileContent);
            }



            var jsonConfigFile = GetJsonConfigFile(xDocument, newProjectSystem, projectFolder);
            if (!string.IsNullOrWhiteSpace(jsonConfigFile))
            {
                var configFilePath = Path.Combine(projectFolder, jsonConfigFile);
                var configFileContent = File.ReadAllText(configFilePath);
                configurationHolder = new SpecFlowConfigurationHolder(ConfigSource.Json, configFileContent);
            }

            if (configurationHolder != null)
            {
                specFlowProject.ProjectSettings.ConfigurationHolder = configurationHolder;
                specFlowProject.Configuration = _configurationLoader.LoadConfiguration(configurationHolder);
            }

            return specFlowProject;
        }

        private string GetJsonConfigFile(XDocument xDocument, bool newProjectSystem, string projectFolder)
        {
            return FindFile(xDocument, newProjectSystem, projectFolder, "specflow.json");
        }

        private SpecFlowProject LoadFeatureFiles(SpecFlowProject specFlowProject, XDocument xDocument, string projectFolder, bool newProjectSystem)
        {
            foreach (var item in GetFeatureFiles(xDocument, newProjectSystem, projectFolder))
            {
                specFlowProject.FeatureFiles.Add(item);
            }

            return specFlowProject;
        }

        private bool IsNewProjectSystem(XDocument xDocument)
        {
            return xDocument.Root.Attribute("Sdk") != null;
        }

        public static SpecFlowProject LoadSpecFlowProjectFromMsBuild(string projectFilePath)
        {
            return new MsBuildProjectReader(new GeneratorConfigurationProvider(new ConfigurationLoader()), new MSBuildRelativePathParser()).ReadSpecFlowProject(projectFilePath);
        }

        private string GetAppConfigFile(XDocument xDocument, bool newProjectSystem, string projectFolder)
        {
            return FindFile(xDocument, newProjectSystem, projectFolder, "app.config");
        }

        private string FindFile(XDocument xDocument, bool newProjectSystem, string projectFolder, string fileName)
        {
            var nodesWhereFeatureFilesCouldBe = GetNotCompileableNodes(xDocument, newProjectSystem);

            foreach (var xElement in nodesWhereFeatureFilesCouldBe)
            {
                var include = xElement.Attribute("Include")?.Value;
                if (string.IsNullOrWhiteSpace(include))
                {
                    continue;
                }

                if (Path.GetFileName(include).Equals(fileName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return include;
                }
            }

            if (newProjectSystem)
            {
                var appConfigFilePath = Path.Combine(projectFolder, fileName);

                if (File.Exists(appConfigFilePath))
                {
                    return appConfigFilePath;
                }
            }

            return null;
        }

        private IEnumerable<FeatureFileInput> GetFeatureFiles(XDocument xDocument, bool newProjectSystem, string projectFolder)
        {
            var nodesWhereFeatureFilesCouldBe = GetNotCompileableNodes(xDocument, newProjectSystem);

            var result = new List<FeatureFileInput>();

            foreach (var xElement in nodesWhereFeatureFilesCouldBe)
            {
                var include = xElement.Attribute("Include")?.Value;
                var update = xElement.Attribute("Update")?.Value;

                if (string.IsNullOrWhiteSpace(include) && string.IsNullOrWhiteSpace(update))
                {
                    continue;
                }

                var fileName = string.IsNullOrWhiteSpace(update) ? include : update;

                if (IsAFeatureFile(fileName))
                {
                    var customNamespace = xElement.Descendants(GetNameWithNamespace("CustomToolNamespace", newProjectSystem)).SingleOrDefault();
                  
                    if (fileName.Contains("*"))
                    {
                        var files = _msBuildRelativePathParser.GetFiles(projectFolder, fileName);

                        result.AddRange(files.Select(file => CreateFeatureFileInput(file, customNamespace)));
                    }
                    else
                    {
                        result.Add(CreateFeatureFileInput(fileName, customNamespace));
                    }
                }
            }

            if (newProjectSystem)
            {
                var allFilesInProjectFolder = Directory.EnumerateFiles(projectFolder, "*", SearchOption.AllDirectories);

                foreach (var file in allFilesInProjectFolder)
                {
                    if (IsAFeatureFile(Path.GetFileName(file)))
                    {
                        if (result.Any(i => file.EndsWith(i.ProjectRelativePath)))
                        {
                            continue;
                        }

                        var realtivePath = file.Replace(projectFolder, "").Trim('\\');

                        result.Add(new FeatureFileInput(realtivePath));
                    }
                }
            }

            return result;
        }

        private FeatureFileInput CreateFeatureFileInput(string file, XElement customNamespace)
        {
            var featureFile = new FeatureFileInput(file);

            if (customNamespace != null)
            {
                featureFile.CustomNamespace = customNamespace.Value;
            }
            return featureFile;
        }

        private bool IsAFeatureFile(string fileName)
        {
            return fileName.EndsWith(".feature", StringComparison.InvariantCultureIgnoreCase) ||
                   fileName.EndsWith(".feature.xlsx", StringComparison.InvariantCultureIgnoreCase);
        }

        private IEnumerable<XElement> GetNotCompileableNodes(XDocument xDocument, bool newProjectSystem)
        {
            var contentNodes = xDocument.Descendants(GetNameWithNamespace("Content", newProjectSystem));
            var noneNodes = xDocument.Descendants(GetNameWithNamespace("None", newProjectSystem));

            var nodesWhereFeatureFilesCouldBe = contentNodes.Union(noneNodes);
            return nodesWhereFeatureFilesCouldBe;
        }

        private XName GetNameWithNamespace(string localName, bool newProjectSystem)
        {
            if (newProjectSystem)
            {
                return XName.Get(localName);
            }

            return XName.Get(localName, "http://schemas.microsoft.com/developer/msbuild/2003");
        }

        private string GetLanguage(XDocument project, bool newProjectSystem)
        {
            var imports = project.Descendants(GetNameWithNamespace("Import", newProjectSystem)).ToList();

            if (imports.Any(n => n.Attribute("Project")?.Value.Contains("Microsoft.CSharp.targets") ?? false))
            {
                return GenerationTargetLanguage.CSharp;
            }

            if (imports.Any(n => n.Attribute("Project")?.Value.Contains("Microsoft.VisualBasic.targets") ?? false))
            {
                return GenerationTargetLanguage.VB;
            }

            return GenerationTargetLanguage.CSharp;
        }

        private SpecFlowConfigurationHolder GetConfigurationHolderFromFileContent(string configFileContent)
        {
            try
            {
                var configDocument = new XmlDocument();
                configDocument.LoadXml(configFileContent);

                return new SpecFlowConfigurationHolder(configDocument.SelectSingleNode("/configuration/specFlow"));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex, "Config load error");
                return new SpecFlowConfigurationHolder();
            }
        }
    }
}