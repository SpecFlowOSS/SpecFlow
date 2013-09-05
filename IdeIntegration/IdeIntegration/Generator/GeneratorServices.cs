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

        public ProjectSettings GetProjectSettings()
        {
            return GetProjectSettingsCached();
        }

        protected abstract ProjectSettings LoadProjectSettings();

        private ProjectSettings GetProjectSettingsCached()
        {
            if (!enableSettingsCache)
                return LoadProjectSettings();

            var result = projectSettings;

            if (result == null)
                projectSettings = result = LoadProjectSettings();

            return result;
        }
        
        protected virtual ITestGeneratorFactory GetTestGeneratorFactoryForCreate()
        {
            return GetTestGeneratorFactoryOfIDE(); 
        }

        protected ITestGeneratorFactory GetTestGeneratorFactoryOfIDE()
        {
            return testGeneratorFactory;
        }

        public ITestGenerator CreateTestGenerator()
        {
            var testGeneratorFactoryForCreate = GetTestGeneratorFactoryForCreate();
            return testGeneratorFactoryForCreate.CreateGenerator(GetProjectSettingsCached());
        }

        public ITestGenerator CreateTestGeneratorOfIDE()
        {
            var testGeneratorFactoryForCreate = GetTestGeneratorFactoryOfIDE();
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
