using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using FluentAssertions;
using Xunit;
using TechTalk.SpecFlow.Assist;
using TechTalk.SpecFlow.RuntimeTests.AssistTests.TestInfrastructure;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests
{
    public abstract class SetComparisonExtensionMethods_MessageTests
    {
        [Fact]
        public void Returns_the_names_of_any_fields_that_do_not_exist()
        {
            var table = new Table("StringProperty", "AFieldThatDoesNotExist", "AnotherFieldThatDoesNotExist");

            var items = new[] {new SetComparisonTestObject()};

            var exception = GetTheExceptionThrowByComparingThese(table, items);

            exception.Message.AgnosticLineBreak().Should().Be(
                @"The following fields do not exist:
AFieldThatDoesNotExist
AnotherFieldThatDoesNotExist".AgnosticLineBreak());
        }

        [Fact]
        public void Returns_descriptive_message_when_one_result_exists_but_there_should_be_no_results()
        {
            var table = new Table("StringProperty");

            var items = new[] {new SetComparisonTestObject()};

            var exception = GetTheExceptionThrowByComparingThese(table, items);

            exception.Message.AgnosticLineBreak().Should().Be(
                @"
  | StringProperty |
+ |                |
".AgnosticLineBreak());
        }

        [Fact]
        public void Returns_descriptive_message_when_two_results_exist_but_there_should_be_no_results()
        {
            var table = new Table("StringProperty");

            var items = new[] {new SetComparisonTestObject(), new SetComparisonTestObject()};

            var exception = GetTheExceptionThrowByComparingThese(table, items);

            exception.Message.AgnosticLineBreak().Should().Be(
                @"
  | StringProperty |
+ |                |
+ |                |
".AgnosticLineBreak());
        }

        [Fact]
        public void Returns_1_as_the_missing_item_when_only_one_item_exists()
        {
            var table = new Table("StringProperty");
            table.AddRow("orange");

            var items = new[] {new SetComparisonTestObject {StringProperty = "apple"}};

            var exception = GetTheExceptionThrowByComparingThese(table, items);

            exception.Message.AgnosticLineBreak().Should().Be(
@"
  | StringProperty |
- | orange         |
+ | apple          |
".AgnosticLineBreak());
        }

        [Fact]
        public void Returns_2_as_the_missing_item_when_the_second_item_does_not_match()
        {
            var table = new Table("StringProperty");
            table.AddRow("orange");
            table.AddRow("apple");

            var items = new[]
                            {
                                new SetComparisonTestObject {StringProperty = "orange"},
                                new SetComparisonTestObject {StringProperty = "rotten apple"}
                            };

            var exception = GetTheExceptionThrowByComparingThese(table, items);

            exception.Message.AgnosticLineBreak().Should().Be(
                @"
  | StringProperty |
  | orange         |
- | apple          |
+ | rotten apple   |
".AgnosticLineBreak());
        }

        [Fact]
        public void Returns_a_descriptive_error_when_three_results_exist_when_two_expected()
        {
            var table = new Table("StringProperty");
            table.AddRow("orange");
            table.AddRow("apple");

            var items = new[]
                            {
                                new SetComparisonTestObject {StringProperty = "orange"},
                                new SetComparisonTestObject {StringProperty = "apple"},
                                new SetComparisonTestObject {StringProperty = "extra row"}
                            };

            var exception = GetTheExceptionThrowByComparingThese(table, items);

            exception.Message.AgnosticLineBreak().Should().Be(@"
  | StringProperty |
  | orange         |
  | apple          |
+ | extra row      |
".AgnosticLineBreak());
        }

        [Fact]
        public void Returns_a_descriptive_error_when_three_results_exist_when_one_expected()
        {
            var table = new Table("StringProperty");
            table.AddRow("orange");

            var items = new[]
                            {
                                new SetComparisonTestObject {StringProperty = "orange"},
                                new SetComparisonTestObject {StringProperty = "apple"},
                                new SetComparisonTestObject {StringProperty = "banana"}
                            };

            var exception = GetTheExceptionThrowByComparingThese(table, items);

            exception.Message.AgnosticLineBreak().Should().Be(@"
  | StringProperty |
  | orange         |
+ | apple          |
+ | banana         |
".AgnosticLineBreak());
        }

        [Fact]
        public void Returns_a_descriptive_error_when_no_results_exist_when_one_expected()
        {
            var table = new Table("StringProperty");
            table.AddRow("orange");

            var items = new SetComparisonTestObject[] { };

            var exception = GetTheExceptionThrowByComparingThese(table, items);

            exception.Message.AgnosticLineBreak().Should().Be(@"
  | StringProperty |
- | orange         |
".AgnosticLineBreak());
        }

        [SkippableFact]
        public void Includes_milliseconds_and_ticks_in_error_for_date_time_fields()
        {
            //Skip if not Windows -> .NET Core 2.1 on Linux converts year from 2018 to 18, thus resulting in an error when comparing
            Skip.IfNot(RuntimeInformation.IsOSPlatform(OSPlatform.Windows));

            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);

            var table = new Table("DateTimeProperty");
            table.AddRow("3/28/2018 12:34:56 AM");

            var items = new[]
            {
                new SetComparisonTestObject
                {
                    DateTimeProperty = new System.DateTime(2018, 3, 28, 0, 34, 56, 78).AddTicks(9)
                }
            };

            var exception = GetTheExceptionThrowByComparingThese(table, items);

            exception.Message.AgnosticLineBreak().Should().Be(@"
  | DateTimeProperty              |
- | 3/28/2018 12:34:56 AM         |
+ | 3/28/2018 12:34:56.0780009 AM |
".AgnosticLineBreak());
        }

        protected ComparisonException GetTheExceptionThrowByComparingThese(Table table, SetComparisonTestObject[] items)
        {
            try
            {
                CallComparison(table, items);
            }
            catch (ComparisonException ex)
            {
                return ex;
            }
            return null;
        }

        protected abstract void CallComparison(Table table, SetComparisonTestObject[] items);
    }

    
    public class SetComparisonExtensionMethods_OrderInsensitive_MessageTests : SetComparisonExtensionMethods_MessageTests
    {
        [Fact]
        public void Returns_both_1_and_two_as_the_missing_items_when_both_cannot_be_found()
        {
            var table = new Table("StringProperty");
            table.AddRow("orange");
            table.AddRow("apple");

            var items = new[]
                            {
                                new SetComparisonTestObject {StringProperty = "rotten orange"},
                                new SetComparisonTestObject {StringProperty = "rotten apple"}
                            };

            var exception = GetTheExceptionThrowByComparingThese(table, items);

            exception.Message.AgnosticLineBreak().Should().Be(
                @"
  | StringProperty |
- | orange         |
- | apple          |
+ | rotten orange  |
+ | rotten apple   |
".AgnosticLineBreak());
        }

        protected override void CallComparison(Table table, SetComparisonTestObject[] items)
        {
            table.CompareToSet(items);
        }
    }

    
    public class SetComparisonExtensionMethods_OrderSensitive_MessageTests : SetComparisonExtensionMethods_MessageTests
    {
        [Fact]
        public void Returns_both_1_and_two_as_the_missing_items_when_both_cannot_be_found()
        {
            var table = new Table("StringProperty");
            table.AddRow("orange");
            table.AddRow("apple");

            var items = new[]
                            {
                                new SetComparisonTestObject {StringProperty = "rotten orange"},
                                new SetComparisonTestObject {StringProperty = "rotten apple"}
                            };

            var exception = GetTheExceptionThrowByComparingThese(table, items);

            exception.Message.AgnosticLineBreak().Should().Be(
                @"
  | StringProperty |
- | orange         |
+ | rotten orange  |
- | apple          |
+ | rotten apple   |
".AgnosticLineBreak());
        }

        protected override void CallComparison(Table table, SetComparisonTestObject[] items)
        {
            table.CompareToSet(items, sequentialEquality: true);
        }
    }
}