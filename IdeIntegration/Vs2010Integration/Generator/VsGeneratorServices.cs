using System;
using EnvDTE;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.IdeIntegration;
using TechTalk.SpecFlow.Vs2010Integration.LanguageService;
using TechTalk.SpecFlow.Vs2010Integration.Utils;

namespace TechTalk.SpecFlow.Vs2010Integration.Generator
{
    internal class VsGeneratorServices : GeneratorServices
    {
        private readonly Project project;

        public VsGeneratorServices(Project project) : base(true, false)
        {
            this.project = project;
        }

        protected override SpecFlowConfigurationHolder GetConfigurationHolder()
        {
            return DteProjectReader.LoadConfigHolderFromProject(project);
        }

        protected override ProjectSettings GetProjectSettings()
        {
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

            return new ProjectSettings(VsxHelper.GetProjectFolder(project), VsxHelper.GetProjectDefaultNamespace(project), projectPlatformSettings);
        }
    }
}
