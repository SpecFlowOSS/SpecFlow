using MonoDevelop.Projects;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.IdeIntegration.Generator;
using TechTalk.SpecFlow.IdeIntegration.Tracing;

namespace MonoDevelop.TechTalk.SpecFlow
{
    public class MonoDevelopGeneratorServices : GeneratorServices
    {
        private readonly Project project;

        public MonoDevelopGeneratorServices(Project project) : base(
            new TestGeneratorFactory(), NullIdeTracer.Instance, true)
        {
            this.project = project;
        }

        protected override ProjectSettings LoadProjectSettings()
        {
            ISpecFlowConfigurationReader configurationReader = new MonoDevelopSpecFlowConfigurationReader(project, tracer);

            var configurationHolder = configurationReader.ReadConfiguration();

            // No way to get AssemblyName right now, therefore we'll just use DefaultNamespace
			string defaultNamespace = "Namespace";
			if (project is DotNetProject)
			{
				defaultNamespace = ((DotNetProject)project).GetDefaultNamespace(project.Name);
			}

            return new ProjectSettings
                       {
                           ProjectName = project.Name,
                           AssemblyName = defaultNamespace,
                           ProjectFolder = project.BaseDirectory,
                           DefaultNamespace = defaultNamespace,
                           ProjectPlatformSettings = new ProjectPlatformSettings(), // TODO: We only support C# for now, later we'll add support to grab the provider based on the project
                           ConfigurationHolder = configurationHolder
                       };
        }
    }
}