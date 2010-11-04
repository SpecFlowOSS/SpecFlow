using System;
using NUnit.Framework;
using Should;
using TechTalk.SpecFlow.Assist;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests
{
    [TestFixture]
    public class InstanceComparisonExtensionMethodsTests
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

            var exceptionThrown = ExceptionWasThrownByThisComparison(table, test);

            exceptionThrown.ShouldBeFalse();
        }

        [Test]
        public void Throws_exception_when_first_row_matches_but_second_does_not()
        {
            var table = new Table("Field", "Value");
            table.AddRow("StringProperty", "Howard Roark");
            table.AddRow("IntProperty", "20");

            var test = new ComparisonTest
                           {
                               StringProperty = "Howard Roark",
                               IntProperty = 10
                           };

            var exceptionThrown = ExceptionWasThrownByThisComparison(table, test);

            exceptionThrown.ShouldBeTrue();
        }

        [Test]
        public void Should_not_throw_an_exception_when_the_ToString_values_of_each_match()
        {
            var table = new Table("Field", "Value");
            table.AddRow("StringProperty", "Howard Roark");
            table.AddRow("IntProperty", "10");
            table.AddRow("DateTimeProperty", "12/25/2010 12:00:00 AM");

            var test = new ComparisonTest
            {
                StringProperty = "Howard Roark",
                IntProperty = 10,
                DateTimeProperty = new DateTime(2010, 12, 25)
            };

            var exceptionThrown = ExceptionWasThrownByThisComparison(table, test);

            exceptionThrown.ShouldBeFalse();
        }

        [Test]
        public void Should_not_throw_an_exception_when_actual_value_is_null_and_expected_is_empty()
        {
            var table = new Table("Field", "Value");
            table.AddRow("NullableField", "");

            var test = new ComparisonTest
            {
                NullableField = null,
            };

            var exceptionThrown = ExceptionWasThrownByThisComparison(table, test);

            exceptionThrown.ShouldBeFalse();
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

        [Test]
        public void Exception_returns_a_descriptive_error_when_property_in_table_does_not_exist()
        {
            var table = new Table("Field", "Value");
            table.AddRow("IDoNotExist", "Ok, mister");

            var test = new ComparisonTest();

            var exception = GetExceptionThrownByThisComparison(table, test);

            exception.Message.ShouldEqual(@"The following fields did not match:
IDoNotExist: Property does not exist");
        }

        [Test]
        public void Will_property_handle_true_boolean_matches()
        {
            var table = new Table("Field", "Value");
            table.AddRow("BooleanProperty", "true");

            ExceptionWasThrownByThisComparison(table, new ComparisonTest{
                                                              BooleanProperty = true
                                                          }).ShouldBeFalse();

            ExceptionWasThrownByThisComparison(table, new ComparisonTest
                                                        {
                                                            BooleanProperty = false
                                                        }).ShouldBeTrue();

        }

        [Test]
        public void Throws_exception_when_the_result_is_null()
        {
            var table = new Table("Field", "Value");

            var exceptionThrown = ExceptionWasThrownByThisComparison(table, null);

            exceptionThrown.ShouldBeTrue();
        }

        [Test]
        public void Exception_returns_a_descriptive_error_when_the_result_is_null()
        {
            var table = new Table("Field", "Value");

            var exception = GetExceptionThrownByThisComparison(table, null);

            exception.Message.ShouldEqual("The item to compare was null.");
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
        public DateTime DateTimeProperty { get; set; }

        public bool BooleanProperty { get; set; }

        public int? NullableField { get; set; }
    }
}