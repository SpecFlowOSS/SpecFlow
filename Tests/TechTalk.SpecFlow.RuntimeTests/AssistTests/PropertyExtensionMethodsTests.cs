using System;
using Xunit;
using FluentAssertions;
using TechTalk.SpecFlow.Assist;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests
{
    
    public class PropertyExtensionMethodsTests
    {
        [Fact]
        public void Can_get_the_property_of_an_object_through_GetPropertyValue()
        {
            const string expectedValue = "John Galt";

            var person = new Person { FullName = expectedValue };
            var value = person.GetPropertyValue("FullName");

            Assert.Equal(expectedValue, value);
        }

        [Fact]
        public void Can_get_the_property_of_an_object_even_if_the_name_has_extra_spaces()
        {
            var person = new Person {FullName = "Howard Roark"};

            person.GetPropertyValue("Full Name")
                .Should().Be("Howard Roark");
        }

        [Fact]
        public void Can_get_the_property_of_an_object_even_if_the_casing_is_wrong()
        {
            var person = new Person { FullName = "Howard Roark" };

            person.GetPropertyValue("fullname")
                .Should().Be("Howard Roark");
        }

        [Fact]
        public void Can_set_the_value_on_the_property_through_SetPropertyValue()
        {
            const string expectedValue = "John Galt";

            var person = new Person { FullName = "Howard Roark" };
            person.SetPropertyValue("FullName", expectedValue);

            Assert.Equal(expectedValue, person.FullName);
        }

        [Fact]
        public void Can_set_the_value_on_the_property_regardless_of_spaces()
        {
            var person = new Person { FullName = "Howard Roark" };
            person.SetPropertyValue("Full Name", "John Galt");

            person.FullName.Should().Be("John Galt");
        }

        [Fact]
        public void Can_set_the_value_on_the_property_regardless_of_casing()
        {
            var person = new Person { FullName = "Howard Roark" };
            person.SetPropertyValue("full name", "John Galt");

            person.FullName.Should().Be("John Galt");
        }

        public class Person
        {
            public string FullName { get; set; }
        }
    }
}
