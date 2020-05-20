using System;
using System.Collections.Generic;
using Xunit;
using FluentAssertions;
using TechTalk.SpecFlow.Assist.ValueRetrievers;
using TechTalk.SpecFlow.RuntimeTests.AssistTests.ExampleEntities;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    
    public class EnumValueRetrieverTests
    {
        private const string IrrelevantKey = "Irrelevant";
        private readonly Type IrrelevantType = typeof(object);

        [Theory]
        [InlineData(typeof(Sex), true)]
        [InlineData(typeof(Sex?), true)]
        [InlineData(typeof(int), false)]
        public void CanRetrieve(Type type, bool expectation)
        {
            var retriever = new EnumValueRetriever();
            var result = retriever.CanRetrieve(new KeyValuePair<string, string>(IrrelevantKey, IrrelevantKey), IrrelevantType, type);
            result.Should().Be(expectation);
        }

        [Theory]
        [InlineData("Male", Sex.Male)]
        [InlineData("Unknown Sex", Sex.UnknownSex)]
        [InlineData("feMale", Sex.Female)]
        [InlineData("unknown sex", Sex.UnknownSex)]
        public void Retrieve_correct_value(string value, Sex expectation)
        {
            var retriever = new EnumValueRetriever();
            var result = (Sex)retriever.Retrieve(new KeyValuePair<string, string>(IrrelevantKey, value), IrrelevantType, typeof(Sex));
            result.Should().Be(expectation);
        }

        [Theory]
        [InlineData("", "No enum with value {empty} found")]
        [InlineData(null, "No enum with value {null} found")]
        [InlineData("NotDefinied", "No enum with value NotDefinied found")]
        public void Throws_an_exception_when_the_value_is_illegal(string value, string exceptionMessage)
        {
            var retriever = new EnumValueRetriever();

            Action action = () => retriever.Retrieve(new KeyValuePair<string, string>(IrrelevantKey, value), IrrelevantType, typeof(Sex));

            action.Should().Throw<InvalidOperationException>().WithMessage(exceptionMessage);
        }

        [Theory]
        [InlineData("Male", Sex.Male)]
        [InlineData("Unknown Sex", Sex.UnknownSex)]
        [InlineData("feMale", Sex.Female)]
        [InlineData("unknown sex", Sex.UnknownSex)]
        [InlineData("", null)]
        [InlineData(null, null)]
        public void Retrieve_correct_nullable_value(string value, Sex? expectation)
        {
            var retriever = new EnumValueRetriever();
            var result = (Sex?)retriever.Retrieve(new KeyValuePair<string, string>(IrrelevantKey, value), IrrelevantType, typeof(Sex?));
            result.Should().Be(expectation);
        }

        [Theory]
        [InlineData("NotDefinied", "No enum with value NotDefinied found")]
        public void Throws_an_exception_when_the_value_is_illegal_for_nullable(string value, string exceptionMessage)
        {
            var retriever = new EnumValueRetriever();

            Action action = () => retriever.Retrieve(new KeyValuePair<string, string>(IrrelevantKey, value), IrrelevantType, typeof(Sex?));

            action.Should().Throw<InvalidOperationException>().WithMessage(exceptionMessage);
        }

    }
}