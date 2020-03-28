using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using FluentAssertions;
using Xunit;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    public class UriValueRetrieverTests
    {
        private const string IrrelevantKey = "Irrelevant";
        private readonly Type IrrelevantType = typeof(object);

        [Theory]
        [InlineData(typeof(Uri), true)]
        [InlineData(typeof(int), false)]
        [InlineData(typeof(object), false)]
        [InlineData(typeof(UriValueRetrieverTests), false)]
        public void CanRetrieve(Type type, bool expectation)
        {
            var retriever = new UriValueRetriever();
            var result = retriever.CanRetrieve(new KeyValuePair<string, string>(IrrelevantKey, IrrelevantKey), IrrelevantType, type);
            result.Should().Be(expectation);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" \t ")]
        public void Retrieves_empty_URI(string value)
        {
            var retriever = new UriValueRetriever();
            var result = (Uri)retriever.Retrieve(new KeyValuePair<string, string>(IrrelevantKey, value), IrrelevantType, typeof(Uri));

            result.Should().NotBeNull();
            result.IsAbsoluteUri.Should().BeFalse();
            result.OriginalString.Should().Be(value);
        }

        [Fact]
        public void Retrieves_absolute_URI()
        {
            const string value = "https://github.com/techtalk/SpecFlow";
            var retriever = new UriValueRetriever();

            var result = (Uri)retriever.Retrieve(new KeyValuePair<string, string>(IrrelevantKey, value), IrrelevantType, typeof(Uri));

            result.IsAbsoluteUri.Should().BeTrue();
            result.AbsoluteUri.Should().Be(value);
        }

        [SkippableTheory]
        [InlineData("/techtalk/SpecFlow")]
        [InlineData("techtalk/SpecFlow")]
        public void Retrieves_relative_URI_Windows(string value)
        {
            Skip.IfNot(RuntimeInformation.IsOSPlatform(OSPlatform.Windows));

            var retriever = new UriValueRetriever();

            var result = (Uri)retriever.Retrieve(new KeyValuePair<string, string>(IrrelevantKey, value), IrrelevantType, typeof(Uri));

            result.IsAbsoluteUri.Should().BeFalse();
            result.OriginalString.Should().Be(value);
        }

        [SkippableTheory]
        [InlineData("techtalk/SpecFlow")]
        public void Retrieves_relative_URI_Unix(string value)
        {
            Skip.IfNot(RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX));

            var retriever = new UriValueRetriever();

            var result = (Uri)retriever.Retrieve(new KeyValuePair<string, string>(IrrelevantKey, value), IrrelevantType, typeof(Uri));

            result.IsAbsoluteUri.Should().BeFalse();
            result.OriginalString.Should().Be(value);
        }
    }
}