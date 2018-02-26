using System;
using Xunit;
using FluentAssertions;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    
    public class GuidValueRetriever_GetValueTests
    {
        [Fact]
        public void Returns_a_guid_when_passed_a_valid_guid_string()
        {
            var retriever = new GuidValueRetriever();
            retriever.GetValue("6734CD2C-215B-4F4C-87C0-363ECAC6B9C1")
                .Should().Be(new Guid("6734CD2C-215B-4F4C-87C0-363ECAC6B9C1"));

            retriever.GetValue("2A6E290D-5C4C-4F6D-92F7-0A5CDA038FCD")
                .Should().Be(new Guid("2A6E290D-5C4C-4F6D-92F7-0A5CDA038FCD"));
        }

        [Fact]
        public void Returns_a_guid_when_passed_a_lower_case_guid()
        {
            var retriever = new GuidValueRetriever();
            retriever.GetValue("6734cd2c-215b-4f4c-87c0-363ecac6b9c1")
                .Should().Be(new Guid("6734CD2C-215B-4F4C-87C0-363ECAC6B9C1"));

            retriever.GetValue("2a6e290d-5c4c-4f6d-92f7-0a5cda038fcd")
                .Should().Be(new Guid("2A6E290D-5C4C-4F6D-92F7-0A5CDA038FCD"));
        }

        [Fact]
        public void Returns_a_guid_when_wrapped_in_curly_braces()
        {
            var retriever = new GuidValueRetriever();
            retriever.GetValue("{A04BE0E5-D9EE-4188-993B-899FF82A2B68}")
                .Should().Be(new Guid("A04BE0E5-D9EE-4188-993B-899FF82A2B68"));

            retriever.GetValue("{BF114C72-618C-48EC-98FE-F2804256A280}")
                .Should().Be(new Guid("BF114C72-618C-48EC-98FE-F2804256A280"));
        }

        [Fact]
        public void Returns_an_empty_guid_when_passed_invalid_value()
        {
            var retriever = new GuidValueRetriever();
            retriever.GetValue(null).Should().Be(new Guid());
            retriever.GetValue("").Should().Be(new Guid());
            retriever.GetValue("xxxxx").Should().Be(new Guid());
        }

        [Fact]
        public void Adds_trailing_zeroes_when_passed_one_valid_guid_character()
        {
            var retriever = new GuidValueRetriever();
            retriever.GetValue("1")
                .Should().Be(new Guid("10000000-0000-0000-0000-000000000000"));
            retriever.GetValue("2")
                .Should().Be(new Guid("20000000-0000-0000-0000-000000000000"));
            retriever.GetValue("F")
                .Should().Be(new Guid("F0000000-0000-0000-0000-000000000000"));
        }

        [Fact]
        public void Adds_trailing_zeroes_when_passed_two_valid_guid_character()
        {
            var retriever = new GuidValueRetriever();
            retriever.GetValue("12")
                .Should().Be(new Guid("12000000-0000-0000-0000-000000000000"));
            retriever.GetValue("23")
                .Should().Be(new Guid("23000000-0000-0000-0000-000000000000"));
            retriever.GetValue("DF")
                .Should().Be(new Guid("DF000000-0000-0000-0000-000000000000"));
        }

        [Fact]
        public void Adds_trailing_zeroes_when_passed_32_valid_guid_character()
        {
            var retriever = new GuidValueRetriever();
            retriever.GetValue("12000000-0000-0000-0000-00000000001")
                .Should().Be(new Guid("12000000-0000-0000-0000-000000000010"));
            retriever.GetValue("23000000-0000-0000-0000-00000000009")
                .Should().Be(new Guid("23000000-0000-0000-0000-000000000090"));
            retriever.GetValue("DF000000-0000-0000-0000-00000000007")
                .Should().Be(new Guid("DF000000-0000-0000-0000-000000000070"));
        }

        [Fact]
        public void Adds_trailing_zeroes_when_passed_32_valid_guid_characters_with_no_dashes()
        {
            var retriever = new GuidValueRetriever();
            retriever.GetValue("1200000000000000000000000000001")
                .Should().Be(new Guid("12000000-0000-0000-0000-000000000010"));
            retriever.GetValue("2300000000000000000000000000009")
                .Should().Be(new Guid("23000000-0000-0000-0000-000000000090"));
            retriever.GetValue("DF00000000000000000000000000007")
                .Should().Be(new Guid("DF000000-0000-0000-0000-000000000070"));
        }

        [Fact]
        public void Adds_trailing_zeroes_when_passed_9_valid_guid_characters_with_no_dashes()
        {
            var retriever = new GuidValueRetriever();
            retriever.GetValue("123456789")
                .Should().Be(new Guid("12345678-9000-0000-0000-000000000000"));
            retriever.GetValue("121212123")
                .Should().Be(new Guid("12121212-3000-0000-0000-000000000000"));
        }
    }
}