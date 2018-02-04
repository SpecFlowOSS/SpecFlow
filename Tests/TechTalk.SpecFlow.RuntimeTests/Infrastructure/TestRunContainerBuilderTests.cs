using System;
using System.Diagnostics;
using System.Reflection;
using NUnit.Framework;
using TechTalk.SpecFlow.Configuration;
using FluentAssertions;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.UnitTestProvider;

namespace TechTalk.SpecFlow.RuntimeTests.Infrastructure
{
    [TestFixture]
    public class TestRunContainerBuilderTests
    {
        [Test]
        public void Should_create_a_container()
        {
            var container = TestObjectFactories.CreateDefaultGlobalContainer();
            container.Should().NotBeNull();
        }

        [Test]
        public void Should_register_runtime_configuration_with_default_config()
        {
            var container = TestObjectFactories.CreateDefaultGlobalContainer();
            container.Resolve<SpecFlow.Configuration.SpecFlowConfiguration>().Should().NotBeNull();
        }

        private class DummyTestRunnerFactory : ITestRunnerFactory
        {
            public ITestRunner Create(Assembly testAssembly)
            {
                throw new NotImplementedException();
            }
        }

        #region Fix: ReSharper test runner assemby loading issue workaround
        [SetUp]
        public void Setup()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainOnAssemblyResolve;
        }

        [TearDown]
        public void TearDown()
        {
            AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomainOnAssemblyResolve;
        }

        private Assembly CurrentDomainOnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            var thisAssembly = Assembly.GetExecutingAssembly();
            if (args.Name.Contains(thisAssembly.GetName().Name))
                return thisAssembly;
            return null;
        }
        #endregion

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

            var container = TestObjectFactories.CreateDefaultGlobalContainer(configurationHolder);
            var testRunnerFactory = container.Resolve<ITestRunnerFactory>();
            testRunnerFactory.Should().BeOfType(typeof(DummyTestRunnerFactory));
        }

        public class CustomUnitTestProvider : IUnitTestRuntimeProvider
        {
            public void TestPending(string message)
            {
                throw new NotImplementedException();
            }

            public void TestInconclusive(string message)
            {
                throw new NotImplementedException();
            }

            public void TestIgnore(string message)
            {
                throw new NotImplementedException();
            }

            public bool DelayedFixtureTearDown
            {
                get { throw new NotImplementedException(); }
            }
        }

        [Test]
        public void Should_be_able_to_specify_custom_unit_test_provider_in_config()
        {
            var configurationHolder = new StringConfigProvider(string.Format(@"<?xml version=""1.0"" encoding=""utf-8"" ?>
              <configuration>
                <specFlow>
                  <runtime>  
                    <dependencies>
                      <register type=""{0}"" as=""{1}"" name=""myprovider""/>
                    </dependencies>
                  </runtime>
                  <unitTestProvider name=""myprovider"" />
                </specFlow>
              </configuration>",
                typeof(CustomUnitTestProvider).AssemblyQualifiedName,
                typeof(IUnitTestRuntimeProvider).AssemblyQualifiedName));

            var container = TestObjectFactories.CreateDefaultGlobalContainer(configurationHolder);
            var unitTestProvider = container.Resolve<IUnitTestRuntimeProvider>();
            unitTestProvider.Should().BeOfType(typeof(CustomUnitTestProvider));
        }

        [Test]
        public void Should_be_able_to_specify_custom_unit_test_provider_in_config_compatible_way()
        {
            var configurationHolder = new StringConfigProvider(string.Format(@"<?xml version=""1.0"" encoding=""utf-8"" ?>
              <configuration>
                <specFlow>
                  <unitTestProvider name=""myprovider"" runtimeProvider=""{0}"" />
                </specFlow>
              </configuration>",
                typeof(CustomUnitTestProvider).AssemblyQualifiedName));

            var container = TestObjectFactories.CreateDefaultGlobalContainer(configurationHolder);
            var unitTestProvider = container.Resolve<IUnitTestRuntimeProvider>();
            unitTestProvider.Should().BeOfType(typeof(CustomUnitTestProvider));
        }
    }
}
