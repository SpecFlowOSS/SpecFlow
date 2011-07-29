using System;
using NUnit.Framework;
using Should;
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
        public void Can_get_the_property_of_an_object_even_if_the_name_has_extra_spaces()
        {
            var person = new Person {FullName = "Howard Roark"};

            person.GetPropertyValue("Full Name")
                .ShouldEqual("Howard Roark");
        }

        [Test]
        public void Can_get_the_property_of_an_object_even_if_the_casing_is_wrong()
        {
            var person = new Person { FullName = "Howard Roark" };

            person.GetPropertyValue("fullname")
                .ShouldEqual("Howard Roark");
        }

        [Test]
        public void Can_set_the_value_on_the_property_through_SetPropertyValue()
        {
            const string expectedValue = "John Galt";

            var person = new Person { FullName = "Howard Roark" };
            person.SetPropertyValue("FullName", expectedValue);

            Assert.AreEqual(expectedValue, person.FullName);
        }

        [Test]
        public void Can_set_the_value_on_the_property_regardless_of_spaces()
        {
            var person = new Person { FullName = "Howard Roark" };
            person.SetPropertyValue("Full Name", "John Galt");

            person.FullName.ShouldEqual("John Galt");
        }

        [Test]
        public void Can_set_the_value_on_the_property_regardless_of_casing()
        {
            var person = new Person { FullName = "Howard Roark" };
            person.SetPropertyValue("full name", "John Galt");

            person.FullName.ShouldEqual("John Galt");
        }

        public class Person
        {
            public string FullName { get; set; }
        }
    }
}
