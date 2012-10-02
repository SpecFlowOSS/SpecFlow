using System;
using System.Diagnostics;
using EnvDTE;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.IdeIntegration.Generator;
using TechTalk.SpecFlow.IdeIntegration.Install;
using TechTalk.SpecFlow.IdeIntegration.Tracing;
using TechTalk.SpecFlow.Vs2010Integration.Install;

namespace TechTalk.SpecFlow.Vs2008Integration
{
    public class Vs2008GeneratorServices : GeneratorServices
    {
        private readonly Project project;
        private static bool installerServiceInitialized = false;

        public Vs2008GeneratorServices(Project project) : base(
            new TestGeneratorFactory(), NullIdeTracer.Instance, true)
        {
            this.project = project;

            if (!installerServiceInitialized)
            {
                InstallServices installerService =
                    new InstallServices(new VsBrowserGuidanceNotificationService(project.DTE), tracer,
                                        new WindowsFileAssociationDetector(tracer),
                                        new RegistryStatusAccessor(tracer));

                installerService.OnPackageLoad(IdeIntegration.Install.IdeIntegration.VisualStudio2008);
                installerService.OnPackageUsed();
                installerServiceInitialized = true;
            }
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
                           ProjectPlatformSettings = ProjectPlatformSettings(),
                           ConfigurationHolder = configurationHolder
                       };
        }

        private ProjectPlatformSettings ProjectPlatformSettings()
        {
            var projectPlatformSettings = new ProjectPlatformSettings();
            try
            {
                if (project.FullName.EndsWith(".vbproj"))
                {
                    projectPlatformSettings.Language = GenerationTargetLanguage.VB;
                    projectPlatformSettings.LanguageVersion = new Version("9.0");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex, "ProjectPlatformSettings");
            }
            return projectPlatformSettings;
        }
    }
}