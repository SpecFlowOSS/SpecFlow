using System;
using System.IO;
using System.Linq;
using MonoDevelop.Projects;
using TechTalk.SpecFlow.IdeIntegration.Generator;
using TechTalk.SpecFlow.IdeIntegration.Tracing;

namespace MonoDevelop.TechTalk.SpecFlow
{
    internal class MonoDevelopSpecFlowConfigurationReader : FileBasedSpecFlowConfigurationReader
    {
        private readonly Project project;

        public MonoDevelopSpecFlowConfigurationReader(Project project, IIdeTracer tracer) : base(tracer)
        {
            this.project = project;
        }

        protected override string GetConfigFilePath()
        {
            ProjectFile projectItem = project.Files.FirstOrDefault(fpi => "app.config".Equals(Path.GetFileName(fpi.FilePath), StringComparison.InvariantCultureIgnoreCase));
            if (projectItem == null)
                return null;

            return projectItem.FilePath;
        }
    }
}