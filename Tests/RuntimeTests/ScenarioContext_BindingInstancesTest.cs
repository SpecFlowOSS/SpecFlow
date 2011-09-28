using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BoDi;
using NUnit.Framework;
using Should;

namespace TechTalk.SpecFlow.RuntimeTests
{
    [TestFixture]
    public class ScenarioContext_BindingInstancesTest
    {
        private TestRunner testRunner;

        private ScenarioContext CreateScenarioContext(Action<IObjectContainer> registerMocks = null)
        {
            IObjectContainer container;
            testRunner = TestTestRunnerFactory.CreateTestRunner(out container, registerMocks);
            return new ScenarioContext(new ScenarioInfo("sample scenario", new string[0]), testRunner, container);
        }

        [Test]
        public void GetBindingInstance_should_create_new_instances_without_dependency()
        {
            var scenarioContext = CreateScenarioContext();

            var result = scenarioContext.GetBindingInstance(typeof(SimpleClass));
            result.ShouldNotBeNull();
            result.ShouldBeType(typeof (SimpleClass));
        }

        [Test]
        public void GetBindingInstance_should_create_new_instances_with_dependencies()
        {
            var scenarioContext = CreateScenarioContext();

            var result = scenarioContext.GetBindingInstance(typeof(ClassWithDependencies));
            result.ShouldNotBeNull();
            result.ShouldBeType(typeof(ClassWithDependencies));
        }

        [Test]
        public void GetBindingInstance_should_reuse_created_instances_for_dependencies()
        {
            var scenarioContext = CreateScenarioContext();

            var dependency = (SimpleClass)scenarioContext.GetBindingInstance(typeof(SimpleClass));
            var result = (ClassWithDependencies)scenarioContext.GetBindingInstance(typeof(ClassWithDependencies));

            result.TheDependency.ShouldBeSameAs(dependency);
        }

        [Test]
        public void GetBindingInstance_should_cache_created_instance()
        {
            var scenarioContext = CreateScenarioContext();

            var result1 = scenarioContext.GetBindingInstance(typeof(SimpleClass));
            var result2 = scenarioContext.GetBindingInstance(typeof(SimpleClass));
            result1.ShouldBeSameAs(result2);
        }

        [Test]
        public void GetBindingInstance_should_return_instance_specified_through_SetBindingInstance()
        {
            var scenarioContext = CreateScenarioContext();

            var expectedInstance = new SimpleClass();
            scenarioContext.SetBindingInstance(typeof(SimpleClass), expectedInstance);
            var result = scenarioContext.GetBindingInstance(typeof(SimpleClass));
            result.ShouldBeSameAs(expectedInstance);
        }

        [Test]
        public void Should_dispose_disposable_binding_instances()
        {
            var scenarioContext = CreateScenarioContext();

            var displosableInstance = (DisplosableClass)scenarioContext.GetBindingInstance(typeof(DisplosableClass));

            ((IDisposable)scenarioContext).Dispose();

            displosableInstance.WasDisposed.ShouldBeTrue();
        }

        [Test]
        public void Should_be_able_do_put_dependencies_on_specflow_objects()
        {
            var scenarioContext = CreateScenarioContext();

            var result = (ClassDependsOnTestRunner)scenarioContext.GetBindingInstance(typeof(ClassDependsOnTestRunner));

            result.TestRunner.ShouldBeSameAs(testRunner);
        }

        [Test]
        public void Dispose_should_not_dispose_specflow_objects()
        {
            var displosableInstance = new DisplosableClass();
            var scenarioContext = CreateScenarioContext(c => c.RegisterInstanceAs(displosableInstance));

            scenarioContext.GetBindingInstance(typeof(ClassDependsOnDisposable));

            ((IDisposable)scenarioContext).Dispose();

            displosableInstance.WasDisposed.ShouldBeFalse();
        }

        [Test]
        public void Should_support_classes_with_multiple_construictors()
        {
            var scenarioContext = CreateScenarioContext();

            var result = scenarioContext.GetBindingInstance(typeof(ClassWithMultipleCtor));

            result.ShouldNotBeNull();
        }

        [Test, ExpectedException(typeof(ObjectContainerException))]
        public void Should_throw_error_if_class_have_multiple_ctor_with_max_parameter_count()
        {
            var scenarioContext = CreateScenarioContext();

            scenarioContext.GetBindingInstance(typeof(ClassWithMultipleMaxParamCountCtor));
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
