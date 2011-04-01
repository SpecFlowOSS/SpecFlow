using System;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.Interfaces;

namespace TechTalk.SpecFlow.IdeIntegration
{
    public interface IGeneratorServices
    {
        void InvalidateSettings();
        ITestGenerator CreateTestGenerator();
    }

    public abstract class GeneratorServices : IGeneratorServices
    {
        private readonly bool enableSettingsCache = true;
        private ProjectSettings projectSettings = null;

        protected GeneratorServices(bool enableSettingsCache)
        {
            this.enableSettingsCache = enableSettingsCache;
        }

        public void InvalidateSettings()
        {
            projectSettings = null;
        }

        protected abstract ProjectSettings GetProjectSettings();

        private ProjectSettings GetProjectSettingsCached()
        {
            if (!enableSettingsCache)
                return GetProjectSettings();

            var result = projectSettings;

            if (result == null)
                projectSettings = result = GetProjectSettings();

            return result;
        }
        
        private ITestGeneratorFactory GetTestGeneratorFactory()
        {
            //TODO: create it in new AppDomain if neccessary
            return new TestGeneratorFactory();
        }

        public ITestGenerator CreateTestGenerator()
        {
            var testGeneratorFactory = GetTestGeneratorFactory();

            return testGeneratorFactory.CreateGenerator(GetProjectSettingsCached());
        }
    }
}
