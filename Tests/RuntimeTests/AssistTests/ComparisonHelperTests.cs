using System;
using NUnit.Framework;
using Should;
using TechTalk.SpecFlow.Assist;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests
{
    [TestFixture]
    public class ComparisonHelperTests
    {
        [Test]
        public void Throws_exception_when_value_of_matching_string_property_does_not_match()
        {
            var table = new Table("Field", "Value");
            table.AddRow("StringProperty", "Howard Roark");

            var test = new ComparisonTest {StringProperty = "Peter Keating"};

            var exceptionThrown = ExceptionWasThrownByThisComparison(table, test);

            exceptionThrown.ShouldBeTrue();
        }

        [Test]
        public void Does_not_throw_exception_when_value_of_matching_string_property_matches()
        {
            var table = new Table("Field", "Value");
            table.AddRow("StringProperty", "Howard Roark");

            var test = new ComparisonTest { StringProperty = "Howard Roark" };

            var exception = ExceptionWasThrownByThisComparison(table, test);

            exception.ShouldBeFalse();
        }

        private static bool ExceptionWasThrownByThisComparison(Table table, ComparisonTest test)
        {
            var exception = false;
            try
            {
                table.CompareToInstance(test);
            }
            catch (ComparisonException ex)
            {
                exception = true;
            }
            return exception;
        }
    }

    public class ComparisonTest
    {
        public string StringProperty { get; set; }
    }
}