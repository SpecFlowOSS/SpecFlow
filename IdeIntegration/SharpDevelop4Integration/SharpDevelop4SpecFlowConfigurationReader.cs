using System;
using System.IO;
using System.Linq;
using System.Xml;
using ICSharpCode.SharpDevelop.Project;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.IdeIntegration.Generator;
using TechTalk.SpecFlow.IdeIntegration.Tracing;

namespace TechTalk.SpecFlow.SharpDevelop4Integration
{
    internal class SharpDevelop4SpecFlowConfigurationReader : FileBasedSpecFlowConfigurationReader
    {
        private readonly IProject project;

        public SharpDevelop4SpecFlowConfigurationReader(IProject project, IIdeTracer tracer) : base(tracer)
        {
            this.project = project;
        }

        protected override string GetConfigFilePath()
        {
            ProjectItem projectItem = project.Items.OfType<FileProjectItem>().FirstOrDefault(fpi => "app.config".Equals(Path.GetFileName(fpi.FileName), StringComparison.InvariantCultureIgnoreCase));
            if (projectItem == null)
                return null;

            return projectItem.FileName;
        }
    }
}