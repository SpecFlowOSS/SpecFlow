using System;
using System.Reflection;
using NUnit.Framework;
using System.Linq;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Bindings.Discovery;

namespace TechTalk.SpecFlow.RuntimeTests
{
    [TestFixture]
    public class RuntimeBindingRegistryBuilderTests
    {
        [Binding]
        public class StepTransformationExample
        {
            [StepArgumentTransformation("BindingRegistryTests")]
            public int Transform(string val)
            {
                return 42;
            }
        }

        private BindingSourceProcessorStub bindingSourceProcessorStub = null;

        [SetUp]
        public void Setup()
        {
            bindingSourceProcessorStub = new BindingSourceProcessorStub();
        }

        [Test]
        public void ShouldFindExampleConverter()
        {
            RuntimeBindingRegistryBuilder builder = new RuntimeBindingRegistryBuilder(bindingSourceProcessorStub);

            builder.BuildBindingsFromAssembly(Assembly.GetExecutingAssembly());
            Assert.AreEqual(1, bindingSourceProcessorStub.StepArgumentTransformationBindings.Count(s => s.Regex != null && s.Regex.Match("BindingRegistryTests").Success && s.Regex.Match("").Success == false));
        }

        /*        Steps that are feature scoped               */

        [Binding]
        public class ScopedStepTransformationExample
        {
            [Then("SpecificBindingRegistryTests")]
            [Scope(Feature = "SomeFeature")]
            public int Transform(string val)
            {
                return 42;
            }
        }

        [Binding]
        public class ScopedStepTransformationExampleTheOther
        {
            [Then("SpecificBindingRegistryTests")]
            [Scope(Feature = "AnotherFeature")]
            public int Transform(string val)
            {
                return 24;
            }
        }

        [Test]
        public void ShouldFindScopedExampleConverter()
        {
            RuntimeBindingRegistryBuilder builder = new RuntimeBindingRegistryBuilder(bindingSourceProcessorStub);

            builder.BuildBindingsFromAssembly(Assembly.GetExecutingAssembly());

            Assert.AreEqual(2, bindingSourceProcessorStub.StepDefinitionBindings.Count(s => s.StepDefinitionType == StepDefinitionType.Then && s.Regex.Match("SpecificBindingRegistryTests").Success && s.IsScoped));
            Assert.AreEqual(0, bindingSourceProcessorStub.StepDefinitionBindings.Count(s => s.StepDefinitionType == StepDefinitionType.Then && s.Regex.Match("SpecificBindingRegistryTests").Success && s.IsScoped == false));
        }

        [Binding]
        public class ScopedHookExample
        {
            [BeforeScenario]
            [Scope(Tag = "tag1")]
            public void Tag1BeforeScenario()
            {
            }

            [BeforeScenario("tag2")]
            public void Tag2BeforeScenario()
            {
            }

            [BeforeScenario("tag3", "tag4")]
            public void Tag34BeforeScenario()
            {
            }
        }

        [Test]
        public void ShouldFindScopedHook_WithScopeAttribute()
        {
            RuntimeBindingRegistryBuilder builder = new RuntimeBindingRegistryBuilder(bindingSourceProcessorStub);

            builder.BuildBindingsFromAssembly(Assembly.GetExecutingAssembly());

            Assert.AreEqual(1, bindingSourceProcessorStub.HookBindings.Count(s => s.Method.Name == "Tag1BeforeScenario" && s.IsScoped));
        }

        [Test]
        public void ShouldFindScopedHook_WithCtorArg()
        {
            RuntimeBindingRegistryBuilder builder = new RuntimeBindingRegistryBuilder(bindingSourceProcessorStub);

            builder.BuildBindingsFromAssembly(Assembly.GetExecutingAssembly());

            Assert.AreEqual(1, bindingSourceProcessorStub.HookBindings.Count(s => s.Method.Name == "Tag2BeforeScenario" && s.IsScoped));
        }

        [Test]
        public void ShouldFindScopedHook_WithMultipleCtorArg()
        {
            RuntimeBindingRegistryBuilder builder = new RuntimeBindingRegistryBuilder(bindingSourceProcessorStub);

            builder.BuildBindingsFromAssembly(Assembly.GetExecutingAssembly());

            Assert.AreEqual(2, bindingSourceProcessorStub.HookBindings.Count(s => s.Method.Name == "Tag34BeforeScenario" && s.IsScoped));
        }
    }
}