using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using TechTalk.SpecFlow.Utils;

namespace UtilsTests
{
    [TestFixture]
    public class RegexSamplerTests
    {
        [Test]
        public void Should_substitute_param_names()
        {
            var result = RegexSampler.GetRegexSample(@"bla (.*) foo (.*) bar", new[] {"p1", "p2"});

            Assert.AreEqual("bla <p1> foo <p2> bar", result);
        }

        [Test]
        public void Should_handle_invalid_param_count()
        {
            var result = RegexSampler.GetRegexSample(@"bla (.*) foo (.*) bar", new[] {"p1"});

            Assert.AreEqual("bla <p1> foo <param> bar", result);
        }

        [Test]
        public void Should_handle_nested_gorups()
        {
            var result = RegexSampler.GetRegexSample(@"bla (a(.*)b) foo (.*) bar", new[] { "p1", "p2" });

            Assert.AreEqual("bla <p1> foo <p2> bar", result);
        }

        [Test]
        public void Should_handle_non_capturing_gorups()
        {
            var result = RegexSampler.GetRegexSample(@"bla (?:.*) foo (.*) bar", new[] { "p1" });

            StringAssert.EndsWith(" foo <p1> bar", result);
        }

        [Test]
        public void Should_handle_questionmark_for_gorups()
        {
            var result = RegexSampler.GetRegexSample(@"bla (?:hello )?foo (.*) bar", new[] { "p1" });

            StringAssert.StartsWith("bla foo", result);
        }

        [Test]
        public void Should_handle_questionmark_for_chars()
        {
            var result = RegexSampler.GetRegexSample(@"bla x?foo (.*) bar", new[] { "p1" });

            StringAssert.StartsWith("bla foo", result);
        }

        [Test]
        public void Should_handle_star_for_chars()
        {
            var result = RegexSampler.GetRegexSample(@"bla x*foo (.*) bar", new[] { "p1" });

            StringAssert.StartsWith("bla foo", result);
        }

        [Test]
        public void Should_handle_star_for_gorups()
        {
            var result = RegexSampler.GetRegexSample(@"bla (?:hello )*foo (.*) bar", new[] { "p1" });

            StringAssert.StartsWith("bla foo", result);
        }

        [Test]
        public void Should_handle_complex_case()
        {
            var result = RegexSampler.GetRegexSample(@"a card for an? ((?:actor-goal)|(?:user story)|(?:business goal)) '([^']*)' on the workspace", new[] { "requirementType", "key" });

            Assert.AreEqual("a card for a <requirementType> '<key>' on the workspace", result);
        }

        [Test]
        public void Should_handle_dot_star()
        {
            var result = RegexSampler.GetRegexSample(@".*", new string[0]);

            StringAssert.StartsWith(@".*", result);
        }

        [Test]
        public void Should_handle_plus_for_chars()
        {
            var result = RegexSampler.GetRegexSample(@"bla +foo (.*) bar", new[] { "p1" });

            StringAssert.StartsWith("bla foo", result);
        }

        [Test]
        public void Should_handle_param_inside_non_capturing_gorups()
        {
            var result = RegexSampler.GetRegexSample(@"bla (?:x(.*)y) foo", new[] { "p1" });

            Assert.AreEqual("bla x<p1>y foo", result);
        }
    }
}
