using System;
using System.IO;
using EnvDTE;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.IdeIntegration.Generator;
using TechTalk.SpecFlow.IdeIntegration.Tracing;
using TechTalk.SpecFlow.Vs2010Integration.LanguageService;
using TechTalk.SpecFlow.Vs2010Integration.Utils;

namespace TechTalk.SpecFlow.Vs2010Integration.Generator
{
    internal class VsGeneratorServices : RemoteGeneratorServices
    {
        protected readonly Project project;
        private readonly ISpecFlowConfigurationReader configurationReader;

        public VsGeneratorServices(Project project, ISpecFlowConfigurationReader configurationReader, IIdeTracer tracer) : base( //TODO: load dependencies through DI
            new TestGeneratorFactory(), 
            new RemoteAppDomainTestGeneratorFactory(tracer), 
            new VsGeneratorInfoProvider(project, tracer, configurationReader), 
            tracer, false)
        {
            this.project = project;
            this.configurationReader = configurationReader;
        }

        protected override ProjectSettings LoadProjectSettings()
        {
            tracer.Trace("Discover project settings", "VsGeneratorServices");

            ProjectPlatformSettings projectPlatformSettings;
            var tergetLanguage = VsProjectScope.GetTargetLanguage(project);
            switch (tergetLanguage)
            {
                case ProgrammingLanguage.CSharp:
                    projectPlatformSettings = new ProjectPlatformSettings
                    {
                        Language = GenerationTargetLanguage.CSharp,
                        LanguageVersion = new Version("3.0"),
                        Platform = GenerationTargetPlatform.DotNet,
                        PlatformVersion = new Version("3.5"),
                    };
                    break;
                case ProgrammingLanguage.VB:
                    projectPlatformSettings = new ProjectPlatformSettings
                    {
                        Language = GenerationTargetLanguage.VB,
                        LanguageVersion = new Version("9.0"),
                        Platform = GenerationTargetPlatform.DotNet,
                        PlatformVersion = new Version("3.5"),
                    };
                    break;
                default:
                    throw new NotSupportedException("target language not supported");
            }

            var configurationHolder = configurationReader.ReadConfiguration();
            return new ProjectSettings
                       {
                           ProjectName = Path.GetFileNameWithoutExtension(project.FullName),
                           AssemblyName = VsxHelper.GetProjectAssemblyName(project),
                           ProjectFolder = VsxHelper.GetProjectFolder(project),
                           DefaultNamespace = VsxHelper.GetProjectDefaultNamespace(project),
                           ProjectPlatformSettings = projectPlatformSettings,
                           ConfigurationHolder = configurationHolder
                       };
        }
    }
}
