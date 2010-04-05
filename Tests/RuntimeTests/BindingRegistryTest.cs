using System;
using System.Globalization;
using System.Reflection;
using NUnit.Framework;
using System.Linq;

namespace TechTalk.SpecFlow.RuntimeTests
{
    [TestFixture]
    public class BindingRegistryTests
    {
        [Binding]
        public class StepTransformationExample
        {
            [StepTransformation("BindingRegistryTests")]
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

            Assert.AreEqual(1, registry.StepTransformations.Where(s => s.Regex.Match("BindingRegistryTests").Success).Count());
        }
    }
}