using System;
using System.Diagnostics;
using System.Reflection;
using Xunit;
using TechTalk.SpecFlow.Configuration;
using FluentAssertions;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.UnitTestProvider;

namespace TechTalk.SpecFlow.RuntimeTests.Infrastructure
{
    
    public class TestRunContainerBuilderTests : IDisposable
    {
        [Fact]
        public void Should_create_a_container()
        {
            var container = TestObjectFactories.CreateDefaultGlobalContainer();
            container.Should().NotBeNull();
        }

        [Fact]
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
        public TestRunContainerBuilderTests()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainOnAssemblyResolve;
        }
        

        private Assembly CurrentDomainOnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            var thisAssembly = Assembly.GetExecutingAssembly();
            if (args.Name.Contains(thisAssembly.GetName().Name))
                return thisAssembly;
            return null;
        }
        #endregion

        [Fact]
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

        [Fact]
        public void Should_be_able_to_customize_dependencies_from_json_config()
        {
            var expectedInterface = typeof(ITestRunnerFactory).AssemblyQualifiedName;
            var expectedImplementation = typeof(DummyTestRunnerFactory).AssemblyQualifiedName;
            
            var configurationHolder = new StringConfigProvider(
            $@"{{
                ""runtime"": {{ 
                    ""dependencies"": [
                        {{
                            ""type"": ""{expectedImplementation}"",
                            ""as"": ""{expectedInterface}""
                        }}
                    ]
                }}
            }}");

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

        public void Dispose()
        {
            AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomainOnAssemblyResolve;
        }
    }
}
