using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using BoDi;
using Xunit;
using FluentAssertions;
using Moq;
using TechTalk.SpecFlow.Infrastructure;

namespace TechTalk.SpecFlow.RuntimeTests
{
    
    public class ScenarioContext_BindingInstancesTest
    {
        private TestRunner testRunner;

        private ScenarioContext CreateScenarioContext(Action<IObjectContainer> registerTestThreadMocks = null, Action<IObjectContainer> registerGlobalMocks = null)
        {
            IObjectContainer testThreadContainer;
            testRunner = TestObjectFactories.CreateTestRunner(out testThreadContainer, registerTestThreadMocks, registerGlobalMocks);
            return new ScenarioContext(new ObjectContainer(testThreadContainer), new ScenarioInfo("sample scenario", "sample scenario description", new string[0], null), testThreadContainer.Resolve<ITestObjectResolver>());
        }

        [Fact]
        public void GetBindingInstance_should_create_new_instances_without_dependency()
        {
            var scenarioContext = CreateScenarioContext();

            var result = scenarioContext.GetBindingInstance(typeof(SimpleClass));
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof (SimpleClass));
        }

        [Fact]
        public void GetBindingInstance_should_create_new_instances_with_dependencies()
        {
            var scenarioContext = CreateScenarioContext();

            var result = scenarioContext.GetBindingInstance(typeof(ClassWithDependencies));
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(ClassWithDependencies));
        }

        [Fact]
        public void GetBindingInstance_should_reuse_created_instances_for_dependencies()
        {
            var scenarioContext = CreateScenarioContext();

            var dependency = (SimpleClass)scenarioContext.GetBindingInstance(typeof(SimpleClass));
            var result = (ClassWithDependencies)scenarioContext.GetBindingInstance(typeof(ClassWithDependencies));

            result.TheDependency.Should().Be(dependency);
        }

        [Fact]
        public void GetBindingInstance_should_cache_created_instance()
        {
            var scenarioContext = CreateScenarioContext();

            var result1 = scenarioContext.GetBindingInstance(typeof(SimpleClass));
            var result2 = scenarioContext.GetBindingInstance(typeof(SimpleClass));
            result1.Should().Be(result2);
        }

        [Fact]
        public void GetBindingInstance_should_return_instance_specified_through_SetBindingInstance()
        {
            var scenarioContext = CreateScenarioContext();

            var expectedInstance = new SimpleClass();
            scenarioContext.SetBindingInstance(typeof(SimpleClass), expectedInstance);
            var result = scenarioContext.GetBindingInstance(typeof(SimpleClass));
            result.Should().Be(expectedInstance);
        }

        [Fact]
        public void GetBindingInstance_should_return_instance_through_TestObjectResolver()
        {
            var expectedInstance = new SimpleClass();

            var scenarioContext = CreateScenarioContext(registerGlobalMocks: (globalContainer =>
            {
                var testObjectResolverMock = new Mock<ITestObjectResolver>();
                testObjectResolverMock.Setup(m => m.ResolveBindingInstance(typeof(SimpleClass), It.IsAny<IObjectContainer>()))
                    .Returns(expectedInstance);
                globalContainer.RegisterInstanceAs(testObjectResolverMock.Object);
            }));

            var result = scenarioContext.GetBindingInstance(typeof(SimpleClass));
            result.Should().Be(expectedInstance);
        }

        [Fact]
        public void Should_dispose_disposable_binding_instances()
        {
            var scenarioContext = CreateScenarioContext();

            var displosableInstance = (DisplosableClass)scenarioContext.GetBindingInstance(typeof(DisplosableClass));

            scenarioContext.ScenarioContainer.Dispose();

            displosableInstance.WasDisposed.Should().BeTrue();
        }

        [Fact]
        public void Should_be_able_do_put_dependencies_on_specflow_objects()
        {
            var scenarioContext = CreateScenarioContext();

            var result = (ClassDependsOnTestRunner)scenarioContext.GetBindingInstance(typeof(ClassDependsOnTestRunner));

            result.TestRunner.Should().Be(testRunner);
        }

        [Fact]
        public void Dispose_should_not_dispose_specflow_objects()
        {
            var displosableInstance = new DisplosableClass();
            var scenarioContext = CreateScenarioContext(c => c.RegisterInstanceAs(displosableInstance));

            scenarioContext.GetBindingInstance(typeof(ClassDependsOnDisposable));

            ((IDisposable)scenarioContext).Dispose();

            displosableInstance.WasDisposed.Should().BeFalse();
        }

        [Fact]
        public void Should_support_classes_with_multiple_construictors()
        {
            var scenarioContext = CreateScenarioContext();

            var result = scenarioContext.GetBindingInstance(typeof(ClassWithMultipleCtor));

            result.Should().NotBeNull();
        }

        [Fact]
        public void Should_throw_error_if_class_have_multiple_ctor_with_max_parameter_count()
        {
            var scenarioContext = CreateScenarioContext();

            Assert.Throws<ObjectContainerException>(() => scenarioContext.GetBindingInstance(typeof(ClassWithMultipleMaxParamCountCtor)));
        }

        public class SimpleClass
        {
            public string WasCreatedBy { get; set; }
        }

        public class ClassWithDependencies
        {
            private readonly SimpleClass _context;

            public ClassWithDependencies(SimpleClass context)
            {
                _context = context;
            }

            public SimpleClass TheDependency
            {
                get { return _context; }
            }
        }

        public class DisplosableClass : IDisposable
        {
            public bool WasDisposed { get; private set; }

            public void Dispose()
            {
                WasDisposed = true;
            }
        }

        public class ClassDependsOnTestRunner
        {
            public ITestRunner TestRunner { get; private set; }

            public ClassDependsOnTestRunner(ITestRunner testRunner)
            {
                TestRunner = testRunner;
            }
        }

        public class ClassDependsOnDisposable
        {
            public DisplosableClass DisplosableInstance { get; private set; }

            public ClassDependsOnDisposable(DisplosableClass testRunner)
            {
                DisplosableInstance = testRunner;
            }
        }

        public class ClassWithMultipleCtor
        {
            public ClassWithMultipleCtor()
            {
            }

            public ClassWithMultipleCtor(SimpleClass context)
            {
            }
        }

        public class ClassWithMultipleMaxParamCountCtor
        {
            public ClassWithMultipleMaxParamCountCtor(DisplosableClass displosableClass)
            {
            }

            public ClassWithMultipleMaxParamCountCtor(SimpleClass simpleClass)
            {
            }
        }
    }
}
