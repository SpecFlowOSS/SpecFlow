using System;
using System.Linq;
using EnvDTE;
using TechTalk.SpecFlow.IdeIntegration.Generator;
using TechTalk.SpecFlow.IdeIntegration.Tracing;
using TechTalk.SpecFlow.Vs2010Integration.Utils;

namespace TechTalk.SpecFlow.Vs2010Integration
{
    internal class VsSpecFlowConfigurationReader : FileBasedSpecFlowConfigurationReader
    {
        private readonly Project project;

        public VsSpecFlowConfigurationReader(Project project, IIdeTracer tracer) : base(tracer)
        {
            this.project = project;
        }

        protected override string GetConfigFileContent()
        {
            ProjectItem projectItem = VsxHelper.FindProjectItemByProjectRelativePath(project, "app.config");
            if (projectItem == null)
                return null;

            return VsxHelper.GetFileContent(projectItem, true);
        }
    }
}