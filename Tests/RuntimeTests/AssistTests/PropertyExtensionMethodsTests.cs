using System;
using NUnit.Framework;
using TechTalk.SpecFlow.Assist;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests
{
    [TestFixture]
    public class PropertyExtensionMethodsTests
    {
        [Test]
        public void Can_get_the_property_of_an_object_through_GetPropertyValue()
        {
            const string expectedValue = "John Galt";

            var person = new Person { FullName = expectedValue };
            var value = person.GetPropertyValue("FullName");

            Assert.AreEqual(expectedValue, value);
        }

        [Test]
        public void Can_set_the_value_on_the_property_through_SetPropertyValue()
        {
            const string expectedValue = "John Galt";

            var person = new Person { FullName = "Howard Roark" };
            person.SetPropertyValue("FullName", expectedValue);

            Assert.AreEqual(expectedValue, person.FullName);
        }

        public class Person
        {
            public string FullName { get; set; }
        }
    }
}
