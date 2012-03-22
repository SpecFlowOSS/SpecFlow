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

            Feature theFeature = new Feature();
            theFeature.Tags = new Tags(new Tag("mytag"));

            matcher.Match("mytag", theFeature).Should().BeTrue();
        }

        [Test]
        public void Should_not_match_not_included_tag()
        {
            var matcher = new TagFilterMatcher();

            Feature theFeature = new Feature();
            theFeature.Tags = new Tags(new Tag("othertag"));

            matcher.Match("mytag", theFeature).Should().BeFalse();
        }

        [Test]
        public void Should_not_match_empty_tag_list()
        {
            var matcher = new TagFilterMatcher();

            Feature theFeature = new Feature();
            theFeature.Tags = new Tags();

            matcher.Match("mytag", theFeature).Should().BeFalse();
        }

        [Test]
        public void Should_not_match_null_tag_list()
        {
            var matcher = new TagFilterMatcher();

            Feature theFeature = new Feature();
            theFeature.Tags = null;

            matcher.Match("mytag", theFeature).Should().BeFalse();
        }

        [Test]
        public void Should_match_simple_tag_with_at()
        {
            var matcher = new TagFilterMatcher();

            Feature theFeature = new Feature();
            theFeature.Tags = new Tags(new Tag("mytag"));

            matcher.Match("@mytag", theFeature).Should().BeTrue();
        }

        [Test]
        public void Should_match_tag_prefix()
        {
            var matcher = new TagFilterMatcher();

            Feature theFeature = new Feature();
            theFeature.Tags = new Tags(new Tag("mytag:foo"));

            matcher.Match("mytag", theFeature).Should().BeTrue();
        }

        [Test]
        public void Should_match_tag_prefix_with_at()
        {
            var matcher = new TagFilterMatcher();

            Feature theFeature = new Feature();
            theFeature.Tags = new Tags(new Tag("mytag:foo"));

            matcher.Match("@mytag", theFeature).Should().BeTrue();
        }
    }
}
