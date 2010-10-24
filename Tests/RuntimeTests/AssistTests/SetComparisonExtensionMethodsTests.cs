using System;
using NUnit.Framework;
using Should;
using TechTalk.SpecFlow.Assist;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests
{
    [TestFixture]
    public class SetComparisonExtensionMethodsTests
    {
        [Test]
        public void Throws_exception_when_the_table_is_empty_and_the_set_has_one_item()
        {
            var table = new Table("StringProperty");

            var items = new[] {new SetTestObject()};

            var exceptionWasThrown = DetermineIfExceptionWasThrownByComparingThese(table, items);

            exceptionWasThrown.ShouldBeTrue();
        }

        [Test]
        public void Does_not_throw_an_exception_when_the_table_is_empty_and_the_set_is_empty()
        {
            var table = new Table("StringProperty");

            var items = new SetTestObject[] {};

            var exceptionWasThrown = DetermineIfExceptionWasThrownByComparingThese(table, items);

            exceptionWasThrown.ShouldBeFalse();
        }

        [Test]
        public void Throws_an_exception_when_there_is_one_row_but_the_set_is_empty()
        {
            var table = new Table("StringProperty");
            table.AddRow("this is not an empty table");

            var items = new SetTestObject[] {};

            var exceptionWasThrown = DetermineIfExceptionWasThrownByComparingThese(table, items);

            exceptionWasThrown.ShouldBeTrue();
        }

        [Test]
        public void Does_not_throw_an_exception_when_there_is_one_row_and_one_matching_item_in_the_set()
        {
            var table = new Table("StringProperty");
            table.AddRow("Taggart Transcontinental");

            var items = new[] {new SetTestObject {StringProperty = "Taggart Transcontinental"}};

            var exceptionWasThrown = DetermineIfExceptionWasThrownByComparingThese(table, items);

            exceptionWasThrown.ShouldBeFalse();
        }

        [Test]
        public void Throws_an_exception_when_the_table_has_one_item_and_the_set_has_one_item_that_does_not_match()
        {
            var table = new Table("StringProperty");
            table.AddRow("Taggart Transcontinental");

            var items = new[] {new SetTestObject {StringProperty = "Phoenix-Durango"}};

            var exceptionWasThrown = DetermineIfExceptionWasThrownByComparingThese(table, items);

            exceptionWasThrown.ShouldBeTrue();
        }

        [Test]
        public void Throws_an_exception_when_the_first_property_matches_but_the_second_does_not()
        {
            var table = new Table("StringProperty", "IntProperty");
            table.AddRow("Taggart Transcontinental", "10");

            var items = new[] {new SetTestObject {StringProperty = "Taggart Transcontinental", IntProperty = 20}};

            var exceptionWasThrown = DetermineIfExceptionWasThrownByComparingThese(table, items);

            exceptionWasThrown.ShouldBeTrue();
        }

        [Test]
        public void Does_not_throw_an_exception_when_there_are_two_matching_properties_of_varying_types()
        {
            var table = new Table("StringProperty", "IntProperty");
            table.AddRow("Taggart Transcontinental", "10");

            var items = new[]
                            {
                                new SetTestObject
                                    {
                                        StringProperty = "Taggart Transcontinental",
                                        IntProperty = 10
                                    }
                            };

            var exceptionWasThrown = DetermineIfExceptionWasThrownByComparingThese(table, items);

            exceptionWasThrown.ShouldBeFalse();
        }

        [Test]
        public void Throws_an_exception_when_the_first_two_properties_match_but_the_third_does_not()
        {
            var table = new Table("StringProperty", "IntProperty", "DateTimeProperty");
            table.AddRow("Taggart Transcontinental", "10", "12/25/2010");

            var items = new[]
                            {
                                new SetTestObject
                                    {
                                        StringProperty = "Taggart Transcontinental",
                                        IntProperty = 10,
                                        DateTimeProperty = new DateTime(2010, 12, 26)
                                    }
                            };

            var exceptionWasThrown = DetermineIfExceptionWasThrownByComparingThese(table, items);

            exceptionWasThrown.ShouldBeTrue();
        }

        [Test]
        public void Does_not_throw_an_exception_if_all_three_properties_match()
        {
            var table = new Table("StringProperty", "IntProperty", "DateTimeProperty");
            table.AddRow("Taggart Transcontinental", "10", "12/25/2010");

            var items = new[]
                            {
                                new SetTestObject
                                    {
                                        StringProperty = "Taggart Transcontinental",
                                        IntProperty = 10,
                                        DateTimeProperty = new DateTime(2010, 12, 25)
                                    }
                            };

            var exceptionWasThrown = DetermineIfExceptionWasThrownByComparingThese(table, items);

            exceptionWasThrown.ShouldBeFalse();
        }

        [Test]
        public void Throws_exception_if_property_in_table_does_not_exist_on_item()
        {
            var table = new Table("IDoNotExist");
            table.AddRow("nananana boo boo");

            var items = new[]{new SetTestObject()};

            var exceptionWasThrown = DetermineIfExceptionWasThrownByComparingThese(table, items);

            exceptionWasThrown.ShouldBeTrue();
        }

        [Test]
        public void Does_not_throw_exception_if_string_is_empty_in_table_and_null_on_item()
        {
            var table = new Table("StringProperty");
            table.AddRow("");

            var items = new[]{new SetTestObject{StringProperty = null}};

            var exceptionWasThrown = DetermineIfExceptionWasThrownByComparingThese(table, items);

            exceptionWasThrown.ShouldBeFalse();
        }

        [Test]
        public void Throws_exception_if_string_is_empty_in_table_and_not_null_on_item()
        {
            var table = new Table("StringProperty");
            table.AddRow("");

            var items = new[] { new SetTestObject { StringProperty = "ketchup" } };

            var exceptionWasThrown = DetermineIfExceptionWasThrownByComparingThese(table, items);

            exceptionWasThrown.ShouldBeTrue();
        }

        [Test]
        public void Throws_exception_if_string_is_not_empty_in_table_and_null_on_item()
        {
            var table = new Table("StringProperty");
            table.AddRow("mustard");

            var items = new[] { new SetTestObject { StringProperty = null } };

            var exceptionWasThrown = DetermineIfExceptionWasThrownByComparingThese(table, items);

            exceptionWasThrown.ShouldBeTrue();
        }

        private static bool DetermineIfExceptionWasThrownByComparingThese(Table table, SetTestObject[] items)
        {
            var exceptionWasThrown = false;
            try
            {
                table.CompareToSet(items);
            }
            catch (ComparisonException ex)
            {
                exceptionWasThrown = true;
            }
            return exceptionWasThrown;
        }
    }

    public class SetTestObject
    {
        public string StringProperty { get; set; }
        public int IntProperty { get; set; }

        public DateTime DateTimeProperty { get; set; }
    }
}