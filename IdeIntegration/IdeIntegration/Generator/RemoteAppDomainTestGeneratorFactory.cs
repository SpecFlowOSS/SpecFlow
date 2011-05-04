using System;
using System.Diagnostics;
using System.Reflection;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.IdeIntegration.Tracing;

namespace TechTalk.SpecFlow.IdeIntegration.Generator
{
    public class RemoteAppDomainTestGeneratorFactory : IRemoteAppDomainTestGeneratorFactory
    {
        private readonly IIdeTracer tracer;

        private string generatorFolder;
        private AppDomain appDomain = null;
        private ITestGeneratorFactory remoteTestGeneratorFactory = null;
        private UsageCounter usageCounter;
        private readonly string remoteGeneratorAssemblyName;

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

        public RemoteAppDomainTestGeneratorFactory(IIdeTracer tracer)
        {
            this.tracer = tracer;
            this.remoteGeneratorAssemblyName = typeof(ITestGeneratorFactory).Assembly.GetName().Name;
        }

        public void Setup(string newGeneratorFolder)
        {
            if (generatorFolder == newGeneratorFolder)
                return;

            Cleanup();
            generatorFolder = newGeneratorFolder;
        }

        public void EnsureInitialized()
        {
            if (!IsRunning)
                Initialize();
        }

        private void Initialize()
        {
            if (generatorFolder == null)
                throw new InvalidOperationException("The RemoteAppDomainTestGeneratorFactory has to be configured with the Setup() method before initialization.");

            AppDomainSetup appDomainSetup = new AppDomainSetup { ApplicationBase = generatorFolder };
            appDomain = AppDomain.CreateDomain("AppDomainForTestGeneration", null, appDomainSetup);

            var testGeneratorFactoryTypeFullName = typeof(TestGeneratorFactory).FullName;
            Debug.Assert(testGeneratorFactoryTypeFullName != null);

            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainOnAssemblyResolve;

            var generatorFactoryObject = appDomain.CreateInstanceAndUnwrap(remoteGeneratorAssemblyName, testGeneratorFactoryTypeFullName);
            remoteTestGeneratorFactory = generatorFactoryObject as ITestGeneratorFactory;

            if (remoteTestGeneratorFactory == null)
                throw new InvalidOperationException("Could not load test generator factory.");

            usageCounter = new UsageCounter(Cleanup);
            tracer.Trace("AppDomain for generator created", "RemoteAppDomainTestGeneratorFactory");
        }

        private Assembly CurrentDomainOnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            string assemblyName = args.Name.Split(new[] {','}, 2)[0];
            if (assemblyName.Equals(remoteGeneratorAssemblyName, StringComparison.InvariantCultureIgnoreCase))
            {
                return typeof (ITestGeneratorFactory).Assembly;
            }
            return null;
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

        public void Cleanup()
        {
            if (!IsRunning)
                return;

            AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomainOnAssemblyResolve;
            remoteTestGeneratorFactory = null;
            AppDomain.Unload(appDomain);
            appDomain = null;
            tracer.Trace("AppDomain for generator disposed", "RemoteAppDomainTestGeneratorFactory");
        }
    }
}
