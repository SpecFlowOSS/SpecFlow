using EnvDTE;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.IdeIntegration.Generator;
using TechTalk.SpecFlow.IdeIntegration.Tracing;

namespace TechTalk.SpecFlow.Vs2008Integration
{
    public class Vs2008GeneratorServices : GeneratorServices
    {
        private readonly Project project;

        public Vs2008GeneratorServices(Project project) : base(
            new TestGeneratorFactory(), NullIdeTracer.Instance, true)
        {
            this.project = project;
        }

        protected override ProjectSettings LoadProjectSettings()
        {
            ISpecFlowConfigurationReader configurationReader = new Vs2008SpecFlowConfigurationReader(project, tracer);

            var configurationHolder = configurationReader.ReadConfiguration();

            return new ProjectSettings
                       {
                           ProjectName = project.Name,
                           AssemblyName = project.Properties.Item("AssemblyName").Value as string,
                           ProjectFolder = VsxHelper.GetProjectFolder(project),
                           DefaultNamespace = project.Properties.Item("DefaultNamespace").Value as string,
                           ProjectPlatformSettings = new ProjectPlatformSettings(), // TODO: We only support C# for now, later we'll add support to grab the provider based on the project
                           ConfigurationHolder = configurationHolder
                       };
        }
    }
}