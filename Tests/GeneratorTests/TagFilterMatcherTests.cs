using System;
using System.Linq;
using NUnit.Framework;
using TechTalk.SpecFlow.Generator.UnitTestConverter;
using TechTalk.SpecFlow.Parser.SyntaxElements;
using FluentAssertions;

namespace TechTalk.SpecFlow.GeneratorTests
{
    [TestFixture]
    public class TagFilterMatcherTests
    {
        [Test]
        public void Should_match_simple_tag()
        {
            var matcher = new TagFilterMatcher();

            var tags = new string[] {"mytag"};

            matcher.MatchPrefix("mytag", tags).Should().BeTrue();
        }

        [Test]
        public void Should_match_tag_case_insensitive()
        {
            var matcher = new TagFilterMatcher();

            var tags = new string[] {"MyTag"};

            matcher.MatchPrefix("mytag", tags).Should().BeTrue();
        }

        [Test]
        public void Should_not_match_not_included_tag()
        {
            var matcher = new TagFilterMatcher();

            var tags = new string[] {"othertag"};

            matcher.MatchPrefix("mytag", tags).Should().BeFalse();
        }

        [Test]
        public void Should_not_match_empty_tag_list()
        {
            var matcher = new TagFilterMatcher();

            var tags = new string[0];

            matcher.MatchPrefix("mytag", tags).Should().BeFalse();
        }

        [Test]
        public void Should_not_match_null_tag_list()
        {
            var matcher = new TagFilterMatcher();

            string[] tags = null;

            matcher.MatchPrefix("mytag", tags).Should().BeFalse();
        }

        [Test]
        public void Should_match_simple_tag_with_at()
        {
            var matcher = new TagFilterMatcher();

            var tags = new string[] {"mytag"};

            matcher.MatchPrefix("@mytag", tags).Should().BeTrue();
        }

        [Test]
        public void Should_match_tag_prefix()
        {
            var matcher = new TagFilterMatcher();

            var tags = new string[] {"mytag:foo"};

            matcher.MatchPrefix("mytag", tags).Should().BeTrue();
        }

        [Test]
        public void Should_match_tag_prefix_with_at()
        {
            var matcher = new TagFilterMatcher();

            var tags = new string[] {"mytag:foo"};

            matcher.MatchPrefix("@mytag", tags).Should().BeTrue();
        }

        [Test]
        public void Should_GetTagValue_return_prefixed_value()
        {
            var matcher = new TagFilterMatcher();

            var tags = new string[] {"mytag:foo"};

            string value;
            matcher.GetTagValue("@mytag", tags, out value).Should().Be(true);
            value.Should().Be("foo");
        }

        [Test]
        public void Should_GetTagValue_returns_false_when_null_tag_list()
        {
            var matcher = new TagFilterMatcher();

            string[] tags = null;

            string value;
            matcher.GetTagValue("@mytag", tags, out value).Should().Be(false);
        }

        [Test]
        public void Should_GetTagValue_returns_false_when_no_match()
        {
            var matcher = new TagFilterMatcher();

            var tags = new string[] {"othertag"};

            string value;
            matcher.GetTagValue("@mytag", tags, out value).Should().Be(false);
        }

        [Test]
        public void Should_GetTagValue_returns_empty_when_exact_match()
        {
            var matcher = new TagFilterMatcher();

            var tags = new string[] {"mytag"};

            string value;
            matcher.GetTagValue("@mytag", tags, out value).Should().Be(true);
            value.Should().Be("");
        }

        [Test]
        public void Should_GetTagValues_returns_prefixed_values()
        {
            var matcher = new TagFilterMatcher();

            var tags = new string[] {"mytag:foo", "mytag:bar"};

            var values = matcher.GetTagValues("mytag", tags);
            values.Should().Contain("foo");
            values.Should().Contain("bar");
        }

        [Test]
        public void Should_GetTagValues_returns_empty_list_when_no_match()
        {
            var matcher = new TagFilterMatcher();

            var tags = new string[] {"othertag"};

            var values = matcher.GetTagValues("mytag", tags);
            values.Should().BeEmpty();
        }

        [Test]
        public void Should_GetTagValues_returns_list_with_empty_string_for_exact_mathces()
        {
            var matcher = new TagFilterMatcher();

            var tags = new string[] {"mytag:foo", "mytag"};

            var values = matcher.GetTagValues("mytag", tags);
            values.Should().Contain("foo");
            values.Should().Contain("");
        }

    }
}
