using System;
using FluentAssertions;
using TechTalk.SpecFlow.Assist.ValueRetrievers;
using Xunit;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    public class NullValueRetrieverTests
    {
        private const string TestNullValue = "MyNullValue";

        [Theory]
        [InlineData("MyNullValue")]
        [InlineData(" \tMyNullValue\t ")]
        [InlineData("MyNuLLValue")]
        [InlineData("\tMyNuLLValue\t")]
        public void Can_retrieve_null_value(string value)
        {
            var retriever = CreateTestee();

            var actual = retriever.CanRetrieve<object>(value);

            actual.Should().BeTrue();
        }

        [Theory]
        [InlineData(typeof(object))]
        [InlineData(typeof(bool?))]
        public void Can_retrieve_nullable_types(Type propertyType)
        {
            var retriever = CreateTestee();

            var actual = retriever.CanRetrieve(TestNullValue, propertyType);

            actual.Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("Any string")]
        [InlineData("30.0")]
        [InlineData("true")]
        [InlineData(" \t ")]
        public void Cannot_retrieve_non_null_values(string value)
        {
            var retriever = CreateTestee();

            var actual = retriever.CanRetrieve<object>(value);

            actual.Should().BeFalse();
        }

        [Theory]
        [InlineData(typeof(bool))]
        [InlineData(typeof(Guid))]
        [InlineData(typeof(StringSplitOptions))]
        public void Cannot_retrieve_non_nullable_types(Type propertyType)
        {
            var retriever = CreateTestee();

            var actual = retriever.CanRetrieve(TestNullValue, propertyType);

            actual.Should().BeFalse();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("Any string")]
        [InlineData("30.0")]
        [InlineData("true")]
        [InlineData(" \t ")]
        [InlineData(" \tMyNullValue\t ")]
        [InlineData("MyNuLLValue")]
        public void Retrieves_null_for_any_value(string value)
        {
            var retriever = CreateTestee();

            var actual = retriever.GetValue<object>(value);

            actual.Should().BeNull();
        }

        [Theory]
        [InlineData(typeof(object))]
        [InlineData(typeof(bool?))]
        [InlineData(typeof(bool))]
        [InlineData(typeof(Guid))]
        [InlineData(typeof(StringSplitOptions))]
        public void Retrieves_null_for_any_type(Type propertyType)
        {
            var retriever = CreateTestee();

            var actual = retriever.GetValue("Any value", propertyType);

            actual.Should().BeNull();
        }

        private NullValueRetriever CreateTestee()
        {
            return new NullValueRetriever(TestNullValue);
        }
    }
}
