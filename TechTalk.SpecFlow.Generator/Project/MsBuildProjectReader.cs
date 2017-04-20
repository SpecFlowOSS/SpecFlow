using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using TechTalk.SpecFlow.Assist.ValueRetrievers;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Generator.Interfaces;

namespace TechTalk.SpecFlow.Generator.Project
{
    public class MsBuildProjectReader : ISpecFlowProjectReader
    {
        private readonly IGeneratorConfigurationProvider _configurationLoader;

        public MsBuildProjectReader(IGeneratorConfigurationProvider configurationLoader)
        {
            _configurationLoader = configurationLoader;
        }

        public static SpecFlowProject LoadSpecFlowProjectFromMsBuild(string projectFilePath)
        {
            return new MsBuildProjectReader(new GeneratorConfigurationProvider()).ReadSpecFlowProject(projectFilePath);
        }

        public SpecFlowProject ReadSpecFlowProject(string projectFilePath)
        {
            string projectFolder = Path.GetDirectoryName(projectFilePath);

            using (var filestream = new FileStream(projectFilePath, FileMode.Open))
            {
                var xDocument = XDocument.Load(filestream);


                var specFlowProject = new SpecFlowProject();
                specFlowProject.ProjectSettings.ProjectFolder = projectFolder;
                specFlowProject.ProjectSettings.ProjectName = Path.GetFileNameWithoutExtension(projectFilePath);
                specFlowProject.ProjectSettings.DefaultNamespace = xDocument.Descendants().Where(n => n.Name.LocalName == "RootNamespace").SingleOrDefault()?.Value;
                specFlowProject.ProjectSettings.AssemblyName = xDocument.Descendants().Where(n => n.Name.LocalName == "AssemblyName").SingleOrDefault()?.Value;

                specFlowProject.ProjectSettings.ProjectPlatformSettings.Language = GetLanguage(xDocument);

                foreach (var item in GetFeatureFiles(xDocument))
                {
                    specFlowProject.FeatureFiles.Add(item);
                }

                var appConfigFile = GetAppConfigFile(xDocument);
                if (!String.IsNullOrWhiteSpace(appConfigFile))
                {
                    var configFilePath = Path.Combine(projectFolder, appConfigFile);
                    var configFileContent = File.ReadAllText(configFilePath);
                    var configurationHolder = GetConfigurationHolderFromFileContent(configFileContent);
                    specFlowProject.ProjectSettings.ConfigurationHolder = configurationHolder;
                    specFlowProject.Configuration = _configurationLoader.LoadConfiguration(configurationHolder);
                }

                return specFlowProject;
            }
        }

        private string GetAppConfigFile(XDocument xDocument)
        {
            var nodesWhereFeatureFilesCouldBe = GetNotCompileableNodes(xDocument);

            foreach (var xElement in nodesWhereFeatureFilesCouldBe)
            {
                var include = xElement.Attribute("Include")?.Value;
                if (String.IsNullOrWhiteSpace(include))
                {
                    continue;
                }

                if (Path.GetFileName(include).Equals("app.config", StringComparison.InvariantCultureIgnoreCase))
                {
                    return include;
                }
            }

            return null;
        }

        private IEnumerable<FeatureFileInput> GetFeatureFiles(XDocument xDocument)
        {
            var nodesWhereFeatureFilesCouldBe = GetNotCompileableNodes(xDocument);

            foreach (var xElement in nodesWhereFeatureFilesCouldBe)
            {
                var include = xElement.Attribute("Include")?.Value;

                if (String.IsNullOrWhiteSpace(include))
                {
                    continue;
                }

                if (include.EndsWith(".feature", StringComparison.InvariantCultureIgnoreCase) ||
                    include.EndsWith(".feature.xslx", StringComparison.InvariantCultureIgnoreCase))
                {
                    var featureFile = new FeatureFileInput(include);

                    var customNamespace = xElement.Descendants(GetNameWithNamespace("CustomToolNamespace")).SingleOrDefault();
                    if (customNamespace != null)
                    {
                        featureFile.CustomNamespace = customNamespace.Value;
                    }

                    yield return featureFile;
                }
            }
        }

        private IEnumerable<XElement> GetNotCompileableNodes(XDocument xDocument)
        {
            var contentNodes = xDocument.Descendants(GetNameWithNamespace("Content"));
            var noneNodes = xDocument.Descendants(GetNameWithNamespace("None"));

            var nodesWhereFeatureFilesCouldBe = contentNodes.Union(noneNodes);
            return nodesWhereFeatureFilesCouldBe;
        }

        private XName GetNameWithNamespace(string localName)
        {
            return XName.Get(localName, "http://schemas.microsoft.com/developer/msbuild/2003");
        }

        private string GetLanguage(XDocument project)
        {
            var imports = project.Descendants(GetNameWithNamespace("Import")).ToList();

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

        private static SpecFlowConfigurationHolder GetConfigurationHolderFromFileContent(string configFileContent)
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