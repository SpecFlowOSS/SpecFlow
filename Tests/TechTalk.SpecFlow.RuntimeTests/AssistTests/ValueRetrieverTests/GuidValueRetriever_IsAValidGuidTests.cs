using FluentAssertions;
using Xunit;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    
    public class GuidValueRetriever_IsAValidGuidTests
    {
        [Fact]
        public void Returns_false_when_the_guids_contain_invalid_characters()
        {
            var retriever = new GuidValueRetriever();

            retriever.IsAValidGuid("z").Should().BeFalse();
            retriever.IsAValidGuid("{9537F8CA-EB2B-4B8A-97D5-705778AAD1G5}").Should().BeFalse();
        }

        [Fact]
        public void Returns_true_when_the_guids_can_be_valid()
        {
            var retriever = new GuidValueRetriever();

            retriever.IsAValidGuid("0").Should().BeTrue();
            retriever.IsAValidGuid("{47B864A3-DEB9-4222-B7F8-F176F2E2CEFF}").Should().BeTrue();
        }

        [Fact]
        public void Returns_false_when_the_guid_is_null()
        {
            var retriever = new GuidValueRetriever();

            retriever.IsAValidGuid(null).Should().BeFalse();
        }

        [Fact]
        public void Returns_false_when_the_guid_is_empty()
        {
            var retriever = new GuidValueRetriever();

            retriever.IsAValidGuid(string.Empty).Should().BeFalse();
        }
    }
}