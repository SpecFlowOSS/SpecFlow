using System;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.Interfaces;

namespace TechTalk.SpecFlow.IdeIntegration
{
    public interface IGeneratorServices
    {
        void InvalidateConfiguration();
        void InvalidateSettings();
        ITestGenerator CreateTestGenerator();
    }

    public abstract class GeneratorServices : IGeneratorServices
    {
        private readonly bool enableConfigurationCache = true;
        private readonly bool enableSettingsCache = true;
        private SpecFlowConfigurationHolder configurationHolder = null;
        private ProjectSettings projectSettings = null;

        protected GeneratorServices(bool enableConfigurationCache, bool enableSettingsCache)
        {
            this.enableConfigurationCache = enableConfigurationCache;
            this.enableSettingsCache = enableSettingsCache;
        }

        public void InvalidateConfiguration()
        {
            configurationHolder = null;
        }

        public void InvalidateSettings()
        {
            projectSettings = null;
        }

        protected abstract SpecFlowConfigurationHolder GetConfigurationHolder();
        protected abstract ProjectSettings GetProjectSettings();

        private SpecFlowConfigurationHolder GetConfigurationHolderCached()
        {
            if (!enableConfigurationCache)
                return GetConfigurationHolder();

            var result = configurationHolder;

            if (result == null)
                configurationHolder = result = GetConfigurationHolder();

            return result;
        }

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

            return testGeneratorFactory.CreateGenerator(GetConfigurationHolderCached(), GetProjectSettingsCached());
        }
    }
}
