using System;
using System.Diagnostics;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.Interfaces;

namespace TechTalk.SpecFlow.IdeIntegration.Generator
{
    public class RemoteAppDomainTestGeneratorFactory : ITestGeneratorFactory, IDisposable
    {
        private readonly string generatorFolder;
        private AppDomain appDomain = null;
        private ITestGeneratorFactory remoteTestGeneratorFactory = null;
        private UsageCounter usageCounter;

        private class UsageCounter
        {
            private int counter = 0;
            private readonly Action cleanup;

            public UsageCounter(Action cleanup)
            {
                this.cleanup = cleanup;
            }

            public void Increase()
            {
                counter++;
            }

            public void Decrease()
            {
                if (--counter <= 0)
                    cleanup();
            }
        }

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
            AppDomainSetup appDomainSetup = new AppDomainSetup { ApplicationBase = generatorFolder };
            appDomain = AppDomain.CreateDomain("AppDomainForTestGeneration", null, appDomainSetup);

            var remoteGeneratorAssemblyName = typeof(ITestGeneratorFactory).Assembly.GetName().Name;
            var testGeneratorFactoryTypeFullName = typeof(TestGeneratorFactory).FullName;
            Debug.Assert(testGeneratorFactoryTypeFullName != null);

            remoteTestGeneratorFactory = appDomain.CreateInstanceAndUnwrap(remoteGeneratorAssemblyName, testGeneratorFactoryTypeFullName) as ITestGeneratorFactory;

            if (remoteTestGeneratorFactory == null)
                throw new InvalidOperationException("Could not load test generator factory.");

            usageCounter = new UsageCounter(Cleanup);
        }

        public Version GetGeneratorVersion()
        {
            EnsureInitialized();
            try
            {
                usageCounter.Increase();
                return remoteTestGeneratorFactory.GetGeneratorVersion();
            }
            finally
            {
                usageCounter.Decrease();
            }
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
            usageCounter.Increase();
            var remoteGenerator = remoteTestGeneratorFactory.CreateGenerator(projectSettings);

            var disposeNotificationGenerator = new DisposeNotificationTestGenerator(remoteGenerator);
            disposeNotificationGenerator.Disposed += () => usageCounter.Decrease();
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
