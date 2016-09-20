using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Generator.Interfaces;

namespace TechTalk.SpecFlow.Generator.Project
{
    public class DirectoryProjectReader
    {
        public static SpecFlowProject LoadSpecFlowProject(string directoryName)
        {
            var settings = TryLoadSpecflowSettings(directoryName);
            var projectFolder = new DirectoryInfo(directoryName);

            //TODO: Need a cleaner way to express this.
            var featureLanguage = settings.ContainsKey("featureLanguage") ? CultureInfo.GetCultureInfo(settings["featureLanguage"].ToString()) : CultureInfo.GetCultureInfo("en-us");
            var toolLanguage = settings.ContainsKey("toolLanguage") ? CultureInfo.GetCultureInfo(settings["toolLanguage"].ToString()) : CultureInfo.GetCultureInfo("en-us");
            var generatorUnitTestProvider = settings.ContainsKey("unittestProvider") ? settings["unittestProvider"].ToString() : "NUnit";
            var defaultNamespace = settings.ContainsKey("defaultNamespace") ? settings["defaultNamespace"].ToString() : projectFolder.Name;

            var project = new SpecFlowProject()
            {
                Configuration = new SpecFlowProjectConfiguration()
                {
                    GeneratorConfiguration = new GeneratorConfiguration()
                    {
                        FeatureLanguage = featureLanguage,
                        GeneratorUnitTestProvider = generatorUnitTestProvider,
                        ToolLanguage = toolLanguage,
                        AllowDebugGeneratedFiles = true,
                        AllowRowTests = true
                    }
                },
                ProjectSettings = new ProjectSettings()
                {
                    ProjectFolder = directoryName,
                    ProjectName = projectFolder.Name,
                    AssemblyName = projectFolder.Name,
                    DefaultNamespace = defaultNamespace,

                    //TODO: plugins aren't supported because the XML is ignored.
                    ConfigurationHolder = new SpecFlowConfigurationHolder(string.Empty)
                }
            };

            var files = projectFolder.GetFiles("*.feature", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                project.FeatureFiles.Add(new FeatureFileInput(RelativeFilePath(projectFolder.FullName, file.FullName)));
            }

            return project;
        }

        private static string RelativeFilePath(string rootFolder, string filePath)
        {
            string fullFilePath = Path.GetFullPath(filePath);
            string fullFolderPath = Path.GetFullPath(rootFolder);

            if (!fullFilePath.StartsWith(fullFolderPath))
            {
                return fullFilePath;
            }

            string relativePath = fullFilePath.Substring(fullFolderPath.Length);

            if (relativePath[0] == Path.DirectorySeparatorChar)
            {
                relativePath = relativePath.Substring(1);
            }

            return relativePath;
        }
        
        private static Dictionary<string, object> TryLoadSpecflowSettings(string directoryName)
        {
            var configurationFileName = Path.Combine(directoryName, "specflow.json");

            if (File.Exists(configurationFileName))
            {
                return JsonConvert.DeserializeObject<Dictionary<string, object>>(
                    File.ReadAllText(configurationFileName));
            }

            return new Dictionary<string, object>();
        }
    }
}