using System;
using System.Globalization;
using System.Threading;
using NUnit.Framework;
using Should;
using TechTalk.SpecFlow.Assist;
using TechTalk.SpecFlow.RuntimeTests.AssistTests.TestInfrastructure;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests
{
    [TestFixture]
    public class InstanceComparisonExtensionMethodsTests
    {
        [SetUp]
        public void SetUp()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        }

        [Test]
        public void Throws_exception_when_value_of_matching_string_property_does_not_match()
        {
            var table = new Table("Field", "Value");
            table.AddRow("StringProperty", "Howard Roark");

            var test = new InstanceComparisonTestObject {StringProperty = "Peter Keating"};

            ComparisonTestResult comparisonResult = ExceptionWasThrownByThisComparison(table, test);

            comparisonResult.ExceptionWasThrown.ShouldBeTrue();
        }

        [Test]
        public void Does_not_throw_exception_when_value_of_matching_string_property_matches()
        {
            var table = new Table("Field", "Value");
            table.AddRow("StringProperty", "Howard Roark");

            var test = new InstanceComparisonTestObject {StringProperty = "Howard Roark"};

            ComparisonTestResult comparisonResult = ExceptionWasThrownByThisComparison(table, test);

            comparisonResult.ExceptionWasThrown.ShouldBeFalse(comparisonResult.ExceptionMessage);
        }

        [Test]
        public void Throws_exception_when_first_row_matches_but_second_does_not()
        {
            var table = new Table("Field", "Value");
            table.AddRow("StringProperty", "Howard Roark");
            table.AddRow("IntProperty", "20");

            var test = new InstanceComparisonTestObject
                           {
                               StringProperty = "Howard Roark",
                               IntProperty = 10
                           };

            ComparisonTestResult comparisonResult = ExceptionWasThrownByThisComparison(table, test);

            comparisonResult.ExceptionWasThrown.ShouldBeTrue();
        }

        [Test]
        public void Should_not_throw_an_exception_when_the_ToString_values_of_each_match()
        {
            var table = new Table("Field", "Value");
            table.AddRow("StringProperty", "Howard Roark");
            table.AddRow("IntProperty", "10");
            table.AddRow("DateTimeProperty", "12/25/2010 8:00:00 AM");

            var test = new InstanceComparisonTestObject
            {
                StringProperty = "Howard Roark",
                IntProperty = 10,
                DateTimeProperty = new DateTime(2010, 12, 25, 8, 0, 0)
            };

            ComparisonTestResult comparisonResult = ExceptionWasThrownByThisComparison(table, test);

            comparisonResult.ExceptionWasThrown.ShouldBeFalse(comparisonResult.ExceptionMessage);
        }

        [Test]
        public void Should_not_throw_an_exception_when_the_date_time_value_is_midnight_and_the_table_does_not_include_a_time()
        {
            var table = new Table("Field", "Value");
            table.AddRow("DateTimeProperty", "12/25/2010");

            var test = new InstanceComparisonTestObject
            {
                DateTimeProperty = new DateTime(2010, 12, 25, 0, 0, 0)
            };

            ComparisonTestResult comparisonResult = ExceptionWasThrownByThisComparison(table, test);

            comparisonResult.ExceptionWasThrown.ShouldBeFalse(comparisonResult.ExceptionMessage);
        }

        [Test]
        public void Should_not_throw_an_exception_when_actual_value_is_null_and_expected_is_empty()
        {
            var table = new Table("Field", "Value");
            table.AddRow("NullableField", "");

            var test = new InstanceComparisonTestObject
            {
                NullableField = null,
            };

            ComparisonTestResult comparisonResult = ExceptionWasThrownByThisComparison(table, test);

            comparisonResult.ExceptionWasThrown.ShouldBeFalse(comparisonResult.ExceptionMessage);
        }

        [Test]
        public void Exception_returns_an_exception_for_one_error_when_there_is_one_difference()
        {
            var table = new Table("Field", "Value");
            table.AddRow("StringProperty", "Howard Roark");

            var test = new InstanceComparisonTestObject {StringProperty = "Peter Keating"};

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

            var test = new InstanceComparisonTestObject
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

            var test = new InstanceComparisonTestObject();

            var exception = GetExceptionThrownByThisComparison(table, test);

            exception.Message.ShouldEqual(@"The following fields did not match:
IDoNotExist: Property does not exist");
        }

        [Test]
        public void Will_property_handle_true_boolean_matches()
        {
            var table = new Table("Field", "Value");
            table.AddRow("BooleanProperty", "true");

            ComparisonTestResult comparisonResult = ExceptionWasThrownByThisComparison(table, new InstanceComparisonTestObject
                                                                                                        {
                                                                                                            BooleanProperty = true
                                                                                                        });
            
            comparisonResult.ExceptionWasThrown.ShouldBeFalse(comparisonResult.ExceptionMessage);

            comparisonResult = ExceptionWasThrownByThisComparison(table, new InstanceComparisonTestObject
                                                                                                    {
                                                                                                        BooleanProperty = false
                                                                                                    });

            comparisonResult.ExceptionWasThrown.ShouldBeTrue(comparisonResult.ExceptionMessage);
        }

        [Test]
        public void Will_match_guids_without_case_insensitivity()
        {
            var table = new Table("Field", "Value");
            table.AddRow("GuidProperty", "DFFC3F4E-670A-400A-8212-C6841E2EA055");

            ComparisonTestResult comparisonResult = ExceptionWasThrownByThisComparison(table, new InstanceComparisonTestObject
                                                                                                        {
                                                                                                            GuidProperty = new Guid("DFFC3F4E-670A-400A-8212-C6841E2EA055")
                                                                                                        });

            comparisonResult.ExceptionWasThrown.ShouldBeFalse(comparisonResult.ExceptionMessage);
        }

        [Test]
        public void Will_match_decimals_regardless_of_trailing_zeroes()
        {
            var table = new Table("Field", "Value");
            table.AddRow("DecimalProperty", "4.23");
            var comparisonResult = ExceptionWasThrownByThisComparison(table, new InstanceComparisonTestObject
                                                                                 {
                                                                                     DecimalProperty = 4.23000000M
                                                                                 });

            comparisonResult.ExceptionWasThrown.ShouldBeFalse();
        }

        [Test]
        public void Throws_exception_when_the_result_is_null()
        {
            var table = new Table("Field", "Value");

            var comparisonResult = ExceptionWasThrownByThisComparison(table, null);

            comparisonResult.ExceptionWasThrown.ShouldBeTrue(comparisonResult.ExceptionMessage);
        }

        [Test]
        public void Exception_returns_a_descriptive_error_when_the_result_is_null()
        {
            var table = new Table("Field", "Value");

            var exception = GetExceptionThrownByThisComparison(table, null);

            exception.Message.ShouldEqual("The item to compare was null.");
        }

        [Test]
        public void Ignores_spaces_when_matching_property_name()
        {
            var table = new Table("Field", "Value");
            table.AddRow("String Property", "Howard Roark");

            var test = new InstanceComparisonTestObject { StringProperty = "Howard Roark" };

            var comparisonResult = ExceptionWasThrownByThisComparison(table, test);

            comparisonResult.ExceptionWasThrown.ShouldBeFalse(comparisonResult.ExceptionMessage);
        }

        [Test]
        public void Ignores_casing_when_matching_property_name()
        {
            var table = new Table("Field", "Value");
            table.AddRow("STRINGproperty", "Howard Roark");

            var test = new InstanceComparisonTestObject { StringProperty = "Howard Roark" };

            var comparisonResult = ExceptionWasThrownByThisComparison(table, test);

            comparisonResult.ExceptionWasThrown.ShouldBeFalse(comparisonResult.ExceptionMessage);
        }

        [Test]
        public void Can_compare_a_horizontal_table()
        {
            var table = new Table("StringProperty", "IntProperty", "DecimalProperty");
            table.AddRow("Test", "42", "23.01");

            var test = new InstanceComparisonTestObject
                           {
                               StringProperty = "Test",
                               IntProperty = 42,
                               DecimalProperty = 23.01M
                           };

            var comparisonResult = ExceptionWasThrownByThisComparison(table, test);
            comparisonResult.ExceptionWasThrown.ShouldBeFalse(comparisonResult.ExceptionMessage);
        }

        private static ComparisonException GetExceptionThrownByThisComparison(Table table, InstanceComparisonTestObject test)
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

        private static ComparisonTestResult ExceptionWasThrownByThisComparison(Table table, InstanceComparisonTestObject test)
        {
            var result = new ComparisonTestResult { ExceptionWasThrown = false };
            try
            {
                table.CompareToInstance(test);
            }
            catch (ComparisonException ex)
            {
                result.ExceptionWasThrown = true;
                result.ExceptionMessage = ex.Message;
            }
            return result;
        }
    }
}