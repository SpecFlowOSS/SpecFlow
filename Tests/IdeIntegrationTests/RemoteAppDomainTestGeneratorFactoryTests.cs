using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Moq;
using NUnit.Framework;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.IdeIntegration;
using Should;
using TechTalk.SpecFlow.IdeIntegration.Generator;
using TechTalk.SpecFlow.IdeIntegration.Tracing;
using TechTalk.SpecFlow.Utils;

namespace IdeIntegrationTests
{
    [TestFixture]
    public class RemoteAppDomainTestGeneratorFactoryTests
    {
        private string currentGeneratorFolder;
        private Mock<IIdeTracer> tracerStub;

        [SetUp]
        public void Setup()
        {
            currentGeneratorFolder = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
            tracerStub = new Mock<IIdeTracer>();
        }

        private RemoteAppDomainTestGeneratorFactory CreateRemoteAppDomainTestGeneratorFactory()
        {
            return CreateRemoteAppDomainTestGeneratorFactory(currentGeneratorFolder);
        }

        private RemoteAppDomainTestGeneratorFactory CreateRemoteAppDomainTestGeneratorFactory(string generatorFolder)
        {
            var factory = new RemoteAppDomainTestGeneratorFactory(tracerStub.Object);
            factory.Setup(generatorFolder);
            return factory;
        }

        [Test]
        public void Should_be_able_to_initialize()
        {
            using (var remoteFactory = CreateRemoteAppDomainTestGeneratorFactory())
            {
                remoteFactory.EnsureInitialized();
                remoteFactory.IsRunning.ShouldBeTrue();
            }
        }

        [Test]
        public void Should_be_able_to_return_generator_version()
        {
            using (var remoteFactory = CreateRemoteAppDomainTestGeneratorFactory())
            {
                var version = remoteFactory.GetGeneratorVersion();

                version.ShouldNotBeNull();
                version.ShouldEqual(TestGeneratorFactory.GeneratorVersion);
            }
        }

        [Test]
        public void Should_be_able_to_create_generator_with_default_config()
        {
            using (var remoteFactory = CreateRemoteAppDomainTestGeneratorFactory())
            {
                var generator = remoteFactory.CreateGenerator(new ProjectSettings());

                generator.ShouldNotBeNull();
            }
        }

        [Serializable]
        private class DummyGenerator : MarshalByRefObject, ITestGenerator
        {
            public TestGeneratorResult GenerateTestFile(FeatureFileInput featureFileInput, GenerationSettings settings)
            {
                throw new NotImplementedException();
            }

            public Version DetectGeneratedTestVersion(FeatureFileInput featureFileInput)
            {
                throw new NotImplementedException();
            }

            public string GetTestFullPath(FeatureFileInput featureFileInput)
            {
                throw new NotImplementedException();
            }

            public void Dispose()
            {
                //nop;
            }

            public override string ToString()
            {
                return "DummyGenerator";
            }
        }

        [Test]
        public void Should_create_custom_generator_when_configured_so()
        {
            var configurationHolder = new SpecFlowConfigurationHolder(string.Format(@"
                <specFlow>
                  <generator>
                  <dependencies>
                    <register type=""{0}"" as=""{1}""/>
                  </dependencies>
                  </generator>
                </specFlow>",
                typeof(DummyGenerator).AssemblyQualifiedName,
                typeof(ITestGenerator).AssemblyQualifiedName));

            var projectSettings = new ProjectSettings();
            projectSettings.ConfigurationHolder = configurationHolder;

            using (var remoteFactory = CreateRemoteAppDomainTestGeneratorFactory())
            {
                var generator = remoteFactory.CreateGenerator(projectSettings);
                generator.ToString().ShouldEqual("DummyGenerator"); // since the type is wrapped, we can only check it this way
            }
        }

        [Test]
        public void Should_be_able_to_load_generator_from_another_folder()
        {
            using(var tempFolder = new TempFolder())
            {
                var runtimeAssemblyFile = typeof(BindingAttribute).Assembly.Location;
                var generatorAssemblyFile = typeof(TestGeneratorFactory).Assembly.Location;
                var utilsAssemblyFile = typeof(FileSystemHelper).Assembly.Location;
                FileSystemHelper.CopyFileToFolder(runtimeAssemblyFile, tempFolder.FolderName);
                FileSystemHelper.CopyFileToFolder(generatorAssemblyFile, tempFolder.FolderName);
                FileSystemHelper.CopyFileToFolder(utilsAssemblyFile, tempFolder.FolderName);

                using (var remoteFactory = CreateRemoteAppDomainTestGeneratorFactory(tempFolder.FolderName))
                {
                    var generator = remoteFactory.CreateGenerator(new ProjectSettings());
                    generator.ShouldNotBeNull();
                }
            }
        }

        [Test]
        public void Should_cleanup_ater_dispose()
        {
            RemoteAppDomainTestGeneratorFactory remoteFactory;
            using (remoteFactory = CreateRemoteAppDomainTestGeneratorFactory())
            {
                remoteFactory.EnsureInitialized();
            }

            remoteFactory.IsRunning.ShouldBeFalse();
        }

        [Test]
        public void Should_start_running_delayed()
        {
            RemoteAppDomainTestGeneratorFactory remoteFactory;
            using (remoteFactory = CreateRemoteAppDomainTestGeneratorFactory())
            {
                remoteFactory.IsRunning.ShouldBeFalse();
            }
        }

        [Test]
        public void Should_cleanup_after_generator_disposed_when_timeout_ellapses()
        {
            using (var remoteFactory = CreateRemoteAppDomainTestGeneratorFactory())
            {
                remoteFactory.AppDomainCleanupTime = TimeSpan.FromSeconds(1);

                var generator = remoteFactory.CreateGenerator(new ProjectSettings());
                generator.Dispose();

                Thread.Sleep(TimeSpan.FromSeconds(1.1));

                remoteFactory.IsRunning.ShouldBeFalse();
            }
        }

        [Test]
        public void Should_not_cleanup_after_generator_disposed_immediately()
        {
            using (var remoteFactory = CreateRemoteAppDomainTestGeneratorFactory())
            {
                var generator = remoteFactory.CreateGenerator(new ProjectSettings());
                generator.Dispose();

                remoteFactory.IsRunning.ShouldBeTrue();
            }
        }

        [Test]
        public void Should_not_cleanup_when_one_generator_is_still_used()
        {
            using (var remoteFactory = CreateRemoteAppDomainTestGeneratorFactory())
            {
                var generator1 = remoteFactory.CreateGenerator(new ProjectSettings());
                var generator2 = remoteFactory.CreateGenerator(new ProjectSettings());
                generator1.Dispose();

                remoteFactory.IsRunning.ShouldBeTrue();
            }
        }

        [Test]
        public void Should_cleanup_when_all_generators_are_disposed()
        {
            using (var remoteFactory = CreateRemoteAppDomainTestGeneratorFactory())
            {
                remoteFactory.AppDomainCleanupTime = TimeSpan.FromSeconds(1);

                var generator1 = remoteFactory.CreateGenerator(new ProjectSettings());
                var generator2 = remoteFactory.CreateGenerator(new ProjectSettings());
                generator1.Dispose();
                generator2.Dispose();

                Thread.Sleep(TimeSpan.FromSeconds(1.1));

                remoteFactory.IsRunning.ShouldBeFalse();
            }
        }


        [Test]
        public void Should_be_able_generate_from_a_simple_valid_feature()
        {
            using (var remoteFactory = CreateRemoteAppDomainTestGeneratorFactory())
            {
                var generator = remoteFactory.CreateGenerator(new ProjectSettings()
                                                                  {
                                                                      ProjectFolder = Path.GetTempPath()
                                                                  });

                FeatureFileInput featureFileInput = new FeatureFileInput("Test.feature")
                                                        {
                                                            FeatureFileContent = @"
Feature: Addition

@mytag
Scenario: Add two numbers
	Given I have entered 50 into the calculator
	And I have entered 70 into the calculator
	When I press add
	Then the result should be 120 on the screen
"
                                                        };
                var result = generator.GenerateTestFile(featureFileInput, new GenerationSettings());

                result.ShouldNotBeNull();
                result.Success.ShouldBeTrue();
                result.GeneratedTestCode.ShouldNotBeNull();
            }
        }

        [Test]
        public void Should_be_able_generate_from_a_simple_invalid_feature()
        {
            using (var remoteFactory = CreateRemoteAppDomainTestGeneratorFactory())
            {
                var generator = remoteFactory.CreateGenerator(new ProjectSettings()
                                                                  {
                                                                      ProjectFolder = Path.GetTempPath()
                                                                  });

                FeatureFileInput featureFileInput = new FeatureFileInput("Test.feature")
                                                        {
                                                            FeatureFileContent = @"
Feature: Addition
Scenario: Add two numbers
	Given I have entered 50 into the calculator
    AndXXX the keyword is misspelled
"
                                                        };
                var result = generator.GenerateTestFile(featureFileInput, new GenerationSettings());

                result.ShouldNotBeNull();
                result.Success.ShouldBeFalse();
                result.Errors.ShouldNotBeNull();
                result.Errors.ShouldNotBeEmpty();
            }
        }
    }
}
