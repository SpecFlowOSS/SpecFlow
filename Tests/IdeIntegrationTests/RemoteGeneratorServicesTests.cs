using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.IdeIntegration.Generator;
using TechTalk.SpecFlow.IdeIntegration.Tracing;
using Should;

namespace IdeIntegrationTests
{
    internal class RemoteGeneratorServicesMock : RemoteGeneratorServices
    {
        private readonly Func<GeneratorInfo> getGeneratorInfo;

        public RemoteGeneratorServicesMock(ITestGeneratorFactory testGeneratorFactory, IRemoteAppDomainTestGeneratorFactory remoteAppDomainTestGeneratorFactory, Func<GeneratorInfo> getGeneratorInfo)
            : base(testGeneratorFactory, remoteAppDomainTestGeneratorFactory, new Mock<IGeneratorInfoProvider>().Object, new Mock<IIdeTracer>().Object, false)
        {
            this.getGeneratorInfo = getGeneratorInfo;
        }

        protected override ProjectSettings LoadProjectSettings()
        {
            return new ProjectSettings();
        }

        protected override GeneratorInfo GetGeneratorInfo()
        {
            return getGeneratorInfo();
        }

        public ITestGeneratorFactory GetTestGeneratorFactoryForCreatePublic()
        {
            return GetTestGeneratorFactoryForCreate();
        }
    }

    [TestFixture]
    public class RemoteGeneratorServicesTests
    {
        Mock<ITestGeneratorFactory> TestGeneratorFactoryStub;
        Mock<IRemoteAppDomainTestGeneratorFactory> RemoteTestGeneratorFactoryMock;
        public Version v15 = new Version(1, 5);
        public Version v16 = new Version(1, 6);
        public Version v17 = new Version(1, 7);
        public string SampleGeneratorFolder = @"C:\SampleGeneratorFolder";

        [SetUp]
        public void Setup()
        {
            TestGeneratorFactoryStub = new Mock<ITestGeneratorFactory>();
            RemoteTestGeneratorFactoryMock = new Mock<IRemoteAppDomainTestGeneratorFactory>();
        }

        private RemoteGeneratorServicesMock CreateRemoteGeneratorServices(Func<GeneratorInfo> getGeneratorInfo)
        {
            return new RemoteGeneratorServicesMock(TestGeneratorFactoryStub.Object, RemoteTestGeneratorFactoryMock.Object, getGeneratorInfo);
        }

        [Test]
        public void Should_use_default_generator_when_no_generator_info_detected()
        {
            var generatorServices = CreateRemoteGeneratorServices(() => null);
            generatorServices.GetTestGeneratorFactoryForCreatePublic().ShouldEqual(TestGeneratorFactoryStub.Object);
        }

        [Test]
        public void Should_use_default_generator_when_no_generator_version_detected()
        {
            var generatorServices = CreateRemoteGeneratorServices(() => new GeneratorInfo { GeneratorAssemblyVersion = null, GeneratorFolder = SampleGeneratorFolder});
            generatorServices.GetTestGeneratorFactoryForCreatePublic().ShouldEqual(TestGeneratorFactoryStub.Object);
        }

        [Test]
        public void Should_use_default_generator_when_no_generator_folder_detected()
        {
            var generatorServices = CreateRemoteGeneratorServices(() => new GeneratorInfo {GeneratorAssemblyVersion = v16, GeneratorFolder = null});
            generatorServices.GetTestGeneratorFactoryForCreatePublic().ShouldEqual(TestGeneratorFactoryStub.Object);
        }

        [Test]
        public void Should_use_default_generator_when_generator_is_older_than_16()
        {
            var generatorServices = CreateRemoteGeneratorServices(() => new GeneratorInfo { GeneratorAssemblyVersion = v15, GeneratorFolder = SampleGeneratorFolder });
            generatorServices.GetTestGeneratorFactoryForCreatePublic().ShouldEqual(TestGeneratorFactoryStub.Object);
        }

        [Test]
        public void Should_use_default_generator_when_generator_is_the_current_one_and_not_using_plugins()
        {
            var currentGeneratorVersion = typeof(TestGeneratorFactory).Assembly.GetName().Version;
            var generatorServices = CreateRemoteGeneratorServices(() => new GeneratorInfo
                                                                            {
                                                                                GeneratorAssemblyVersion = currentGeneratorVersion, 
                                                                                GeneratorFolder = SampleGeneratorFolder,
                                                                                UsesPlugins = false
                                                                            });
            generatorServices.GetTestGeneratorFactoryForCreatePublic().ShouldEqual(TestGeneratorFactoryStub.Object);
        }

        [Test]
        public void Should_use_remote_factory_when_generator_is_the_current_one_and_uses_plugins()
        {
            var currentGeneratorVersion = typeof(TestGeneratorFactory).Assembly.GetName().Version;
            var generatorServices = CreateRemoteGeneratorServices(() => new GeneratorInfo
            {
                GeneratorAssemblyVersion = currentGeneratorVersion,
                GeneratorFolder = SampleGeneratorFolder,
                UsesPlugins = true
            });
            generatorServices.GetTestGeneratorFactoryForCreatePublic().ShouldEqual(RemoteTestGeneratorFactoryMock.Object);
        }

        [Test]
        public void Should_use_remote_factory_when_is_at_least_v16_and_not_current()
        {
            var generatorServices = CreateRemoteGeneratorServices(() => new GeneratorInfo { GeneratorAssemblyVersion = v17, GeneratorFolder = SampleGeneratorFolder });
            generatorServices.GetTestGeneratorFactoryForCreatePublic().ShouldEqual(RemoteTestGeneratorFactoryMock.Object);
        }

        [Test]
        public void Should_setup_and_initialize_remote_factory()
        {
            var generatorServices = CreateRemoteGeneratorServices(() => new GeneratorInfo { GeneratorAssemblyVersion = v17, GeneratorFolder = SampleGeneratorFolder });
            generatorServices.GetTestGeneratorFactoryForCreatePublic();

            RemoteTestGeneratorFactoryMock.Verify(m => m.Setup(SampleGeneratorFolder));
            RemoteTestGeneratorFactoryMock.Verify(m => m.EnsureInitialized());
        }

        [Test]
        public void Should_cleanup_remote_factory_when_invalidating_settings()
        {
            var generatorServices = CreateRemoteGeneratorServices(() => new GeneratorInfo { GeneratorAssemblyVersion = v17, GeneratorFolder = SampleGeneratorFolder });

            generatorServices.InvalidateSettings();
            RemoteTestGeneratorFactoryMock.Verify(m => m.Cleanup());
        }

        [Test]
        public void Should_cleanup_remote_factory_when_disposed()
        {
            var generatorServices = CreateRemoteGeneratorServices(() => new GeneratorInfo { GeneratorAssemblyVersion = v17, GeneratorFolder = SampleGeneratorFolder });

            generatorServices.Dispose();
            RemoteTestGeneratorFactoryMock.Verify(m => m.Cleanup());
        }
    }
}
