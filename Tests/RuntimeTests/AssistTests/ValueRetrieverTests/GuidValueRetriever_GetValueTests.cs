using System;
using NUnit.Framework;
using Should;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    [TestFixture]
    public class GuidValueRetriever_GetValueTests
    {
        [Test]
        public void Returns_a_guid_when_passed_a_valid_guid_string()
        {
            var retriever = new GuidValueRetriever();
            retriever.GetValue("6734CD2C-215B-4F4C-87C0-363ECAC6B9C1")
                .ShouldEqual(new Guid("6734CD2C-215B-4F4C-87C0-363ECAC6B9C1"));

            retriever.GetValue("2A6E290D-5C4C-4F6D-92F7-0A5CDA038FCD")
                .ShouldEqual(new Guid("2A6E290D-5C4C-4F6D-92F7-0A5CDA038FCD"));
        }

        [Test]
        public void Returns_a_guid_when_passed_a_lower_case_guid()
        {
            var retriever = new GuidValueRetriever();
            retriever.GetValue("6734cd2c-215b-4f4c-87c0-363ecac6b9c1")
                .ShouldEqual(new Guid("6734CD2C-215B-4F4C-87C0-363ECAC6B9C1"));

            retriever.GetValue("2a6e290d-5c4c-4f6d-92f7-0a5cda038fcd")
                .ShouldEqual(new Guid("2A6E290D-5C4C-4F6D-92F7-0A5CDA038FCD"));
        }

        [Test]
        public void Returns_a_guid_when_wrapped_in_curly_braces()
        {
            var retriever = new GuidValueRetriever();
            retriever.GetValue("{A04BE0E5-D9EE-4188-993B-899FF82A2B68}")
                .ShouldEqual(new Guid("A04BE0E5-D9EE-4188-993B-899FF82A2B68"));

            retriever.GetValue("{BF114C72-618C-48EC-98FE-F2804256A280}")
                .ShouldEqual(new Guid("BF114C72-618C-48EC-98FE-F2804256A280"));
        }

        [Test]
        public void Returns_an_empty_guid_when_passed_invalid_value()
        {
            var retriever = new GuidValueRetriever();
            retriever.GetValue(null).ShouldEqual(new Guid());
            retriever.GetValue("").ShouldEqual(new Guid());
            retriever.GetValue("xxxxx").ShouldEqual(new Guid());
        }

        [Test]
        public void Adds_trailing_zeroes_when_passed_one_valid_guid_character()
        {
            var retriever = new GuidValueRetriever();
            retriever.GetValue("1")
                .ShouldEqual(new Guid("10000000-0000-0000-0000-000000000000"));
            retriever.GetValue("2")
                .ShouldEqual(new Guid("20000000-0000-0000-0000-000000000000"));
            retriever.GetValue("F")
                .ShouldEqual(new Guid("F0000000-0000-0000-0000-000000000000"));
        }

        [Test]
        public void Adds_trailing_zeroes_when_passed_two_valid_guid_character()
        {
            var retriever = new GuidValueRetriever();
            retriever.GetValue("12")
                .ShouldEqual(new Guid("12000000-0000-0000-0000-000000000000"));
            retriever.GetValue("23")
                .ShouldEqual(new Guid("23000000-0000-0000-0000-000000000000"));
            retriever.GetValue("DF")
                .ShouldEqual(new Guid("DF000000-0000-0000-0000-000000000000"));
        }

        [Test]
        public void Adds_trailing_zeroes_when_passed_32_valid_guid_character()
        {
            var retriever = new GuidValueRetriever();
            retriever.GetValue("12000000-0000-0000-0000-00000000001")
                .ShouldEqual(new Guid("12000000-0000-0000-0000-000000000010"));
            retriever.GetValue("23000000-0000-0000-0000-00000000009")
                .ShouldEqual(new Guid("23000000-0000-0000-0000-000000000090"));
            retriever.GetValue("DF000000-0000-0000-0000-00000000007")
                .ShouldEqual(new Guid("DF000000-0000-0000-0000-000000000070"));
        }

        [Test]
        public void Adds_trailing_zeroes_when_passed_32_valid_guid_characters_with_no_dashes()
        {
            var retriever = new GuidValueRetriever();
            retriever.GetValue("1200000000000000000000000000001")
                .ShouldEqual(new Guid("12000000-0000-0000-0000-000000000010"));
            retriever.GetValue("2300000000000000000000000000009")
                .ShouldEqual(new Guid("23000000-0000-0000-0000-000000000090"));
            retriever.GetValue("DF00000000000000000000000000007")
                .ShouldEqual(new Guid("DF000000-0000-0000-0000-000000000070"));
        }

        [Test]
        public void Adds_trailing_zeroes_when_passed_9_valid_guid_characters_with_no_dashes()
        {
            var retriever = new GuidValueRetriever();
            retriever.GetValue("123456789")
                .ShouldEqual(new Guid("12345678-9000-0000-0000-000000000000"));
            retriever.GetValue("121212123")
                .ShouldEqual(new Guid("12121212-3000-0000-0000-000000000000"));
        }
    }
}