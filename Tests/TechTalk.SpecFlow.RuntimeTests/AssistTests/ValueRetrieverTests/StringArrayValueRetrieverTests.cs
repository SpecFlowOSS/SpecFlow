using System.Collections.Generic;
using FluentAssertions;
using TechTalk.SpecFlow.Assist.ValueRetrievers;
using Xunit;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    public class StringArrayValueRetrieverTests
    {
        [Fact]
        public void Can_retrieve_string_array_properties()
        {
            var retriever = new StringArrayValueRetriever();

            retriever.CanRetrieve(new KeyValuePair<string, string>(), null, typeof(string[])).Should().BeTrue();
        }

        [Fact]
        public void Cannot_retrieve_other_properties()
        {
            var retriever = new StringArrayValueRetriever();

            retriever.CanRetrieve(new KeyValuePair<string, string>(), null, typeof(int[])).Should().BeFalse();
            retriever.CanRetrieve(new KeyValuePair<string, string>(), null, typeof(List<string>)).Should().BeFalse();
            retriever.CanRetrieve(new KeyValuePair<string, string>(), null, typeof(string)).Should().BeFalse();
        }

        [Fact]
        public void Returns_array_from_comma_separated_list()
        {
            var retriever = new StringArrayValueRetriever();

            var result = retriever.GetValue("A,B,C");

            AssertTheArray(result);
        }



        [Fact]
        public void Returns_array_from_semicolon_separated_list()
        {
            var retriever = new StringArrayValueRetriever();

            var result = retriever.GetValue("A;B;C");

            AssertTheArray(result);
        }

        [Fact]
        public void Returns_array_from_mixed_separator_list()
        {
            var retriever = new StringArrayValueRetriever();

            var result = retriever.GetValue("A,B;C");

            AssertTheArray(result);
        }


        [Fact]
        public void Trims_the_individual_values()
        {
            var retriever = new StringArrayValueRetriever();

            var result = retriever.GetValue("A , B, C ");

            AssertTheArray(result);
        }

        static void AssertTheArray(string[] result)
        {
            result[0].Should().Be("A");
            result[1].Should().Be("B");
            result[2].Should().Be("C");
            result.Length.Should().Be(3);
        }
    }
}