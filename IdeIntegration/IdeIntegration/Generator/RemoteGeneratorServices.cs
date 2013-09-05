using System;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.IdeIntegration.Tracing;

namespace TechTalk.SpecFlow.IdeIntegration.Generator
{
    public abstract class RemoteGeneratorServices : GeneratorServices
    {
        private readonly IRemoteAppDomainTestGeneratorFactory remoteAppDomainTestGeneratorFactory;
        private readonly IGeneratorInfoProvider generatorInfoProvider;

        protected RemoteGeneratorServices(ITestGeneratorFactory testGeneratorFactory, IRemoteAppDomainTestGeneratorFactory remoteAppDomainTestGeneratorFactory, IGeneratorInfoProvider generatorInfoProvider, IIdeTracer tracer, bool enableSettingsCache)
            : base(testGeneratorFactory, tracer, enableSettingsCache)
        {
            this.remoteAppDomainTestGeneratorFactory = remoteAppDomainTestGeneratorFactory;
            this.generatorInfoProvider = generatorInfoProvider;
        }

        protected virtual GeneratorInfo GetGeneratorInfo()
        {
            return generatorInfoProvider.GetGeneratorInfo();
        }

        protected Version GetCurrentGeneratorAssemblyVersion()
        {
            return typeof(TestGeneratorFactory).Assembly.GetName().Version;
        }

        protected override ITestGeneratorFactory GetTestGeneratorFactoryForCreate()
        {
            var generatorInfo = GetGeneratorInfo();
            if (generatorInfo == null || generatorInfo.GeneratorAssemblyVersion == null || generatorInfo.GeneratorFolder == null)
            {
                // we don't know about the generator -> call the "current" directly
                tracer.Trace("Unable to detect generator location: the generator bound to the IDE is used", "RemoteGeneratorServices");
                return GetTestGeneratorFactoryOfIDE();
            }

            var currentGeneratorAssemblyVersion = GetCurrentGeneratorAssemblyVersion();

            if (generatorInfo.GeneratorAssemblyVersion < new Version(1, 6) && 
                generatorInfo.GeneratorAssemblyVersion != currentGeneratorAssemblyVersion) // in debug mode 1.0 is the version, that is < 1.6
            {
                // old generator version -> call the "current" directly
                tracer.Trace(string.Format("The project's generator ({0}) is older than v1.6: the generator bound to the IDE is used", generatorInfo.GeneratorAssemblyVersion), "RemoteGeneratorServices");
                return GetTestGeneratorFactoryOfIDE();
            }

            if (generatorInfo.GeneratorAssemblyVersion == currentGeneratorAssemblyVersion && !generatorInfo.UsesPlugins)
            {
                // uses the "current" generator (and no plugins) -> call it directly
                tracer.Trace("The generator of the project is the same as the generator bound to the IDE: using it from the IDE", "RemoteGeneratorServices");
                return GetTestGeneratorFactoryOfIDE();
            }

            try
            {
                tracer.Trace(string.Format("Creating remote wrapper for the project's generator ({0} at {1})", generatorInfo.GeneratorAssemblyVersion, generatorInfo.GeneratorFolder), "RemoteGeneratorServices");
                remoteAppDomainTestGeneratorFactory.Setup(generatorInfo.GeneratorFolder);
                remoteAppDomainTestGeneratorFactory.EnsureInitialized();
                return remoteAppDomainTestGeneratorFactory;
            }
            catch(Exception exception)
            {
                tracer.Trace(exception.ToString(), "RemoteGeneratorServices");
                // there was an error -> call the "current" directly (plus cleanup)
                Cleanup();
                return base.GetTestGeneratorFactoryForCreate();
            }
        }

        public override void InvalidateSettings()
        {
            Cleanup();

            base.InvalidateSettings();
        }

        public override void Dispose()
        {
            Cleanup();
        }

        private void Cleanup()
        {
            remoteAppDomainTestGeneratorFactory.Cleanup();
        }
    }
}