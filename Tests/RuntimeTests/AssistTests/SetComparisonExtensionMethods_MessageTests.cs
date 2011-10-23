using NUnit.Framework;
using Should;
using TechTalk.SpecFlow.Assist;
using TechTalk.SpecFlow.RuntimeTests.AssistTests.TestInfrastructure;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests
{
    [TestFixture]
    public class SetComparisonExtensionMethods_MessageTests
    {
        [Test]
        public void Returns_the_names_of_any_fields_that_do_not_exist()
        {
            var table = new Table("StringProperty", "AFieldThatDoesNotExist", "AnotherFieldThatDoesNotExist");

            var items = new[] {new SetComparisonTestObject()};

            var exception = GetTheExceptionThrowByComparingThese(table, items);

            exception.Message.ShouldEqual(
                @"The following fields do not exist:
AFieldThatDoesNotExist
AnotherFieldThatDoesNotExist");
        }

        [Test]
        public void Returns_descriptive_message_when_one_result_exists_but_there_should_be_no_results()
        {
            var table = new Table("StringProperty");

            var items = new[] {new SetComparisonTestObject()};

            var exception = GetTheExceptionThrowByComparingThese(table, items);

            exception.Message.ShouldEqual(
                @"
  | StringProperty |
+ |                |
");
        }

        [Test]
        public void Returns_descriptive_message_when_two_results_exist_but_there_should_be_no_results()
        {
            var table = new Table("StringProperty");

            var items = new[] {new SetComparisonTestObject(), new SetComparisonTestObject()};

            var exception = GetTheExceptionThrowByComparingThese(table, items);

            exception.Message.ShouldEqual(
                @"
  | StringProperty |
+ |                |
+ |                |
");
        }

        [Test]
        public void Returns_1_as_the_missing_item_when_only_one_item_exists()
        {
            var table = new Table("StringProperty");
            table.AddRow("orange");

            var items = new[] {new SetComparisonTestObject {StringProperty = "apple"}};

            var exception = GetTheExceptionThrowByComparingThese(table, items);

            exception.Message.ShouldEqual(
@"
  | StringProperty |
- | orange         |
+ | apple          |
");
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

            exception.Message.ShouldEqual(
                @"
  | StringProperty |
  | orange         |
- | apple          |
+ | rotten apple   |
");
        }

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

            exception.Message.ShouldEqual(
                @"
  | StringProperty |
- | orange         |
- | apple          |
+ | rotten orange  |
+ | rotten apple   |
");
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

            exception.Message.ShouldEqual(@"
  | StringProperty |
  | orange         |
  | apple          |
+ | extra row      |
");
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

            exception.Message.ShouldEqual(@"
  | StringProperty |
  | orange         |
+ | apple          |
+ | banana         |
");
        }

        private static ComparisonException GetTheExceptionThrowByComparingThese(Table table, SetComparisonTestObject[] items)
        {
            try
            {
                table.CompareToSet(items);
            }
            catch (ComparisonException ex)
            {
                return ex;
            }
            return null;
        }
    }
}