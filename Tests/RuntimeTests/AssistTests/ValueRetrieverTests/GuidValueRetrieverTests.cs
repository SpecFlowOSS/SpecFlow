using System;
using NUnit.Framework;
using Should;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    [TestFixture]
    public class GuidValueRetrieverTests
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
    }
}