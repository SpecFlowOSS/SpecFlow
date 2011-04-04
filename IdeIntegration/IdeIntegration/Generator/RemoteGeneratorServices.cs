using System;
using System.Diagnostics;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.Interfaces;

namespace TechTalk.SpecFlow.IdeIntegration.Generator
{
    public class GeneratorInfo
    {
        public Version GeneratorAssemblyVersion { get; set; }
        public string GeneratorFolder { get; set; }
    }

    public abstract class RemoteGeneratorServices : GeneratorServices
    {
        private RemoteAppDomainTestGeneratorFactory remoteAppDomainTestGeneratorFactory;

        protected RemoteGeneratorServices(ITestGeneratorFactory testGeneratorFactory, bool enableSettingsCache) : base(testGeneratorFactory, enableSettingsCache)
        {
        }

        protected abstract GeneratorInfo GetGeneratorInfo();

        protected Version GetCurrentGeneratorAssemblyVersion()
        {
            return typeof(TestGeneratorFactory).Assembly.GetName().Version;
        }

        protected override ITestGeneratorFactory GetTestGeneratorFactoryForCreate()
        {
            // if we already have a generator factory -> use it!
            if (remoteAppDomainTestGeneratorFactory != null)
                return remoteAppDomainTestGeneratorFactory;

            GeneratorInfo generatorInfo = GetGeneratorInfo();
            if (generatorInfo == null || generatorInfo.GeneratorAssemblyVersion == null || generatorInfo.GeneratorFolder == null)
            {
                // we don't know about the generator -> call the "current" directly
                return base.GetTestGeneratorFactoryForCreate();
            }

            if (generatorInfo.GeneratorAssemblyVersion == GetCurrentGeneratorAssemblyVersion())
            {
                // uses the "current" generator -> call it directly
                return base.GetTestGeneratorFactoryForCreate();
            }

            try
            {
                remoteAppDomainTestGeneratorFactory = new RemoteAppDomainTestGeneratorFactory(generatorInfo.GeneratorFolder);
                remoteAppDomainTestGeneratorFactory.Initialize();
                return remoteAppDomainTestGeneratorFactory;
            }
            catch(Exception exception)
            {
                Debug.WriteLine(exception, "RemoteGeneratorServices.GetTestGeneratorFactoryForCreate");
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
            if (remoteAppDomainTestGeneratorFactory != null)
            {
                remoteAppDomainTestGeneratorFactory.Dispose();
                remoteAppDomainTestGeneratorFactory = null;
            }
        }
    }
}