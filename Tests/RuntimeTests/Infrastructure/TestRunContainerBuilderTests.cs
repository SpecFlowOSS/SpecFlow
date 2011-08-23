using System;
using System.Diagnostics;
using System.Reflection;
using System.Xml;
using NUnit.Framework;
using TechTalk.SpecFlow.Configuration;
using Should;
using TechTalk.SpecFlow.Infrastructure;

namespace TechTalk.SpecFlow.RuntimeTests.Infrastructure
{
    [TestFixture]
    public class TestRunContainerBuilderTests
    {
        [Test]
        public void Should_create_a_container()
        {
            var container = TestRunContainerBuilder.CreateContainer();
            container.ShouldNotBeNull();
        }

        [Test]
        public void Should_register_runtime_configuration_with_default_config()
        {
            var container = TestRunContainerBuilder.CreateContainer();
            container.Resolve<RuntimeConfiguration>().ShouldNotBeNull();
        }

        private class DummyTestRunnerFactory : ITestRunnerFactory
        {
            public ITestRunner Create(Assembly testAssembly)
            {
                throw new NotImplementedException();
            }
        }

        private class StringConfigProvider : IRuntimeConfigurationProvider
        {
            private readonly string configFileContent;

            public StringConfigProvider(string configContent)
            {
                this.configFileContent = configContent;
            }

            public RuntimeConfiguration GetConfiguration()
            {
                XmlDocument configDocument = new XmlDocument();
                configDocument.LoadXml(configFileContent);

                var specFlowNode = configDocument.SelectSingleNode("/configuration/specFlow");
                if (specFlowNode == null)
                    throw new InvalidOperationException("invalid config file content");

                var section = ConfigurationSectionHandler.CreateFromXml(specFlowNode);
                return RuntimeConfiguration.LoadFromConfigFile(section);
            }
        }

        [Test]
        public void Should_be_able_to_customize_dependencies_from_config()
        {
            var configurationHolder = new StringConfigProvider(string.Format(@"<?xml version=""1.0"" encoding=""utf-8"" ?>
              <configuration>
                <specFlow>
                  <runtime>  
                    <dependencies>
                      <register type=""{0}"" as=""{1}""/>
                    </dependencies>
                  </runtime>
                </specFlow>
              </configuration>",
                typeof(DummyTestRunnerFactory).AssemblyQualifiedName,
                typeof(ITestRunnerFactory).AssemblyQualifiedName));

            var container = TestRunContainerBuilder.CreateContainer(configurationHolder);
            var testRunnerFactory = container.Resolve<ITestRunnerFactory>();
            testRunnerFactory.ShouldBeType(typeof(DummyTestRunnerFactory));
        }
    }
}
