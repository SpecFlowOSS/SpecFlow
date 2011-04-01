using System;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.Interfaces;

namespace TechTalk.SpecFlow.IdeIntegration.Generator
{
    public class RemoteAppDomainTestGeneratorFactory : ITestGeneratorFactory, IDisposable
    {
        private readonly string generatorFolder;
        private AppDomain appDomain = null;
        private ITestGeneratorFactory remoteTestGeneratorFactory = null;
        private int generatorCounter = 0;

        public bool IsRunning
        {
            get { return appDomain != null; }
        }

        public RemoteAppDomainTestGeneratorFactory(string generatorFolder)
        {
            this.generatorFolder = generatorFolder;
        }

        private void EnsureInitialized()
        {
            if (!IsRunning)
                Initialize();
        }

        public void Initialize()
        {
            AppDomainSetup appDomainSetup = new AppDomainSetup();
            appDomainSetup.ApplicationBase = generatorFolder;
            appDomain = AppDomain.CreateDomain("AppDomainForTestGeneration", null, appDomainSetup);

            var remoteGeneratorAssemblyName = typeof(ITestGeneratorFactory).Assembly.GetName().Name;
            remoteTestGeneratorFactory = appDomain.CreateInstanceAndUnwrap(remoteGeneratorAssemblyName, typeof(TestGeneratorFactory).FullName) as ITestGeneratorFactory;

            if (remoteTestGeneratorFactory == null)
                throw new InvalidOperationException("Could not load test generator factory.");
        }

        public Version GetGeneratorVersion()
        {
            EnsureInitialized();
            return remoteTestGeneratorFactory.GetGeneratorVersion();
        }

        private class DisposeNotificationTestGenerator : ITestGenerator
        {
            private readonly ITestGenerator innerGenerator;
            public event Action Disposed;

            public DisposeNotificationTestGenerator(ITestGenerator innerGenerator)
            {
                this.innerGenerator = innerGenerator;
            }

            public void Dispose()
            {
                innerGenerator.Dispose();
                if (Disposed != null)
                {
                    Disposed();
                }
            }

            public TestGeneratorResult GenerateTestFile(FeatureFileInput featureFileInput, GenerationSettings settings)
            {
                return innerGenerator.GenerateTestFile(featureFileInput, settings);
            }

            public Version DetectGeneratedTestVersion(FeatureFileInput featureFileInput)
            {
                return innerGenerator.DetectGeneratedTestVersion(featureFileInput);
            }

            public string GetTestFullPath(FeatureFileInput featureFileInput)
            {
                return innerGenerator.GetTestFullPath(featureFileInput);
            }

            public override string ToString()
            {
                return innerGenerator.ToString();
            }
        }

        public ITestGenerator CreateGenerator(ProjectSettings projectSettings)
        {
            EnsureInitialized();
            generatorCounter++;
            var remoteGenerator = remoteTestGeneratorFactory.CreateGenerator(projectSettings);

            var disposeNotificationGenerator = new DisposeNotificationTestGenerator(remoteGenerator);
            disposeNotificationGenerator.Disposed +=
                () =>
                    {
                        if (--generatorCounter == 0)
                            Cleanup();
                    };
            return disposeNotificationGenerator;
        }

        public void Dispose()
        {
            Cleanup();
        }

        private void Cleanup()
        {
            if (!IsRunning)
                return;

            remoteTestGeneratorFactory = null;
            AppDomain.Unload(appDomain);
            appDomain = null;
        }
    }
}
