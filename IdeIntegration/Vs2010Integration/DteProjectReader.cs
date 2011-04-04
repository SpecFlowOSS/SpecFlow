using System;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using EnvDTE;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.IdeIntegration.Generator;
using TechTalk.SpecFlow.Vs2010Integration.Generator;
using TechTalk.SpecFlow.Vs2010Integration.LanguageService;
using TechTalk.SpecFlow.Vs2010Integration.Tracing;
using TechTalk.SpecFlow.Vs2010Integration.Utils;
using VSLangProj;

namespace TechTalk.SpecFlow.Vs2010Integration
{
    internal class VsProjectReference : IProjectReference
    {
        public Project Project { get; private set; }

        public VsProjectReference(Project project)
        {
            Project = project;
        }

        static public VsProjectReference AssertVsProjectReference(IProjectReference projectReference)
        {
            return (VsProjectReference)projectReference; //TODO: better error handling
        }
    }

    internal class VsProjectScopeReference : VsProjectReference
    {
        public VsProjectScope VsProjectScope { get; set; }

        public VsProjectScopeReference(VsProjectScope vsProjectScope) : base(vsProjectScope.Project)
        {
            VsProjectScope = vsProjectScope;
        }

        static public VsProjectScopeReference AssertVsProjectScopeReference(IProjectReference projectReference)
        {
            return (VsProjectScopeReference)projectReference; //TODO: better error handling
        }
    }

    internal class VsSpecFlowProjectConfigurationLoader : SpecFlowProjectConfigurationLoader
    {
        protected override Version GetGeneratorVersion(IProjectReference projectReference)
        {
            var vsProjectScopeReference = VsProjectScopeReference.AssertVsProjectScopeReference(projectReference);

            //HACK: temporary solution: we use the SpecFlow runtime version as generator version, to avoid unwanted popups reporting outdated tests
            try
            {
                VSProject vsProject = (VSProject) vsProjectScopeReference.Project.Object;
                var specFlowRef =
                    vsProject.References.Cast<Reference>().FirstOrDefault(r => r.Name == "TechTalk.SpecFlow");
                if (specFlowRef != null)
                    return new Version(specFlowRef.Version);
            }
            catch(Exception exception)
            {
                Debug.WriteLine(exception, "VsSpecFlowProjectConfigurationLoader.GetGeneratorVersion");
            }

            return vsProjectScopeReference.VsProjectScope.GeneratorServices.GetGeneratorVersion();
        }
    }

    internal class VsSpecFlowConfigurationReader : ISpecFlowConfigurationReader
    {
        public SpecFlowConfigurationHolder ReadConfiguration(IProjectReference projectReference)
        {
            var vsProjectReference = VsProjectReference.AssertVsProjectReference(projectReference);
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