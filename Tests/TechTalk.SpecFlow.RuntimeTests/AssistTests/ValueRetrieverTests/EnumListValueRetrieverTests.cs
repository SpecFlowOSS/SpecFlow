using System.Collections.Generic;
using FluentAssertions;
using TechTalk.SpecFlow.Assist.ValueRetrievers;
using Xunit;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    public class EnumListValueRetrieverTests
    {

        [Fact]
        public void Can_retrieve_when_property_is_list_of_enums()
        {
            var retriever = new EnumListValueRetriever();

            var result = retriever.CanRetrieve(new KeyValuePair<string, string>(), typeof(List<TestEnum>), typeof(List<TestEnum>));

            result.Should().BeTrue();
        }

        [Fact]
        public void Can_retrieve_when_property_is_ilist_of_enums()
        {
            var retriever = new EnumListValueRetriever();

            var result = retriever.CanRetrieve(new KeyValuePair<string, string>(), typeof(IList<TestEnum>), typeof(IList<TestEnum>));

            result.Should().BeTrue();
        }

        [Fact]
        public void Cannot_retrieve_when_property_is_List_of_ints()
        {
            var retriever = new EnumListValueRetriever();

            var result = retriever.CanRetrieve(new KeyValuePair<string, string>(), typeof(List<int>), typeof(List<int>));

            result.Should().BeFalse();
        }

        [Fact]
        public void Cannot_retrieve_when_property_is_List_of_strings()
        {
            var retriever = new EnumListValueRetriever();

            var result = retriever.CanRetrieve(new KeyValuePair<string, string>(), typeof(List<string>), typeof(List<string>));

            result.Should().BeFalse();
        }

        [Fact]
        public void Returns_list_of_enums_from_comma_separated_list()
        {
            var retriever = new EnumListValueRetriever();
            var dictionary = new KeyValuePair<string, string>("key", "Bar, Foo, FooBar");
           
            var result = retriever.Retrieve(dictionary, typeof(List<TestEnum>), typeof(List<TestEnum>)) as List<TestEnum>;

            result[0].Should().Be(TestEnum.Bar);
            result[1].Should().Be(TestEnum.Foo);
            result[2].Should().Be(TestEnum.FooBar);
            result.Count.Should().Be(3);
        }

    }

}