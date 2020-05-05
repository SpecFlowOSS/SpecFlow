using System;
using System.Globalization;
using System.Threading;
using Xunit;
using FluentAssertions;
using TechTalk.SpecFlow.Assist;
using TechTalk.SpecFlow.RuntimeTests.AssistTests.TestInfrastructure;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests
{
    public abstract class SetComparisonExtensionMethods_ThrowTests
    {
        public SetComparisonExtensionMethods_ThrowTests()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);
        }

        [Fact]
        public void Throws_exception_when_the_table_is_empty_and_the_set_has_one_item()
        {
            var table = new Table("StringProperty");

            var items = new[] {new SetComparisonTestObject()};

            var comparisonResult = DetermineIfExceptionWasThrownByComparingThese(table, items);

            comparisonResult.ExceptionWasThrown.Should().BeTrue(comparisonResult.ExceptionMessage);
        }

        [Fact]
        public void Does_not_throw_an_exception_when_the_table_is_empty_and_the_set_is_empty()
        {
            var table = new Table("StringProperty");

            var items = new SetComparisonTestObject[] {};

            var comparisonResult = DetermineIfExceptionWasThrownByComparingThese(table, items);

            comparisonResult.ExceptionWasThrown.Should().BeFalse(comparisonResult.ExceptionMessage);
        }

        [Fact]
        public void Throws_an_exception_when_there_is_one_row_but_the_set_is_empty()
        {
            var table = new Table("StringProperty");
            table.AddRow("this is not an empty table");

            var items = new SetComparisonTestObject[] {};

            var comparisonResult = DetermineIfExceptionWasThrownByComparingThese(table, items);

            comparisonResult.ExceptionWasThrown.Should().BeTrue(comparisonResult.ExceptionMessage);
        }

        [Fact]
        public void Does_not_throw_an_exception_when_there_is_one_row_and_one_matching_item_in_the_set()
        {
            var table = new Table("StringProperty");
            table.AddRow("Taggart Transcontinental");

            var items = new[] {new SetComparisonTestObject {StringProperty = "Taggart Transcontinental"}};

            var comparisonResult = DetermineIfExceptionWasThrownByComparingThese(table, items);

            comparisonResult.ExceptionWasThrown.Should().BeFalse(comparisonResult.ExceptionMessage);
        }

        [Fact]
        public void Throws_an_exception_when_the_table_has_one_item_and_the_set_has_one_item_that_does_not_match()
        {
            var table = new Table("StringProperty");
            table.AddRow("Taggart Transcontinental");

            var items = new[] {new SetComparisonTestObject {StringProperty = "Phoenix-Durango"}};

            var comparisonResult = DetermineIfExceptionWasThrownByComparingThese(table, items);

            comparisonResult.ExceptionWasThrown.Should().BeTrue(comparisonResult.ExceptionMessage);
        }

        [Fact]
        public void Throws_an_exception_when_the_first_property_matches_but_the_second_does_not()
        {
            var table = new Table("StringProperty", "IntProperty");
            table.AddRow("Taggart Transcontinental", "10");

            var items = new[] {new SetComparisonTestObject {StringProperty = "Taggart Transcontinental", IntProperty = 20}};

            var comparisonResult = DetermineIfExceptionWasThrownByComparingThese(table, items);

            comparisonResult.ExceptionWasThrown.Should().BeTrue(comparisonResult.ExceptionMessage);
        }

        [Fact]
        public void Does_not_throw_an_exception_when_there_are_two_matching_properties_of_varying_types()
        {
            var table = new Table("StringProperty", "IntProperty");
            table.AddRow("Taggart Transcontinental", "10");

            var items = new[]
                            {
                                new SetComparisonTestObject
                                    {
                                        StringProperty = "Taggart Transcontinental",
                                        IntProperty = 10
                                    }
                            };

            var comparisonResult = DetermineIfExceptionWasThrownByComparingThese(table, items);

            comparisonResult.ExceptionWasThrown.Should().BeFalse(comparisonResult.ExceptionMessage);
        }

        [Fact]
        public void Throws_an_exception_when_the_first_two_properties_match_but_the_third_does_not()
        {
            var table = new Table("StringProperty", "IntProperty", "DateTimeProperty");
            table.AddRow("Taggart Transcontinental", "10", "12/25/2010 8:00:00 AM");

            var items = new[]
                            {
                                new SetComparisonTestObject
                                    {
                                        StringProperty = "Taggart Transcontinental",
                                        IntProperty = 10,
                                        DateTimeProperty = new DateTime(2010, 12, 26, 8, 0, 0)
                                    }
                            };

            var comparisonResult = DetermineIfExceptionWasThrownByComparingThese(table, items);

            comparisonResult.ExceptionWasThrown.Should().BeTrue(comparisonResult.ExceptionMessage);
        }

        [Fact]
        public void Does_not_throw_an_exception_if_all_three_properties_match()
        {
            var table = new Table("StringProperty", "IntProperty", "DateTimeProperty");
            table.AddRow("Taggart Transcontinental", "10", "12/25/2010 8:00:00 AM");

            var items = new[]
                            {
                                new SetComparisonTestObject
                                    {
                                        StringProperty = "Taggart Transcontinental",
                                        IntProperty = 10,
                                        DateTimeProperty = new DateTime(2010, 12, 25, 8, 0, 0)
                                    }
                            };

            var comparisonResult = DetermineIfExceptionWasThrownByComparingThese(table, items);

            comparisonResult.ExceptionWasThrown.Should().BeFalse(comparisonResult.ExceptionMessage);
        }

        [Fact]
        public void Throws_exception_if_property_in_table_does_not_exist_on_item()
        {
            var table = new Table("IDoNotExist");
            table.AddRow("nananana boo boo");

            var items = new[] {new SetComparisonTestObject()};

            var comparisonResult = DetermineIfExceptionWasThrownByComparingThese(table, items);

            comparisonResult.ExceptionWasThrown.Should().BeTrue(comparisonResult.ExceptionMessage);
        }

        [Fact]
        public void Does_not_throw_an_exception_if_the_property_being_checked_for_has_extra_spaces_in_the_name()
        {
            var table = new Table("String Property");
            table.AddRow("applesauce");

            var items = new[] { new SetComparisonTestObject{StringProperty = "applesauce"} };

            var comparisonResult = DetermineIfExceptionWasThrownByComparingThese(table, items);

            comparisonResult.ExceptionWasThrown.Should().BeFalse();
        }

        [Fact]
        public void Does_not_throw_an_exception_if_the_property_being_checked_for_has_different_casing()
        {
            var table = new Table("stringproperty");
            table.AddRow("mustard");

            var items = new[] { new SetComparisonTestObject { StringProperty = "mustard" } };

            var comparisonResult = DetermineIfExceptionWasThrownByComparingThese(table, items);

            comparisonResult.ExceptionWasThrown.Should().BeFalse();
        }

        [Fact]
        public void Does_not_throw_an_exception_if_the_property_being_checked_for_has_international_characters_mapped_to_identifier_name()
        {
            var table = new Table("ŠtríngPröpërtý");
            table.AddRow("mustard");

            var items = new[] { new SetComparisonTestObject { StringProperty = "mustard" } };

            var comparisonResult = DetermineIfExceptionWasThrownByComparingThese(table, items);

            comparisonResult.ExceptionWasThrown.Should().BeFalse();
        }

        [Fact]
        public void Does_not_throw_exception_if_string_is_empty_in_table_and_null_on_item()
        {
            var table = new Table("StringProperty");
            table.AddRow("");

            var items = new[] {new SetComparisonTestObject {StringProperty = null}};

            var comparisonResult = DetermineIfExceptionWasThrownByComparingThese(table, items);

            comparisonResult.ExceptionWasThrown.Should().BeFalse(comparisonResult.ExceptionMessage);

        }

        [Fact]
        public void Throws_exception_if_string_is_empty_in_table_and_not_null_on_item()
        {
            var table = new Table("StringProperty");
            table.AddRow("");

            var items = new[] {new SetComparisonTestObject {StringProperty = "ketchup"}};

            var comparisonResult = DetermineIfExceptionWasThrownByComparingThese(table, items);

            comparisonResult.ExceptionWasThrown.Should().BeTrue(comparisonResult.ExceptionMessage);
        }

        [Fact]
        public void Throws_exception_if_string_is_not_empty_in_table_and_null_on_item()
        {
            var table = new Table("StringProperty");
            table.AddRow("mustard");

            var items = new[] {new SetComparisonTestObject {StringProperty = null}};

            var comparisonResult = DetermineIfExceptionWasThrownByComparingThese(table, items);

            comparisonResult.ExceptionWasThrown.Should().BeTrue(comparisonResult.ExceptionMessage);
        }

        [Fact]
        public void Throw_exception_if_first_item_in_sets_match_but_second_item_does_not()
        {
            var table = new Table("StringProperty");
            table.AddRow("ketchup");
            table.AddRow("mustard");

            var items = new[]
                            {
                                new SetComparisonTestObject {StringProperty = "ketchup"},
                                new SetComparisonTestObject {StringProperty = "spicy mustard"}
                            };

            var comparisonResult = DetermineIfExceptionWasThrownByComparingThese(table, items);

            comparisonResult.ExceptionWasThrown.Should().BeTrue(comparisonResult.ExceptionMessage);
        }

        [Fact]
        public void Does_not_throw_exception_if_two_items_in_both_sets_match()
        {
            var table = new Table("StringProperty");
            table.AddRow("ketchup");
            table.AddRow("mustard");

            var items = new[]
                            {
                                new SetComparisonTestObject {StringProperty = "ketchup"},
                                new SetComparisonTestObject {StringProperty = "mustard"}
                            };

            var comparisonResult = DetermineIfExceptionWasThrownByComparingThese(table, items);

            comparisonResult.ExceptionWasThrown.Should().BeFalse(comparisonResult.ExceptionMessage);
        }

        [Fact]
        public void Throws_an_exception_if_items_do_not_match_the_correct_number_of_times()
        {
            var table = new Table("StringProperty");
            table.AddRow("Burt");
            table.AddRow("Ernie");
            table.AddRow("Burt");

            var items = new[]
                            {
                                new SetComparisonTestObject {StringProperty = "Burt"},
                                new SetComparisonTestObject {StringProperty = "Ernie"},
                                new SetComparisonTestObject {StringProperty = "Ernie"}
                            };

            var comparisonResult = DetermineIfExceptionWasThrownByComparingThese(table, items);

            comparisonResult.ExceptionWasThrown.Should().BeTrue(comparisonResult.ExceptionMessage);
        }

        [Fact]
        public void Throws_an_exception_if_the_result_set_has_extra_items_that_are_not_in_the_expected_results()
        {
            var table = new Table("StringProperty");
            table.AddRow("Burt");
            table.AddRow("Ernie");

            var items = new[]
                            {
                                new SetComparisonTestObject {StringProperty = "Burt"},
                                new SetComparisonTestObject {StringProperty = "Ernie"},
                                new SetComparisonTestObject {StringProperty = "Grover"}
                            };

            var comparisonResult = DetermineIfExceptionWasThrownByComparingThese(table, items);

            comparisonResult.ExceptionWasThrown.Should().BeTrue(comparisonResult.ExceptionMessage);
        }

        [Fact]
        public void Can_compare_guids_properly()
        {
            var table = new Table("GuidProperty");
            table.AddRow("5F270095-FF26-4CE1-9C43-DA909FF228F4");

            var items = new[]
                            {
                                new SetComparisonTestObject {GuidProperty = new Guid("5F270095-FF26-4CE1-9C43-DA909FF228F4")},
                            };

            var comparisonResult = DetermineIfExceptionWasThrownByComparingThese(table, items);

            comparisonResult.ExceptionWasThrown.Should().BeFalse(comparisonResult.ExceptionMessage);
        }

        protected ComparisonTestResult DetermineIfExceptionWasThrownByComparingThese(Table table, SetComparisonTestObject[] items)
        {
            var result = new ComparisonTestResult { ExceptionWasThrown = false };
            try
            {
                CallComparison(table, items);
            }
            catch (ComparisonException ex)
            {
                result.ExceptionWasThrown = true;
                result.ExceptionMessage = ex.Message;
            }
            return result;
        }

        protected abstract void CallComparison(Table table, SetComparisonTestObject[] items);
    }

    
    public class SetComparisonExtensionMethods_OrderInsensitive_ThrowTests : SetComparisonExtensionMethods_ThrowTests
    {
        [Fact]
        public void Does_not_throw_exception_if_all_items_match_but_not_in_the_same_order()
        {
            var table = new Table("StringProperty");
            table.AddRow("relish");
            table.AddRow("mustard");
            table.AddRow("ketchup");

            var items = new[]
                            {
                                new SetComparisonTestObject {StringProperty = "ketchup"},
                                new SetComparisonTestObject {StringProperty = "relish"},
                                new SetComparisonTestObject {StringProperty = "mustard"}
                            };

            var comparisonResult = DetermineIfExceptionWasThrownByComparingThese(table, items);

            comparisonResult.ExceptionWasThrown.Should().BeFalse(comparisonResult.ExceptionMessage);
        }

        protected override void CallComparison(Table table, SetComparisonTestObject[] items)
        {
            table.CompareToSet(items);
        }
    }

    
    public class SetComparisonExtensionMethods_OrderSensitive_ThrowTests : SetComparisonExtensionMethods_ThrowTests
    {
        [Fact]
        public void Throws_an_exception_if_all_items_match_but_not_in_the_same_order()
        {
            var table = new Table("StringProperty");
            table.AddRow("relish");
            table.AddRow("mustard");
            table.AddRow("ketchup");

            var items = new[]
                            {
                                new SetComparisonTestObject {StringProperty = "ketchup"},
                                new SetComparisonTestObject {StringProperty = "relish"},
                                new SetComparisonTestObject {StringProperty = "mustard"}
                            };

            var comparisonResult = DetermineIfExceptionWasThrownByComparingThese(table, items);

            comparisonResult.ExceptionWasThrown.Should().BeTrue(comparisonResult.ExceptionMessage);
        }

        protected override void CallComparison(Table table, SetComparisonTestObject[] items)
        {
            table.CompareToSet(items, sequentialEquality: true);
        }
    }
}