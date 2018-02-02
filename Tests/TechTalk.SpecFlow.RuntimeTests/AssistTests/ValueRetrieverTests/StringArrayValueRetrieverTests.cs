using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    [TestFixture]
    public class StringArrayValueRetrieverTests
    {
        [Test]
        public void Can_retrieve_string_array_properties()
        {
            var retriever = new StringArrayValueRetriever();

            retriever.CanRetrieve(new KeyValuePair<string, string>(), null, typeof(string[])).Should().BeTrue();
        }

        [Test]
        public void Cannot_retrieve_other_properties()
        {
            var retriever = new StringArrayValueRetriever();

            retriever.CanRetrieve(new KeyValuePair<string, string>(), null, typeof(int[])).Should().BeFalse();
            retriever.CanRetrieve(new KeyValuePair<string, string>(), null, typeof(List<string>)).Should().BeFalse();
            retriever.CanRetrieve(new KeyValuePair<string, string>(), null, typeof(string)).Should().BeFalse();
        }

        [Test]
        public void Returns_array_from_comma_separated_list()
        {
            var retriever = new StringArrayValueRetriever();

            var result = retriever.GetValue("A,B,C");

            result[0].Should().Be("A");
            result[1].Should().Be("B");
            result[2].Should().Be("C");
        }

        [Test]
        public void Trims_the_individual_values()
        {
            var retriever = new StringArrayValueRetriever();

            var result = retriever.GetValue("A , B, C ");

            result[0].Should().Be("A");
            result[1].Should().Be("B");
            result[2].Should().Be("C");
        }
    }
}