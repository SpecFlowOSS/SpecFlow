using System;
using System.Collections.Generic;
using Xunit;
using FluentAssertions;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    public class GuidValueRetriever_GetValueTests
    {
        private const string IrrelevantKey = "Irrelevant";
        private readonly Type IrrelevantType = typeof(object);

        [Theory]
        [InlineData(typeof(Guid), true)]
        [InlineData(typeof(Guid?), true)]
        [InlineData(typeof(int), false)]
        public void CanRetrieve(Type type, bool expectation)
        {
            var retriever = new GuidValueRetriever();
            var result = retriever.CanRetrieve(new KeyValuePair<string, string>(IrrelevantKey, IrrelevantKey), IrrelevantType, type);
            result.Should().Be(expectation);
        }

        [Theory]
        [InlineData("6734CD2C-215B-4F4C-87C0-363ECAC6B9C1")]
        [InlineData("2A6E290D-5C4C-4F6D-92F7-0A5CDA038FCD")]
        [InlineData("6734cd2c-215b-4f4c-87c0-363ecac6b9c1")]
        [InlineData("2a6e290d-5c4c-4f6d-92f7-0a5cda038fcd")]
        [InlineData("{A04BE0E5-D9EE-4188-993B-899FF82A2B68}")]
        [InlineData("{BF114C72-618C-48EC-98FE-F2804256A280}")]
        [InlineData("(BF114C72-618C-48EC-98FE-F2804256A280)")]
        public void Retrieve_correct_value(string value)
        {
            var retriever = new GuidValueRetriever();
            var result = (Guid)retriever.Retrieve(new KeyValuePair<string, string>(IrrelevantKey, value), IrrelevantType, typeof(Guid));
            result.Should().Be(new Guid(value));
        }

        [Theory]
        [InlineData("1", "10000000-0000-0000-0000-000000000000")]
        [InlineData("2", "20000000-0000-0000-0000-000000000000")]
        [InlineData("F", "F0000000-0000-0000-0000-000000000000")]
        [InlineData("12", "12000000-0000-0000-0000-000000000000")]
        [InlineData("23", "23000000-0000-0000-0000-000000000000")]
        [InlineData("DF", "DF000000-0000-0000-0000-000000000000")]
        [InlineData("12000000-0000-0000-0000-00000000001", "12000000-0000-0000-0000-000000000010")]
        [InlineData("23000000-0000-0000-0000-00000000009", "23000000-0000-0000-0000-000000000090")]
        [InlineData("DF000000-0000-0000-0000-00000000007", "DF000000-0000-0000-0000-000000000070")]
        [InlineData("1200000000000000000000000000001", "12000000-0000-0000-0000-000000000010")]
        [InlineData("2300000000000000000000000000009", "23000000-0000-0000-0000-000000000090")]
        [InlineData("DF00000000000000000000000000007", "DF000000-0000-0000-0000-000000000070")]
        [InlineData("123456789", "12345678-9000-0000-0000-000000000000")]
        [InlineData("121212123", "12121212-3000-0000-0000-000000000000")]
        [InlineData("xxxxx", "00000000-0000-0000-0000-000000000000")]
        [InlineData(null, "00000000-0000-0000-0000-000000000000")]
        [InlineData("", "00000000-0000-0000-0000-000000000000")]
        public void Retrieve_expected_value(string value, string expectation)
        {
            var retriever = new GuidValueRetriever();
            var result = (Guid)retriever.Retrieve(new KeyValuePair<string, string>(IrrelevantKey, value), IrrelevantType, typeof(Guid));
            result.Should().Be(new Guid(expectation));
        }

        [Theory]
        [InlineData("6734CD2C-215B-4F4C-87C0-363ECAC6B9C1")]
        [InlineData("2A6E290D-5C4C-4F6D-92F7-0A5CDA038FCD")]
        [InlineData("6734cd2c-215b-4f4c-87c0-363ecac6b9c1")]
        [InlineData("2a6e290d-5c4c-4f6d-92f7-0a5cda038fcd")]
        [InlineData("{A04BE0E5-D9EE-4188-993B-899FF82A2B68}")]
        [InlineData("{BF114C72-618C-48EC-98FE-F2804256A280}")]
        [InlineData("(BF114C72-618C-48EC-98FE-F2804256A280)")]
        public void Retrieve_correct_nullable_value(string value)
        {
            var retriever = new GuidValueRetriever();
            var result = (Guid?)retriever.Retrieve(new KeyValuePair<string, string>(IrrelevantKey, value), IrrelevantType, typeof(Guid?));
            result.Should().Be(new Guid(value));
        }

        [Theory]
        [InlineData("1", "10000000-0000-0000-0000-000000000000")]
        [InlineData("2", "20000000-0000-0000-0000-000000000000")]
        [InlineData("F", "F0000000-0000-0000-0000-000000000000")]
        [InlineData("12", "12000000-0000-0000-0000-000000000000")]
        [InlineData("23", "23000000-0000-0000-0000-000000000000")]
        [InlineData("DF", "DF000000-0000-0000-0000-000000000000")]
        [InlineData("12000000-0000-0000-0000-00000000001", "12000000-0000-0000-0000-000000000010")]
        [InlineData("23000000-0000-0000-0000-00000000009", "23000000-0000-0000-0000-000000000090")]
        [InlineData("DF000000-0000-0000-0000-00000000007", "DF000000-0000-0000-0000-000000000070")]
        [InlineData("1200000000000000000000000000001", "12000000-0000-0000-0000-000000000010")]
        [InlineData("2300000000000000000000000000009", "23000000-0000-0000-0000-000000000090")]
        [InlineData("DF00000000000000000000000000007", "DF000000-0000-0000-0000-000000000070")]
        [InlineData("123456789", "12345678-9000-0000-0000-000000000000")]
        [InlineData("121212123", "12121212-3000-0000-0000-000000000000")]
        [InlineData("12121212-3", "12121212-3000-0000-0000-000000000000")]
        [InlineData("xxxxx", "00000000-0000-0000-0000-000000000000")]
        public void Retrieve_expected_nullable_value(string value, string expectation)
        {
            var retriever = new GuidValueRetriever();
            var result = (Guid?)retriever.Retrieve(new KeyValuePair<string, string>(IrrelevantKey, value), IrrelevantType, typeof(Guid?));
            result.Should().Be(new Guid(expectation));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Retrieve_null_for_nullable_value(string value)
        {
            var retriever = new GuidValueRetriever();
            var result = (Guid?)retriever.Retrieve(new KeyValuePair<string, string>(IrrelevantKey, value), IrrelevantType, typeof(Guid?));
            result.Should().BeNull();
        }
    }
}