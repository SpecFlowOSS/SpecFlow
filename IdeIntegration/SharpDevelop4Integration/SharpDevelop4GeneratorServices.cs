using ICSharpCode.SharpDevelop.Project;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.IdeIntegration.Generator;
using TechTalk.SpecFlow.IdeIntegration.Install;
using TechTalk.SpecFlow.IdeIntegration.Tracing;

namespace TechTalk.SpecFlow.SharpDevelop4Integration
{
    public class SharpDevelop4GeneratorServices : GeneratorServices
    {
        private readonly IProject project;
        private static bool installerServiceInitialized = false;

        public SharpDevelop4GeneratorServices(IProject project) : base(
            new TestGeneratorFactory(), NullIdeTracer.Instance, true)
        {
            this.project = project;

            if (!installerServiceInitialized)
            {
                InstallServices installerService =
                    new InstallServices(new ExternalBrowserGuidanceNotificationService(tracer), tracer,
                                        new WindowsFileAssociationDetector(tracer),
                                        new RegistryStatusAccessor(tracer));

                installerService.OnPackageLoad(IdeIntegration.Install.IdeIntegration.SharpDevelop);
                installerService.OnPackageUsed();
                installerServiceInitialized = true;
            }
        }

        protected override ProjectSettings LoadProjectSettings()
        {
            ISpecFlowConfigurationReader configurationReader = new SharpDevelop4SpecFlowConfigurationReader(project, tracer);

            var configurationHolder = configurationReader.ReadConfiguration();
            return new ProjectSettings
                       {
                           ProjectName = project.Name,
                           AssemblyName = project.AssemblyName,
                           ProjectFolder = project.Directory,
                           DefaultNamespace = project.RootNamespace,
                           ProjectPlatformSettings = new ProjectPlatformSettings(), //TODO: initialize based on project.Language?
                           ConfigurationHolder = configurationHolder
                       };
        }
    }
}