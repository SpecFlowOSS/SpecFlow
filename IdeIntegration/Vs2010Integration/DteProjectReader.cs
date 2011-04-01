using System;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using EnvDTE;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.Vs2010Integration.Utils;

namespace TechTalk.SpecFlow.Vs2010Integration
{
    internal class VsProjectReference : IProjectReference
    {
        public Project Project { get; private set; }

        public VsProjectReference(Project project)
        {
            Project = project;
        }

        static public VsProjectReference AssertFileProjectReference(IProjectReference projectReference)
        {
            return (VsProjectReference)projectReference; //TODO: better error handling
        }
    }

    internal class VsSpecFlowProjectConfigurationLoader : SpecFlowProjectConfigurationLoader
    {
        protected override Version GetGeneratorVersion(IProjectReference projectReference)
        {
            VsProjectReference.AssertFileProjectReference(projectReference);
            //TODO: read generator version from the referenced assembly version
            return TestGeneratorFactory.GeneratorVersion; 
        }
    }

    internal class VsSpecFlowConfigurationReader : ISpecFlowConfigurationReader
    {
        public SpecFlowConfigurationHolder ReadConfiguration(IProjectReference projectReference)
        {
            var vsProjectReference = VsProjectReference.AssertFileProjectReference(projectReference);
            ProjectItem projectItem = VsxHelper.FindProjectItemByProjectRelativePath(vsProjectReference.Project, "app.config");
            if (projectItem == null)
                return new SpecFlowConfigurationHolder();

            string configFileContent = VsxHelper.GetFileContent(projectItem);
            return GetConfigurationHolderFromFileContent(configFileContent);
        }

        private static SpecFlowConfigurationHolder GetConfigurationHolderFromFileContent(string configFileContent)
        {
            XmlDocument configDocument;
            try
            {
                configDocument = new XmlDocument();
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