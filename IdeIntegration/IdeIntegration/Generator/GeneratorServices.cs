using System;
using TechTalk.SpecFlow.Generator.Interfaces;

namespace TechTalk.SpecFlow.IdeIntegration.Generator
{
    public interface IGeneratorServices : IDisposable
    {
        void InvalidateSettings();
        ITestGenerator CreateTestGenerator();
        Version GetGeneratorVersion();
    }

    public abstract class GeneratorServices : IGeneratorServices
    {
        private readonly ITestGeneratorFactory testGeneratorFactory;
        private readonly bool enableSettingsCache = true;
        private ProjectSettings projectSettings = null;

        protected GeneratorServices(ITestGeneratorFactory testGeneratorFactory, bool enableSettingsCache)
        {
            this.testGeneratorFactory = testGeneratorFactory;
            this.enableSettingsCache = enableSettingsCache;
        }

        public virtual void InvalidateSettings()
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
        
        protected virtual ITestGeneratorFactory GetTestGeneratorFactoryForCreate()
        {
            //TODO: create it in new AppDomain if neccessary
            return testGeneratorFactory; 
        }

        public ITestGenerator CreateTestGenerator()
        {
            var testGeneratorFactoryForCreate = GetTestGeneratorFactoryForCreate();
            return testGeneratorFactoryForCreate.CreateGenerator(GetProjectSettingsCached());
        }

        public virtual Version GetGeneratorVersion()
        {
            var testGeneratorFactoryForCreate = GetTestGeneratorFactoryForCreate();
            return testGeneratorFactoryForCreate.GetGeneratorVersion();
        }

        public virtual void Dispose()
        {
            //nop
        }
    }
}
