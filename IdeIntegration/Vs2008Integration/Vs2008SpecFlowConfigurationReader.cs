using System;
using System.Linq;
using EnvDTE;
using TechTalk.SpecFlow.IdeIntegration.Generator;
using TechTalk.SpecFlow.IdeIntegration.Tracing;

namespace TechTalk.SpecFlow.Vs2008Integration
{
    internal class Vs2008SpecFlowConfigurationReader : FileBasedSpecFlowConfigurationReader
    {
        private readonly Project project;

        public Vs2008SpecFlowConfigurationReader(Project project, IIdeTracer tracer) : base(tracer)
        {
            this.project = project;
        }

        protected override string GetConfigFilePath()
        {
            ProjectItem projectItem = VsxHelper.FindProjectItemByProjectRelativePath(project, "app.config");
            if (projectItem == null)
                return null;

            return VsxHelper.GetFileName(projectItem);
        }
    }
}