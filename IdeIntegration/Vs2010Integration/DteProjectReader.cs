using System;
using System.IO;
using System.Linq;
using System.Text;
using EnvDTE;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Vs2010Integration.LanguageService;

namespace TechTalk.SpecFlow.Vs2010Integration
{
    internal class DteProjectReader
    {
        public static SpecFlowProject LoadSpecFlowProjectFromDteProject(Project project)
        {
            if (project == null || !SpecFlowServices.IsProjectSupported(project))
                return null;

            try
            {
                return LoadSpecFlowProjectFromDteProjectInternal(project);
            }
            catch
            {
                return null;
            }
        }

        public static SpecFlowProjectConfiguration LoadSpecFlowConfigurationFromDteProject(Project project)
        {
            if (project == null || !SpecFlowServices.IsProjectSupported(project))
                return null;

            try
            {
                return LoadSpecFlowConfigurationFromDteProjectInternal(project);
            }
            catch
            {
                return null;
            }
        }

        private static SpecFlowProject LoadSpecFlowProjectFromDteProjectInternal(Project project)
        {
            string projectFolder = Path.GetDirectoryName(project.FullName);

            SpecFlowProject specFlowProject = new SpecFlowProject();
            specFlowProject.ProjectFolder = projectFolder;
            specFlowProject.ProjectName = Path.GetFileNameWithoutExtension(project.FullName);
            specFlowProject.AssemblyName = project.Properties.Item("AssemblyName").Value as string;
            specFlowProject.DefaultNamespace = project.Properties.Item("DefaultNamespace").Value as string;

            foreach (ProjectItem projectItem in VsxHelper.GetAllPhysicalFileProjectItem(project))
            {
                if (".feature".Equals(Path.GetExtension(projectItem.Name), StringComparison.InvariantCultureIgnoreCase))
                {
                    var featureFile = new SpecFlowFeatureFile(VsxHelper.GetProjectRelativePath(projectItem));
                    var ns = projectItem.Properties.Item("CustomToolNamespace").Value as string;
                    if (!String.IsNullOrEmpty(ns))
                        featureFile.CustomNamespace = ns;
                    specFlowProject.FeatureFiles.Add(featureFile);
                }
            }

            specFlowProject.Configuration = LoadSpecFlowConfigurationFromDteProjectInternal(project);

            return specFlowProject;
        }

        private static SpecFlowProjectConfiguration LoadSpecFlowConfigurationFromDteProjectInternal(Project project)
        {
            SpecFlowProjectConfiguration configuration = new SpecFlowProjectConfiguration();
            ProjectItem projectItem = VsxHelper.FindProjectItemByProjectRelativePath(project, "app.config");
            if (projectItem != null)
            {
                string configFileContent = VsxHelper.GetFileContent(projectItem);
                GeneratorConfigurationReader.UpdateConfigFromFileContent(configuration.GeneratorConfiguration, configFileContent);
                RuntimeConfigurationReader.UpdateConfigFromFileContent(configuration.RuntimeConfiguration, configFileContent);
            }

            //TODO: read generator version from the referenced assembly version

            return configuration;
        }

        private static string DumpProperties(ProjectItem projectItem)
        {
            StringBuilder result = new StringBuilder();
            foreach (Property property in projectItem.Properties)
            {
                result.AppendLine(property.Name + "=" + property.Value);
            }
            return result.ToString();
        }

        private static string DumpProperties(Project project)
        {
            StringBuilder result = new StringBuilder();
            foreach (Property property in project.Properties)
            {
                try
                {
                    result.AppendLine(property.Name + "=" + property.Value);
                }
                catch (Exception) { }
            }
            return result.ToString();
        }
    }
}