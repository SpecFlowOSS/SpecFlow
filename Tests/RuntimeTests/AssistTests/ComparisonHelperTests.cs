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

            var test = new ComparisonTest {StringProperty = "Howard Roark"};

            var exception = ExceptionWasThrownByThisComparison(table, test);

            exception.ShouldBeFalse();
        }

        [Test]
        public void Throws_exception_when_first_row_matches_but_second_does_not()
        {
            var table = new Table("Field", "Value");
            table.AddRow("StringProperty", "Howard Roark");
            table.AddRow("IntProperty", "10");

            var test = new ComparisonTest
                           {
                               StringProperty = "Howard Roark",
                               IntProperty = 10
                           };

            var exceptionThrown = ExceptionWasThrownByThisComparison(table, test);

            exceptionThrown.ShouldBeTrue();
        }

        [Test]
        public void Exception_returns_an_exception_for_one_error_when_there_is_one_difference()
        {
            var table = new Table("Field", "Value");
            table.AddRow("StringProperty", "Howard Roark");

            var test = new ComparisonTest {StringProperty = "Peter Keating"};

            var exception = GetExceptionThrownByThisComparison(table, test);

            exception.Message.ShouldEqual(
                @"The following fields did not match:
StringProperty: Expected <Howard Roark>, Actual <Peter Keating>");
        }

        [Test]
        public void Exception_returns_an_exception_for_two_errors_when_there_are_two_differences()
        {
            var table = new Table("Field", "Value");
            table.AddRow("StringProperty", "Howard Roark");
            table.AddRow("IntProperty", "1");

            var test = new ComparisonTest
                           {
                               StringProperty = "Peter Keating",
                               IntProperty = 2
                           };

            var exception = GetExceptionThrownByThisComparison(table, test);

            exception.Message.ShouldEqual(
                @"The following fields did not match:
StringProperty: Expected <Howard Roark>, Actual <Peter Keating>
IntProperty: Expected <1>, Actual <2>");
        }

        private static ComparisonException GetExceptionThrownByThisComparison(Table table, ComparisonTest test)
        {
            try
            {
                table.CompareToInstance(test);
            }
            catch (ComparisonException ex)
            {
                return ex;
            }
            return null;
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
        public int IntProperty { get; set; }
    }
}