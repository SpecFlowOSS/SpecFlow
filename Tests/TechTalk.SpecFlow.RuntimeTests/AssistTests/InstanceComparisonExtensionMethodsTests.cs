using System;
using System.Globalization;
using System.Threading;
using Xunit;
using FluentAssertions;
using TechTalk.SpecFlow.Assist;
using TechTalk.SpecFlow.RuntimeTests.AssistTests.TestInfrastructure;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests
{
    
    public class InstanceComparisonExtensionMethodsTests
    {
        public InstanceComparisonExtensionMethodsTests()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);
        }

        [Fact]
        public void Throws_exception_when_value_of_matching_string_property_does_not_match()
        {
            var table = new Table("Field", "Value");
            table.AddRow("StringProperty", "Howard Roark");

            var test = new InstanceComparisonTestObject { StringProperty = "Peter Keating" };

            ComparisonTestResult comparisonResult = ExceptionWasThrownByThisComparison(table, test);

            comparisonResult.ExceptionWasThrown.Should().BeTrue();
        }

        [Fact]
        public void Does_not_throw_exception_when_value_of_matching_string_property_matches()
        {
            var table = new Table("Field", "Value");
            table.AddRow("StringProperty", "Howard Roark");

            var test = new InstanceComparisonTestObject { StringProperty = "Howard Roark" };

            ComparisonTestResult comparisonResult = ExceptionWasThrownByThisComparison(table, test);

            comparisonResult.ExceptionWasThrown.Should().BeFalse(comparisonResult.ExceptionMessage);
        }

        [Fact]
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

            comparisonResult.ExceptionWasThrown.Should().BeTrue();
        }

        [Fact]
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

            comparisonResult.ExceptionWasThrown.Should().BeFalse(comparisonResult.ExceptionMessage);
        }

        [Fact]
        public void Should_not_throw_an_exception_when_the_date_time_value_is_midnight_and_the_table_does_not_include_a_time()
        {
            var table = new Table("Field", "Value");
            table.AddRow("DateTimeProperty", "12/25/2010");

            var test = new InstanceComparisonTestObject
            {
                DateTimeProperty = new DateTime(2010, 12, 25, 0, 0, 0)
            };

            ComparisonTestResult comparisonResult = ExceptionWasThrownByThisComparison(table, test);

            comparisonResult.ExceptionWasThrown.Should().BeFalse(comparisonResult.ExceptionMessage);
        }

        [Fact]
        public void Should_not_throw_an_exception_when_actual_value_is_null_and_expected_is_empty()
        {
            var table = new Table("Field", "Value");
            table.AddRow("NullableIntProperty", "");

            var test = new InstanceComparisonTestObject
            {
                NullableIntProperty = null,
            };

            ComparisonTestResult comparisonResult = ExceptionWasThrownByThisComparison(table, test);

            comparisonResult.ExceptionWasThrown.Should().BeFalse(comparisonResult.ExceptionMessage);
        }

        [Fact]
        public void Exception_returns_an_exception_for_one_error_when_there_is_one_difference()
        {
            var table = new Table("Field", "Value");
            table.AddRow("StringProperty", "Howard Roark");

            var test = new InstanceComparisonTestObject { StringProperty = "Peter Keating" };

            var exception = GetExceptionThrownByThisComparison(table, test);

            exception.Message.AgnosticLineBreak().Should().Be(
                @"The following fields did not match:
StringProperty: Expected <Howard Roark>, Actual <Peter Keating>, Using 'TechTalk.SpecFlow.Assist.ValueComparers.DefaultValueComparer'".AgnosticLineBreak());
        }

        [Fact]
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

            exception.Message.AgnosticLineBreak().Should().Be(
                @"The following fields did not match:
StringProperty: Expected <Howard Roark>, Actual <Peter Keating>, Using 'TechTalk.SpecFlow.Assist.ValueComparers.DefaultValueComparer'
IntProperty: Expected <1>, Actual <2>, Using 'TechTalk.SpecFlow.Assist.ValueComparers.DefaultValueComparer'".AgnosticLineBreak());
        }

        [Fact]
        public void Exception_returns_a_descriptive_error_when_property_in_table_does_not_exist()
        {
            var table = new Table("Field", "Value");
            table.AddRow("IDoNotExist", "Ok, mister");

            var test = new InstanceComparisonTestObject();

            var exception = GetExceptionThrownByThisComparison(table, test);

            exception.Message.AgnosticLineBreak().Should().Be(@"The following fields did not match:
IDoNotExist: Property does not exist".AgnosticLineBreak());
        }

        [Fact]
        public void Will_property_handle_true_boolean_matches()
        {
            var table = new Table("Field", "Value");
            table.AddRow("BoolProperty", "true");

            ComparisonTestResult comparisonResult = ExceptionWasThrownByThisComparison(table, new InstanceComparisonTestObject
            {
                BoolProperty = true
            });

            comparisonResult.ExceptionWasThrown.Should().BeFalse(comparisonResult.ExceptionMessage);

            comparisonResult = ExceptionWasThrownByThisComparison(table, new InstanceComparisonTestObject
            {
                BoolProperty = false
            });

            comparisonResult.ExceptionWasThrown.Should().BeTrue(comparisonResult.ExceptionMessage);
        }

        [Fact]
        public void Will_match_guids_without_case_insensitivity()
        {
            var table = new Table("Field", "Value");
            table.AddRow("GuidProperty", "DFFC3F4E-670A-400A-8212-C6841E2EA055");

            ComparisonTestResult comparisonResult = ExceptionWasThrownByThisComparison(table, new InstanceComparisonTestObject
            {
                GuidProperty = new Guid("DFFC3F4E-670A-400A-8212-C6841E2EA055")
            });

            comparisonResult.ExceptionWasThrown.Should().BeFalse(comparisonResult.ExceptionMessage);
        }

        [Fact]
        public void Will_match_decimals_regardless_of_trailing_zeroes()
        {
            var table = new Table("Field", "Value");
            table.AddRow("DecimalProperty", "4.23");
            var comparisonResult = ExceptionWasThrownByThisComparison(table, new InstanceComparisonTestObject
            {
                DecimalProperty = 4.23000000M
            });

            comparisonResult.ExceptionWasThrown.Should().BeFalse();
        }

        [Fact]
        public void Throws_exception_when_the_result_is_null()
        {
            var table = new Table("Field", "Value");

            var comparisonResult = ExceptionWasThrownByThisComparison(table, (InstanceComparisonTestObject)null);

            comparisonResult.ExceptionWasThrown.Should().BeTrue(comparisonResult.ExceptionMessage);
        }

        [Fact]
        public void Exception_returns_a_descriptive_error_when_the_result_is_null()
        {
            var table = new Table("Field", "Value");

            var exception = GetExceptionThrownByThisComparison(table, (InstanceComparisonTestObject)null);

            exception.Message.Should().Be("The item to compare was null.");
        }

        [Fact]
        public void Ignores_spaces_when_matching_property_name()
        {
            var table = new Table("Field", "Value");
            table.AddRow("String Property", "Howard Roark");

            var test = new InstanceComparisonTestObject { StringProperty = "Howard Roark" };

            var comparisonResult = ExceptionWasThrownByThisComparison(table, test);

            comparisonResult.ExceptionWasThrown.Should().BeFalse(comparisonResult.ExceptionMessage);
        }

        [Fact]
        public void Ignores_casing_when_matching_property_name()
        {
            var table = new Table("Field", "Value");
            table.AddRow("STRINGproperty", "Howard Roark");

            var test = new InstanceComparisonTestObject { StringProperty = "Howard Roark" };

            var comparisonResult = ExceptionWasThrownByThisComparison(table, test);

            comparisonResult.ExceptionWasThrown.Should().BeFalse(comparisonResult.ExceptionMessage);
        }

        [Fact]
        public void Can_compare_a_horizontal_table()
        {
            var table = new Table("StringProperty", "IntProperty", "DecimalProperty", "FloatProperty");
            table.AddRow("Test", "42", "23.01", "11.56");

            var test = new InstanceComparisonTestObject
            {
                StringProperty = "Test",
                IntProperty = 42,
                DecimalProperty = 23.01M,
                FloatProperty = 11.56F
            };

            var comparisonResult = ExceptionWasThrownByThisComparison(table, test);
            comparisonResult.ExceptionWasThrown.Should().BeFalse(comparisonResult.ExceptionMessage);
        }

        [Fact]
        public void Supports_all_standard_types()
        {
            var table = new Table(
                "ByteProperty", "NullableByteProperty", "SByteProperty", "NullableSByteProperty",
                "IntProperty", "NullableIntProperty", "UIntProperty", "NullableUIntProperty",
                "ShortProperty", "NullableShortProperty", "UShortProperty", "NullableUShortProperty",
                "LongProperty", "NullableLongProperty", "ULongProperty", "NullableULongProperty",
                "FloatProperty", "NullableFloatProperty",
                "DoubleProperty", "NullableDoubleProperty",
                "DecimalProperty", "NullableDecimalProperty",
                "CharProperty", "NullableCharProperty",
                "BoolProperty", "NullableBoolProperty",
                "DateTimeProperty", "NullableDateTimeProperty",
                "GuidProperty", "NullableGuidProperty",
                "StringProperty");

            table.AddRow(
                "1", "2", "3", "4",
                "5", "6", "7", "8",
                "9", "10", "11", "12",
                "13", "14", "15", "16",
                "1.01", "2.02",
                "3.03", "4.04",
                "5.05", "6.06",
                "a", "b",
                "true", "false",
                "1.1.2011", "2.2.2022",
                "45D7E8BA-74C9-4B92-9570-42D680F05C2C", "C56B8A48-D7A3-4B75-99FE-93FADBB06180",
                "Test");

            var test = new StandardTypesComparisonTestObject
            {
                ByteProperty = 1,
                NullableByteProperty = 2,
                SByteProperty = 3,
                NullableSByteProperty = 4,

                IntProperty = 5,
                NullableIntProperty = 6,
                UIntProperty = 7,
                NullableUIntProperty = 8,

                ShortProperty = 9,
                NullableShortProperty = 10,
                UShortProperty = 11,
                NullableUShortProperty = 12,

                LongProperty = 13,
                NullableLongProperty = 14,
                ULongProperty = 15,
                NullableULongProperty = 16,

                FloatProperty = 1.01f,
                NullableFloatProperty = 2.02f,

                DoubleProperty = 3.03,
                NullableDoubleProperty = 4.04,

                DecimalProperty = 5.05m,
                NullableDecimalProperty = 6.06m,

                CharProperty = 'a',
                NullableCharProperty = 'b',

                BoolProperty = true,
                NullableBoolProperty = false,

                DateTimeProperty = new DateTime(2011, 1, 1),
                NullableDateTimeProperty = new DateTime(2022, 2, 2),

                GuidProperty = new Guid("45D7E8BA-74C9-4B92-9570-42D680F05C2C"),
                NullableGuidProperty = new Guid("C56B8A48-D7A3-4B75-99FE-93FADBB06180"),

                StringProperty = "Test"
            };

            var comparisonResult = ExceptionWasThrownByThisComparison(table, test);
            comparisonResult.ExceptionWasThrown.Should().BeFalse(comparisonResult.ExceptionMessage);
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

        private static ComparisonTestResult ExceptionWasThrownByThisComparison(Table table, StandardTypesComparisonTestObject test)
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