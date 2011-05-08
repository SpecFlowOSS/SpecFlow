using System;
using NUnit.Framework;
using Should;
using TechTalk.SpecFlow.Assist.ValueComparers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueComparerTests
{
    [TestFixture]
    public class GuidValueComparerTests
    {
        [Test]
        public void Can_compare_if_the_value_is_a_guid()
        {
            var valueComparer = new GuidValueComparer();
            valueComparer.CanCompare(new Guid()).ShouldBeTrue();
            valueComparer.CanCompare(new Guid("{6EFE6DA0-A8B4-4E71-8329-49819C409227}"))
                .ShouldBeTrue();
        }

        [Test]
        public void Cannot_compare_if_the_value_is_null()
        {
            new GuidValueComparer()
                .CanCompare(null)
                .ShouldBeFalse();
        }

        [Test]
        public void Cannot_compare_if_the_value_is_not_a_guid()
        {
            var valueComparer = new GuidValueComparer();
            valueComparer.CanCompare("John Galt").ShouldBeFalse();
            valueComparer.CanCompare(1).ShouldBeFalse();
            valueComparer.CanCompare(3.14).ShouldBeFalse();
        }

        [Test]
        public void Returns_true_when_the_value_and_the_string_match()
        {
            var valueComparer = new GuidValueComparer();
            valueComparer.TheseValuesAreTheSame("A5C82A02-4A2F-4DEE-AB4A-E829E7B476B3",
                                                new Guid("A5C82A02-4A2F-4DEE-AB4A-E829E7B476B3"))
                .ShouldBeTrue();
            valueComparer.TheseValuesAreTheSame("D237B442-8364-4C07-AE13-99FFD55F729B",
                                                new Guid("D237B442-8364-4C07-AE13-99FFD55F729B"))
                .ShouldBeTrue();
        }

        [Test]
        public void Returns_false_when_the_value_and_the_string_do_not_match()
        {
            var valueComparer = new GuidValueComparer();
            valueComparer.TheseValuesAreTheSame("B5C82A02-4A2F-4DEE-AB4A-E829E7B476B3",
                                                new Guid("A5C82A02-4A2F-4DEE-AB4A-E829E7B476B3"))
                .ShouldBeFalse();
            valueComparer.TheseValuesAreTheSame("D237B442-8364-4C07-AE13-99FFD55F729C",
                                                new Guid("D237B442-8364-4C07-AE13-99FFD55F729B"))
                .ShouldBeFalse();
        }

        [Test]
        public void Returns_true_even_if_the_expected_value_is_wrapped_in_curly_braces()
        {
            var valueComparer = new GuidValueComparer();
            valueComparer.TheseValuesAreTheSame("{5C50F10A-87C7-4A6E-B772-8055317A39B8}",
                                                new Guid("{5C50F10A-87C7-4A6E-B772-8055317A39B8}"))
                .ShouldBeTrue();
            valueComparer.TheseValuesAreTheSame("{A44604A1-0144-4AA1-B4EC-3B1117C1127D}",
                                                new Guid("{A44604A1-0144-4AA1-B4EC-3B1117C1127D}"))
                .ShouldBeTrue();
        }

        [Test]
        public void Returns_false_if_the_expected_value_is_not_a_valid_guid()
        {
            var valueComparer = new GuidValueComparer();
            valueComparer.TheseValuesAreTheSame("x", new Guid())
                .ShouldBeFalse();
            valueComparer.TheseValuesAreTheSame("1234", new Guid("{4CD16C19-9A8A-4B2B-BA8C-2D3985EBD292}"))
                .ShouldBeFalse();
        }

        [Test]
        public void Matches_regardless_of_casing()
        {
            var valueComparer = new GuidValueComparer();
            valueComparer.TheseValuesAreTheSame("{767c5221-ed1e-4e6b-9028-6b77b5195d56}",
                                                new Guid("{767C5221-ED1E-4E6B-9028-6B77B5195D56}"))
                .ShouldBeTrue();
            valueComparer.TheseValuesAreTheSame("{43BA4A14-C65C-4D47-9A83-6D36E03F3576}",
                                                new Guid("{43ba4a14-C65c-4D47-9a83-6d36e03f3576}"))
                .ShouldBeTrue();
        }

        [Test]
        public void Matches_based_on_the_first_eight_digits_when_the_rest_are_zeroes()
        {
            var valueComparer = new GuidValueComparer();
            valueComparer.TheseValuesAreTheSame("B6F8CA06",
                                                new Guid("B6F8CA06-0000-0000-0000-000000000000"))
                .ShouldBeTrue();

            valueComparer.TheseValuesAreTheSame("35E9525C",
                                                new Guid("35E9525C-0000-0000-0000-000000000000"))
                .ShouldBeTrue();
        }
    }
}