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

        [Binding]
        public class PrioritisedHookExample
        {
            [BeforeScenario]
            public void PriorityTenThousand()
            {                
            }

            [Before(9000)]
            public void PriorityNineThousand()
            {
            }

            [BeforeScenarioBlock(10001)]
            public void PriorityTenThousandAnd1()
            {
            }

            [BeforeFeature(10002)]
            public static void PriorityTenThousandAnd2()
            {
            }

            [BeforeStep(10003)]
            public void PriorityTenThousandAnd3()
            {
            }

            [BeforeTestRun(10004)]
            public static void PriorityTenThousandAnd4()
            {
            }

            [AfterScenario]
            public void AfterPriorityTenThousand()
            {
            }

            [After(9000)]
            public void AfterPriorityNineThousand()
            {
            }

            [AfterScenarioBlock(10001)]
            public void AfterPriorityTenThousandAnd1()
            {
            }

            [AfterFeature(10002)]
            public static void AfterPriorityTenThousandAnd2()
            {
            }

            [AfterStep(10003)]
            public void AfterPriorityTenThousandAnd3()
            {
            }

            [AfterTestRun(10004)]
            public static void AfterPriorityTenThousandAnd4()
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

        [Test]
        public void ShouldFindBinding_WithDefaultPriority()
        {
            RuntimeBindingRegistryBuilder builder = new RuntimeBindingRegistryBuilder(bindingSourceProcessorStub);

            builder.BuildBindingsFromType(typeof(ScopedHookExample));

            Assert.AreEqual(4, bindingSourceProcessorStub.HookBindings.Count(s => s.HookPriority==10000));
        }

        [Test]
        public void ShouldFindBinding_WithSpecifiedPriorities()
        {
            RuntimeBindingRegistryBuilder builder = new RuntimeBindingRegistryBuilder(bindingSourceProcessorStub);

            builder.BuildBindingsFromType(typeof(PrioritisedHookExample));

            Assert.AreEqual(1, bindingSourceProcessorStub.HookBindings.Count(s => s.HookType==HookType.BeforeScenario && s.Method.Name == "PriorityTenThousand" && s.HookPriority == 10000));
            Assert.AreEqual(1, bindingSourceProcessorStub.HookBindings.Count(s => s.HookType == HookType.BeforeScenario && s.Method.Name == "PriorityNineThousand" && s.HookPriority == 9000));
            Assert.AreEqual(1, bindingSourceProcessorStub.HookBindings.Count(s => s.HookType == HookType.BeforeScenarioBlock && s.Method.Name == "PriorityTenThousandAnd1" && s.HookPriority == 10001));
            Assert.AreEqual(1, bindingSourceProcessorStub.HookBindings.Count(s => s.HookType == HookType.BeforeFeature && s.Method.Name == "PriorityTenThousandAnd2" && s.HookPriority == 10002));
            Assert.AreEqual(1, bindingSourceProcessorStub.HookBindings.Count(s => s.HookType == HookType.BeforeStep && s.Method.Name == "PriorityTenThousandAnd3" && s.HookPriority == 10003));
            Assert.AreEqual(1, bindingSourceProcessorStub.HookBindings.Count(s => s.HookType == HookType.BeforeTestRun && s.Method.Name == "PriorityTenThousandAnd4" && s.HookPriority == 10004));

            Assert.AreEqual(1, bindingSourceProcessorStub.HookBindings.Count(s => s.HookType == HookType.AfterScenario && s.Method.Name == "AfterPriorityTenThousand" && s.HookPriority == 10000));
            Assert.AreEqual(1, bindingSourceProcessorStub.HookBindings.Count(s => s.HookType == HookType.AfterScenario && s.Method.Name == "AfterPriorityNineThousand" && s.HookPriority == 9000));
            Assert.AreEqual(1, bindingSourceProcessorStub.HookBindings.Count(s => s.HookType == HookType.AfterScenarioBlock && s.Method.Name == "AfterPriorityTenThousandAnd1" && s.HookPriority == 10001));
            Assert.AreEqual(1, bindingSourceProcessorStub.HookBindings.Count(s => s.HookType == HookType.AfterFeature && s.Method.Name == "AfterPriorityTenThousandAnd2" && s.HookPriority == 10002));
            Assert.AreEqual(1, bindingSourceProcessorStub.HookBindings.Count(s => s.HookType == HookType.AfterStep && s.Method.Name == "AfterPriorityTenThousandAnd3" && s.HookPriority == 10003));
            Assert.AreEqual(1, bindingSourceProcessorStub.HookBindings.Count(s => s.HookType == HookType.AfterTestRun && s.Method.Name == "AfterPriorityTenThousandAnd4" && s.HookPriority == 10004));
        }
    }
}