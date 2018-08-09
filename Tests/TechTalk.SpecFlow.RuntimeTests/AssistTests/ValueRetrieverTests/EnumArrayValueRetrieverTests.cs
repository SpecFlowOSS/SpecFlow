using System.Collections.Generic;
using FluentAssertions;
using TechTalk.SpecFlow.Assist.ValueRetrievers;
using Xunit;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    public class EnumArrayValueRetrieverTests
    {

        [Fact]
        public void Can_retrieve_when_property_is_array_of_enums()
        {
            var retriever = new EnumArrayValueRetriever();

            var result = retriever.CanRetrieve(new KeyValuePair<string, string>(), typeof(TestEnum[]), typeof(TestEnum[]));

            result.Should().BeTrue();
        }

        [Fact]
        public void Cannot_retrieve_when_property_is_array_of_ints()
        {
            var retriever = new EnumArrayValueRetriever();

            var result = retriever.CanRetrieve(new KeyValuePair<string, string>(), typeof(int[]), typeof(int[]));

            result.Should().BeFalse();

        }

        [Fact]
        public void Returns_array_of_enums_from_comma_separated_list()
        {
            var retriever = new EnumArrayValueRetriever();
            var dictionary = new KeyValuePair<string, string>("key", "Bar, Foo, FooBar");
           
            var result = retriever.Retrieve(dictionary, typeof(TestEnum[]), typeof(TestEnum[])) as TestEnum[];

            result[0].Should().Be(TestEnum.Bar);
            result[1].Should().Be(TestEnum.Foo);
            result[2].Should().Be(TestEnum.FooBar);
            result.Length.Should().Be(3);
        }

    }

    public enum TestEnum
    {
        Foo, Bar, FooBar
    }
}