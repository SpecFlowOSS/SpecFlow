using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using NUnit.Framework;
using TechTalk.SpecFlow.BindingSkeletons;
using FluentAssertions;

namespace TechTalk.SpecFlow.RuntimeTests.BindingSkeletons
{
    [TestFixture]
    public class StepTextAnalyzerTests
    {
        private readonly CultureInfo bindingCulture = new CultureInfo("en-US");

        [Test]
        public void Should_not_change_simple_step()
        {
            var sut = new StepTextAnalyzer();

            var result = sut.Analyze("I do something", bindingCulture);
            result.Parameters.Should().BeEmpty();
            result.TextParts.Count.Should().Be(1);
            result.TextParts[0].Should().Be("I do something");
        }

        [Test]
        public void Should_recognize_quoted_strings()
        {
            var sut = new StepTextAnalyzer();

            var result = sut.Analyze("I \"do\" something", bindingCulture);
            result.Parameters.Count.Should().Be(1);
            result.TextParts.Count.Should().Be(2);
            result.TextParts[0].Should().Be("I \"");
            result.TextParts[1].Should().Be("\" something");
        }

        [Test]
        public void Should_recognize_apostrophed_strings()
        {
            var sut = new StepTextAnalyzer();

            var result = sut.Analyze("I 'do' something", bindingCulture);
            result.Parameters.Count.Should().Be(1);
            result.TextParts.Count.Should().Be(2);
            result.TextParts[0].Should().Be("I '");
            result.TextParts[1].Should().Be("' something");
        }

        [Test]
        public void Should_recognize_angle_bracket_strings()
        {
            var sut = new StepTextAnalyzer();

            var result = sut.Analyze("I <do> something", bindingCulture);
            result.Parameters.Count.Should().Be(1);
            result.TextParts.Count.Should().Be(2);
            result.TextParts[0].Should().Be("I ");
            result.TextParts[1].Should().Be(" something");
        }

        [Test]
        public void Should_handle_quote_overlaps()
        {
            var sut = new StepTextAnalyzer();

            var result = sut.Analyze("I 'do \" something' really \" strange", bindingCulture);
            result.Parameters.Count.Should().Be(1);
            result.TextParts.Count.Should().Be(2);
            result.TextParts[0].Should().Be("I '");
            result.TextParts[1].Should().Be("' really \" strange");
        }

        [Test]
        public void Should_handle_overlaps_with_numbers()
        {
            var sut = new StepTextAnalyzer();

            var result = sut.Analyze("I 'do 42 something' foo", bindingCulture);
            result.Parameters.Count.Should().Be(1);
            result.TextParts.Count.Should().Be(2);
            result.TextParts[0].Should().Be("I '");
            result.TextParts[1].Should().Be("' foo");
        }

        [Test]
        public void Should_recognize_integers()
        {
            var sut = new StepTextAnalyzer();

            var result = sut.Analyze("I have 42 bars", bindingCulture);
            result.Parameters.Count.Should().Be(1);
            result.TextParts.Count.Should().Be(2);
            result.TextParts[0].Should().Be("I have ");
            result.TextParts[1].Should().Be(" bars");
            result.Parameters[0].Type.Should().Be("Int32");
        }

        [Test]
        public void Should_recognize_decimals()
        {
            var sut = new StepTextAnalyzer();

            var result = sut.Analyze("I have 4.2 bars", bindingCulture);
            result.Parameters.Count.Should().Be(1);
            result.TextParts.Count.Should().Be(2);
            result.TextParts[0].Should().Be("I have ");
            result.TextParts[1].Should().Be(" bars");
            result.Parameters[0].Type.Should().Be("Decimal");
        }
    }
}
