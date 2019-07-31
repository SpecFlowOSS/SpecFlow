using System.Linq;
using System.Reflection;
using Xunit;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Bindings.Discovery;

namespace TechTalk.SpecFlow.RuntimeTests
{
    
    public class RuntimeBindingRegistryBuilderTests
    {
        public RuntimeBindingRegistryBuilderTests()
        {
            bindingSourceProcessorStub = new BindingSourceProcessorStub();
        }

        [Binding]
        public class StepTransformationExample
        {
            [StepArgumentTransformation("BindingRegistryTests")]
            public int Transform(string val)
            {
                return 42;
            }
        }

        private BindingSourceProcessorStub bindingSourceProcessorStub;

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
        public class PrioritizedHookExample
        {
            [BeforeScenario]
            public void OrderTenThousand()
            {
            }

            [Before(Order = 9000)]
            public void OrderNineThousand()
            {
            }

            [BeforeScenarioBlock(Order = 10001)]
            public void OrderTenThousandAnd1()
            {
            }

            [BeforeFeature(Order = 10002)]
            public static void OrderTenThousandAnd2()
            {
            }

            [BeforeStep(Order = 10003)]
            public void OrderTenThousandAnd3()
            {
            }

            [BeforeTestRun(Order = 10004)]
            public static void OrderTenThousandAnd4()
            {
            }

            [AfterScenario]
            public void AfterOrderTenThousand()
            {
            }

            [After(Order = 9000)]
            public void AfterOrderNineThousand()
            {
            }

            [AfterScenarioBlock(Order = 10001)]
            public void AfterOrderTenThousandAnd1()
            {
            }

            [AfterFeature(Order = 10002)]
            public static void AfterOrderTenThousandAnd2()
            {
            }

            [AfterStep(Order = 10003)]
            public void AfterOrderTenThousandAnd3()
            {
            }

            [AfterTestRun(Order = 10004)]
            public static void AfterOrderTenThousandAnd4()
            {
            }
        }

        [Fact]
        public void ShouldFindBinding_WithDefaultOrder()
        {
            var builder = new RuntimeBindingRegistryBuilder(bindingSourceProcessorStub, new SpecFlowAttributesFilter());

            builder.BuildBindingsFromType(typeof (ScopedHookExample));

            Assert.Equal(4, bindingSourceProcessorStub.HookBindings.Count(s => s.HookOrder == 10000));
        }

        [Fact]
        public void ShouldFindBinding_WithSpecifiedPriorities()
        {
            var builder = new RuntimeBindingRegistryBuilder(bindingSourceProcessorStub, new SpecFlowAttributesFilter());

            builder.BuildBindingsFromType(typeof (PrioritizedHookExample));

            Assert.Equal(1,
                bindingSourceProcessorStub.HookBindings.Count(
                    s =>
                        s.HookType == HookType.BeforeScenario && s.Method.Name == "OrderTenThousand" &&
                        s.HookOrder == 10000));
            Assert.Equal(1,
                bindingSourceProcessorStub.HookBindings.Count(
                    s =>
                        s.HookType == HookType.BeforeScenario && s.Method.Name == "OrderNineThousand" &&
                        s.HookOrder == 9000));
            Assert.Equal(1,
                bindingSourceProcessorStub.HookBindings.Count(
                    s =>
                        s.HookType == HookType.BeforeScenarioBlock && s.Method.Name == "OrderTenThousandAnd1" &&
                        s.HookOrder == 10001));
            Assert.Equal(1,
                bindingSourceProcessorStub.HookBindings.Count(
                    s =>
                        s.HookType == HookType.BeforeFeature && s.Method.Name == "OrderTenThousandAnd2" &&
                        s.HookOrder == 10002));
            Assert.Equal(1,
                bindingSourceProcessorStub.HookBindings.Count(
                    s =>
                        s.HookType == HookType.BeforeStep && s.Method.Name == "OrderTenThousandAnd3" &&
                        s.HookOrder == 10003));
            Assert.Equal(1,
                bindingSourceProcessorStub.HookBindings.Count(
                    s =>
                        s.HookType == HookType.BeforeTestRun && s.Method.Name == "OrderTenThousandAnd4" &&
                        s.HookOrder == 10004));

            Assert.Equal(1,
                bindingSourceProcessorStub.HookBindings.Count(
                    s =>
                        s.HookType == HookType.AfterScenario && s.Method.Name == "AfterOrderTenThousand" &&
                        s.HookOrder == 10000));
            Assert.Equal(1,
                bindingSourceProcessorStub.HookBindings.Count(
                    s =>
                        s.HookType == HookType.AfterScenario && s.Method.Name == "AfterOrderNineThousand" &&
                        s.HookOrder == 9000));
            Assert.Equal(1,
                bindingSourceProcessorStub.HookBindings.Count(
                    s =>
                        s.HookType == HookType.AfterScenarioBlock && s.Method.Name == "AfterOrderTenThousandAnd1" &&
                        s.HookOrder == 10001));
            Assert.Equal(1,
                bindingSourceProcessorStub.HookBindings.Count(
                    s =>
                        s.HookType == HookType.AfterFeature && s.Method.Name == "AfterOrderTenThousandAnd2" &&
                        s.HookOrder == 10002));
            Assert.Equal(1,
                bindingSourceProcessorStub.HookBindings.Count(
                    s =>
                        s.HookType == HookType.AfterStep && s.Method.Name == "AfterOrderTenThousandAnd3" &&
                        s.HookOrder == 10003));
            Assert.Equal(1,
                bindingSourceProcessorStub.HookBindings.Count(
                    s =>
                        s.HookType == HookType.AfterTestRun && s.Method.Name == "AfterOrderTenThousandAnd4" &&
                        s.HookOrder == 10004));
        }

        [Fact]
        public void ShouldFindExampleConverter()
        {
            var builder = new RuntimeBindingRegistryBuilder(bindingSourceProcessorStub, new SpecFlowAttributesFilter());

            builder.BuildBindingsFromAssembly(Assembly.GetExecutingAssembly());
            Assert.Equal(1,
                bindingSourceProcessorStub.StepArgumentTransformationBindings.Count(
                    s =>
                        s.Regex != null && s.Regex.Match("BindingRegistryTests").Success &&
                        s.Regex.Match("").Success == false));
        }

        [Fact]
        public void ShouldFindScopedExampleConverter()
        {
            var builder = new RuntimeBindingRegistryBuilder(bindingSourceProcessorStub, new SpecFlowAttributesFilter());

            builder.BuildBindingsFromAssembly(Assembly.GetExecutingAssembly());

            Assert.Equal(2,
                bindingSourceProcessorStub.StepDefinitionBindings.Count(
                    s =>
                        s.StepDefinitionType == StepDefinitionType.Then &&
                        s.Regex.Match("SpecificBindingRegistryTests").Success && s.IsScoped));
            Assert.Equal(0,
                bindingSourceProcessorStub.StepDefinitionBindings.Count(
                    s =>
                        s.StepDefinitionType == StepDefinitionType.Then &&
                        s.Regex.Match("SpecificBindingRegistryTests").Success && s.IsScoped == false));
        }

        [Fact]
        public void ShouldFindScopedHook_WithCtorArg()
        {
            var builder = new RuntimeBindingRegistryBuilder(bindingSourceProcessorStub, new SpecFlowAttributesFilter());

            builder.BuildBindingsFromAssembly(Assembly.GetExecutingAssembly());

            Assert.Equal(1,
                bindingSourceProcessorStub.HookBindings.Count(s => s.Method.Name == "Tag2BeforeScenario" && s.IsScoped));
        }

        [Fact]
        public void ShouldFindScopedHook_WithMultipleCtorArg()
        {
            var builder = new RuntimeBindingRegistryBuilder(bindingSourceProcessorStub, new SpecFlowAttributesFilter());

            builder.BuildBindingsFromAssembly(Assembly.GetExecutingAssembly());

            Assert.Equal(2,
                bindingSourceProcessorStub.HookBindings.Count(s => s.Method.Name == "Tag34BeforeScenario" && s.IsScoped));
        }

        [Fact]
        public void ShouldFindScopedHook_WithScopeAttribute()
        {
            var builder = new RuntimeBindingRegistryBuilder(bindingSourceProcessorStub, new SpecFlowAttributesFilter());

            builder.BuildBindingsFromAssembly(Assembly.GetExecutingAssembly());

            Assert.Equal(1,
                bindingSourceProcessorStub.HookBindings.Count(s => s.Method.Name == "Tag1BeforeScenario" && s.IsScoped));
        }
    }
}