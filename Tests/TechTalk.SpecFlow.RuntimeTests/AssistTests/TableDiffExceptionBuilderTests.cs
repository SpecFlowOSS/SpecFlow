using System;
using System.Linq;
using NUnit.Framework;
using FluentAssertions;
using TechTalk.SpecFlow.Assist;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests
{
    [TestFixture]
    public class TableDiffExceptionBuilderTests
    {
        [Test]
        public void Adds_a_two_character_prefix_to_the_table()
        {
            var table = new Table("One", "Two", "Three");

            var builder = new TableDiffExceptionBuilder<TestObject>();

            var tableDifferenceResults = new TableDifferenceResults<TestObject>(table, new int[] {}, GetEmptyMissingItemsList());
            var message = builder.GetTheTableDiffExceptionMessage(tableDifferenceResults);

            message.AgnosticLineBreak().Should().Be(@"  | One | Two | Three |
".AgnosticLineBreak());
        }

        [Test]
        public void Prepends_a_dash_next_to_any_table_rows_that_were_missing()
        {
            var table = new Table("One", "Two", "Three");
            table.AddRow("testa", "1", "W");
            table.AddRow("testb", "2", "X");
            table.AddRow("testc", "3", "Y");
            table.AddRow("testd", "4", "Z");

            var builder = new TableDiffExceptionBuilder<TestObject>();

            var tableDifferenceResults = new TableDifferenceResults<TestObject>(table, new[] {2, 3}, GetEmptyMissingItemsList());
            var message = builder.GetTheTableDiffExceptionMessage(tableDifferenceResults);

            message.AgnosticLineBreak().Should().Be(@"  | One   | Two | Three |
  | testa | 1   | W     |
- | testb | 2   | X     |
- | testc | 3   | Y     |
  | testd | 4   | Z     |
".AgnosticLineBreak());
        }

        private TableDifferenceItem<TestObject>[] GetEmptyMissingItemsList()
        {
            return new TableDifferenceItem<TestObject>[0];
        }

        private TableDifferenceItem<TestObject>[] GetMissingItemsList(TestObject[] items)
        {
            return items.Select(i => new TableDifferenceItem<TestObject>(i)).ToArray();
        }

        [Test]
        public void Appends_remaining_items_to_the_bottom_of_the_table_with_plus_prefix()
        {
            var table = new Table("One", "Two", "Three");
            table.AddRow("testa", "1", "W");

            var builder = new TableDiffExceptionBuilder<TestObject>();

            var remainingItems = new[]
                                     {
                                         new TestObject {One = "A", Two = 1, Three = "Z"},
                                         new TestObject {One = "B1", Two = 1234567, Three = "ZYXW"}
                                     };
            var tableDifferenceResults = new TableDifferenceResults<TestObject>(table, new int[] {}, GetMissingItemsList(remainingItems));
            var message = builder.GetTheTableDiffExceptionMessage(tableDifferenceResults);

            message.AgnosticLineBreak().Should().Be(@"  | One   | Two | Three |
  | testa | 1   | W     |
+ | A | 1 | Z |
+ | B1 | 1234567 | ZYXW |
".AgnosticLineBreak());
        }

        [Test]
        public void Can_append_lines_that_contain_nulls()
        {
            var table = new Table("One", "Two", "Three");
            table.AddRow("testa", "1", "W");

            var builder = new TableDiffExceptionBuilder<TestObject>();

            var remainingItems = new[]
                                     {
                                         new TestObject {One = "A", Two = 1, Three = "Z"},
                                         new TestObject {One = null, Two = null, Three = null}
                                     };
            var tableDifferenceResults = new TableDifferenceResults<TestObject>(table, new int[] {}, GetMissingItemsList(remainingItems));
            var message = builder.GetTheTableDiffExceptionMessage(tableDifferenceResults);

            message.AgnosticLineBreak().Should().Be(@"  | One   | Two | Three |
  | testa | 1   | W     |
+ | A | 1 | Z |
+ |  |  |  |
".AgnosticLineBreak());
        }

        [Test]
        public void Uses_smart_matching_on_column_names()
        {
            var table = new Table("one", "TWO", "The fourth property");
            table.AddRow("testa", "1", "W");

            var builder = new TableDiffExceptionBuilder<TestObject>();

            var remainingItems = new[]
                                     {
                                         new TestObject {One = "A", Two = 1, TheFourthProperty = "Z"},
                                     };
            var tableDifferenceResults = new TableDifferenceResults<TestObject>(table, new int[] {}, GetMissingItemsList(remainingItems));
            var message = builder.GetTheTableDiffExceptionMessage(tableDifferenceResults);

            message.AgnosticLineBreak().Should().Be(@"  | one   | TWO | The fourth property |
  | testa | 1   | W                   |
+ | A | 1 | Z |
".AgnosticLineBreak());
        }

        [Test]
        public void It_should_report_the_enumerables_as_lists()
        {
            var table = new Table("Doubles");
            table.AddRow("1,2,3");

            var builder = new TableDiffExceptionBuilder<TestObject>();

            var remainingItems = new[]
                                     {
                                         new TestObject {Doubles = new [] {1D, 2D, 5D}},
                                     };
            var tableDifferenceResults = new TableDifferenceResults<TestObject>(table, new int[] {}, GetMissingItemsList(remainingItems));
            var message = builder.GetTheTableDiffExceptionMessage(tableDifferenceResults);

            message.Should().NotContain("System.Double[]");
            message.Should().Contain("1,2,3");
        }

        [Test]
        public void It_should_treat_nulls_as_empty_spots()
        {
            var table = new Table("Objects");
            table.AddRow("1,2,d");

            var builder = new TableDiffExceptionBuilder<TestObject>();

            var remainingItems = new[]
                                     {
                                         new TestObject {Objects = new object[] {1,null,"2","d"}},
                                     };
            var tableDifferenceResults = new TableDifferenceResults<TestObject>(table, new int[] {}, GetMissingItemsList(remainingItems));
            var message = builder.GetTheTableDiffExceptionMessage(tableDifferenceResults);

            message.Should().NotContain("Object[]");
            message.Should().Contain("1,,2,d");
        }

        public class TestObject
        {
            public string One { get; set; }
            public int? Two { get; set; }
            public string Three { get; set; }
            public string TheFourthProperty { get; set; }
            public double[] Doubles { get; set; }
            public object[] Objects { get; set; }
        }

        public class TestObjectWithArrays
        {
            public string Name { get; set; }
            public int[] Numbers { get; set; }
            public string[] Strings { get; set; }
        }
    }
}