using System;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.IdeIntegration.Tracing;

namespace TechTalk.SpecFlow.IdeIntegration.Generator
{
    public abstract class GeneratorServices : IGeneratorServices
    {
        protected readonly ITestGeneratorFactory testGeneratorFactory;
        protected readonly IIdeTracer tracer;
        private readonly bool enableSettingsCache = true;
        private ProjectSettings projectSettings = null;

        protected GeneratorServices(ITestGeneratorFactory testGeneratorFactory, IIdeTracer tracer, bool enableSettingsCache)
        {
            this.testGeneratorFactory = testGeneratorFactory;
            this.tracer = tracer;
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
