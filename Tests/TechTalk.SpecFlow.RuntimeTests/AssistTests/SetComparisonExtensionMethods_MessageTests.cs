using FluentAssertions;
using NUnit.Framework;
using TechTalk.SpecFlow.Assist;
using TechTalk.SpecFlow.RuntimeTests.AssistTests.TestInfrastructure;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests
{
    public abstract class SetComparisonExtensionMethods_MessageTests
    {
        [Test]
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

        [Test]
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

        [Test]
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

        [Test]
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

        [Test]
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

        [Test]
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

        [Test]
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

    [TestFixture]
    public class SetComparisonExtensionMethods_OrderInsensitive_MessageTests : SetComparisonExtensionMethods_MessageTests
    {
        [Test]
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

    [TestFixture]
    public class SetComparisonExtensionMethods_OrderSensitive_MessageTests : SetComparisonExtensionMethods_MessageTests
    {
        [Test]
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