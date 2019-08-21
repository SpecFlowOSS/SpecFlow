using System;
using FluentAssertions;
using Xunit;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    public class UriValueRetrieverTests
    {
        [Theory]
        [InlineData("")]
        [InlineData(" \t ")]
        public void Retrieves_empty_URI(string emptyUri)
        {
            var retriever = new UriValueRetriever();

            var actual = retriever.GetValue(emptyUri);

            actual.Should().NotBeNull();
            actual.IsAbsoluteUri.Should().BeFalse();
            actual.OriginalString.Should().Be(emptyUri);
        }

        [Fact]
        public void Retrieves_absolute_URI()
        {
            const string expectedUri = "https://github.com/techtalk/SpecFlow";
            var retriever = new UriValueRetriever();

            var actual = retriever.GetValue(expectedUri);

            actual.IsAbsoluteUri.Should().BeTrue();
            actual.AbsoluteUri.Should().Be(expectedUri);
        }

        [SkippableTheory]
        [InlineData("/techtalk/SpecFlow")]
        [InlineData("techtalk/SpecFlow")]
        public void Retrieves_relative_URI_Windows(string expectedUri)
        {
            Skip.IfNot(Environment.OSVersion.Platform == PlatformID.Win32NT);

            var retriever = new UriValueRetriever();

            var actual = retriever.GetValue(expectedUri);

            actual.IsAbsoluteUri.Should().BeFalse();
            actual.OriginalString.Should().Be(expectedUri);
        }

        [SkippableTheory]
        [InlineData("techtalk/SpecFlow")]
        public void Retrieves_relative_URI_Unix(string expectedUri)
        {
            Skip.If(Environment.OSVersion.Platform == PlatformID.Win32NT);

            var retriever = new UriValueRetriever();

            var actual = retriever.GetValue(expectedUri);

            actual.IsAbsoluteUri.Should().BeFalse();
            actual.OriginalString.Should().Be(expectedUri);
        }
    }
}