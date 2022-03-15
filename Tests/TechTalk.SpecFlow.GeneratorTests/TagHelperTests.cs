using System;
using FluentAssertions;
using Xunit;

namespace TechTalk.SpecFlow.GeneratorTests
{
    public class TagHelperTests
    {
        public static object[][] ContainsIgnoreTag_Examples_Returns_Expected_Data = new[]
        {
            new object[] { new[] { "ignore" }, true },
            new object[] { new[] { "Ignore" }, true },
            new object[] { new[] { "IGNORE" }, true },
            new object[] { new[] { "anything", "ignore" }, true },
            new object[] { null, false },
            new object[] { new[] { "ignored" }, false },
            new object[] { Array.Empty<string>(), false },
        };

        [Theory]
        [MemberData(nameof(ContainsIgnoreTag_Examples_Returns_Expected_Data))]
        public void ContainsIgnoreTag_Examples_Returns_Expected(string[] tags, bool expected)
        {
            bool actual = TagHelper.ContainsIgnoreTag(tags);
            actual.Should().Be(expected);
        }
    }
}
