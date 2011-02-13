using System;
using System.Globalization;
using System.Reflection;
using NUnit.Framework;
using System.Linq;
using TechTalk.SpecFlow.Bindings;

namespace TechTalk.SpecFlow.RuntimeTests
{
    [TestFixture]
    public class BindingRegistryTests
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

        [Test]
        public void ShouldFindExampleConverter()
        {
            BindingRegistry registry = new BindingRegistry();
            registry.BuildBindingsFromAssembly(Assembly.GetExecutingAssembly());

            Assert.AreEqual(1, registry.StepTransformations.Where(s => s.Regex != null && s.Regex.Match("BindingRegistryTests").Success && s.Regex.Match("").Success == false).Count());
        }

        /*        Steps that are feature scoped               */

        [Binding]
        public class ScopedStepTransformationExample
        {
            [Then("SpecificBindingRegistryTests")]
            [StepScope(Feature = "SomeFeature")]
            public int Transform(string val)
            {
                return 42;
            }
        }

        [Binding]
        public class ScopedStepTransformationExampleTheOther
        {
            [Then("SpecificBindingRegistryTests")]
            [StepScope(Feature = "AnotherFeature")]
            public int Transform(string val)
            {
                return 24;
            }
        }

        [Test]
        public void ShouldFindScopedExampleConverter()
        {
            BindingRegistry registry = new BindingRegistry();
            registry.BuildBindingsFromAssembly(Assembly.GetExecutingAssembly());

            Assert.AreEqual(2,
                registry.Where(s => s.Regex.Match("SpecificBindingRegistryTests").Success && s.IsScoped).Count());

            Assert.AreEqual(0,
                registry.Where(s => s.Regex.Match("SpecificBindingRegistryTests").Success && s.IsScoped == false).Count());
        }
    }
}