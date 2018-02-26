using System;
using Xunit;
using FluentAssertions;
using TechTalk.SpecFlow.Assist.ValueComparers;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueComparerTests
{
    
    public class GuidValueComparerTests
    {
        [Fact]
        public void Can_compare_if_the_value_is_a_guid()
        {
            var valueComparer = CreateComparer();
            valueComparer.CanCompare(new Guid()).Should().BeTrue();
            valueComparer.CanCompare(new Guid("{6EFE6DA0-A8B4-4E71-8329-49819C409227}"))
                .Should().BeTrue();
        }

        [Fact]
        public void Cannot_compare_if_the_value_is_null()
        {
            CreateComparer()
                .CanCompare(null)
                .Should().BeFalse();
        }

        [Fact]
        public void Cannot_compare_if_the_value_is_not_a_guid()
        {
            var valueComparer = CreateComparer();
            valueComparer.CanCompare("John Galt").Should().BeFalse();
            valueComparer.CanCompare(1).Should().BeFalse();
            valueComparer.CanCompare(3.14).Should().BeFalse();
        }

        [Fact]
        public void Returns_true_when_the_value_and_the_string_match()
        {
            var valueComparer = CreateComparer();
            valueComparer.Compare("A5C82A02-4A2F-4DEE-AB4A-E829E7B476B3",
                                                new Guid("A5C82A02-4A2F-4DEE-AB4A-E829E7B476B3"))
                .Should().BeTrue();
            valueComparer.Compare("D237B442-8364-4C07-AE13-99FFD55F729B",
                                                new Guid("D237B442-8364-4C07-AE13-99FFD55F729B"))
                .Should().BeTrue();
        }

        [Fact]
        public void Returns_false_when_the_value_and_the_string_do_not_match()
        {
            var valueComparer = CreateComparer();
            valueComparer.Compare("B5C82A02-4A2F-4DEE-AB4A-E829E7B476B3",
                                                new Guid("A5C82A02-4A2F-4DEE-AB4A-E829E7B476B3"))
                .Should().BeFalse();
            valueComparer.Compare("D237B442-8364-4C07-AE13-99FFD55F729C",
                                                new Guid("D237B442-8364-4C07-AE13-99FFD55F729B"))
                .Should().BeFalse();
        }

        [Fact]
        public void Returns_true_even_if_the_expected_value_is_wrapped_in_curly_braces()
        {
            var valueComparer = CreateComparer();
            valueComparer.Compare("{5C50F10A-87C7-4A6E-B772-8055317A39B8}",
                                                new Guid("{5C50F10A-87C7-4A6E-B772-8055317A39B8}"))
                .Should().BeTrue();
            valueComparer.Compare("{A44604A1-0144-4AA1-B4EC-3B1117C1127D}",
                                                new Guid("{A44604A1-0144-4AA1-B4EC-3B1117C1127D}"))
                .Should().BeTrue();
        }

        [Fact]
        public void Returns_false_if_the_expected_value_is_not_a_valid_guid()
        {
            var valueComparer = CreateComparer();
            valueComparer.Compare("x", new Guid())
                .Should().BeFalse();
            valueComparer.Compare("g234", new Guid("{4CD16C19-9A8A-4B2B-BA8C-2D3985EBD292}"))
                .Should().BeFalse();
        }

        [Fact]
        public void Returns_true_if_the_expected_value_is_0_and_the_expected_is_an_empty_guid()
        {
            var valueComparer = CreateComparer();
            valueComparer.Compare("0", new Guid())
                .Should().BeTrue();
        }

        [Fact]
        public void Matches_regardless_of_casing()
        {
            var valueComparer = CreateComparer();
            valueComparer.Compare("{767c5221-ed1e-4e6b-9028-6b77b5195d56}",
                                                new Guid("{767C5221-ED1E-4E6B-9028-6B77B5195D56}"))
                .Should().BeTrue();
            valueComparer.Compare("{43BA4A14-C65C-4D47-9A83-6D36E03F3576}",
                                                new Guid("{43ba4a14-C65c-4D47-9a83-6d36e03f3576}"))
                .Should().BeTrue();
        }

        [Fact]
        public void Matches_based_on_the_first_eight_digits_when_the_rest_are_zeroes()
        {
            var valueComparer = CreateComparer();
            valueComparer.Compare("B6F8CA06",
                                                new Guid("B6F8CA06-0000-0000-0000-000000000000"))
                .Should().BeTrue();

            valueComparer.Compare("35E9525C",
                                                new Guid("35E9525C-0000-0000-0000-000000000000"))
                .Should().BeTrue();
        }

        [Fact]
        public void Matches_based_on_the_first_nine_digits_when_the_rest_are_zeroes()
        {
            var valueComparer = CreateComparer();
            valueComparer.Compare("B6F8CA067",
                                                new Guid("B6F8CA06-7000-0000-0000-000000000000"))
                .Should().BeTrue();

            valueComparer.Compare("35E9525CA",
                                                new Guid("35E9525C-A000-0000-0000-000000000000"))
                .Should().BeTrue();
        }

        private static GuidValueComparer CreateComparer()
        {
            return new GuidValueComparer(new GuidValueRetriever());
        }
    }
}