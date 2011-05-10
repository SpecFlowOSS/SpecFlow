using System;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using EnvDTE;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.IdeIntegration.Tracing;
using TechTalk.SpecFlow.Vs2010Integration.Utils;

namespace TechTalk.SpecFlow.Vs2010Integration
{
    internal class VsSpecFlowConfigurationReader : ISpecFlowConfigurationReader
    {
        private readonly Project project;
        private readonly IIdeTracer tracer;

        public VsSpecFlowConfigurationReader(Project project, IIdeTracer tracer)
        {
            this.project = project;
            this.tracer = tracer;
        }

        public SpecFlowConfigurationHolder ReadConfiguration()
        {
            ProjectItem projectItem = VsxHelper.FindProjectItemByProjectRelativePath(project, "app.config");
            if (projectItem == null)
                return new SpecFlowConfigurationHolder();

            string configFileContent = VsxHelper.GetFileContent(projectItem);
            return GetConfigurationHolderFromFileContent(configFileContent);
        }

        private SpecFlowConfigurationHolder GetConfigurationHolderFromFileContent(string configFileContent)
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
                tracer.Trace("Config load error: " + ex, "VsSpecFlowConfigurationReader");
                return new SpecFlowConfigurationHolder();
            }
        }
    }
}