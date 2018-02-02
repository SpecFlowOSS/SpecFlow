using FluentAssertions;
using NUnit.Framework;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    [TestFixture]
    public class StringListValueRetrieverTests
    {
        [Test]
        public void Returns_lsit_from_comma_separated_list()
        {
            var retriever = new StringListValueRetriever();

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