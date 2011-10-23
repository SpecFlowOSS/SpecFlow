using NUnit.Framework;
using Should;
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

            var tableDifferenceResults = new TableDifferenceResults<TestObject>(table, new int[] {}, new TestObject[] {});
            var message = builder.GetTheTableDiffExceptionMessage(tableDifferenceResults);

            message.ShouldEqual(@"  | One | Two | Three |
");
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

            var tableDifferenceResults = new TableDifferenceResults<TestObject>(table, new[] {2, 3}, new TestObject[] { });
            var message = builder.GetTheTableDiffExceptionMessage(tableDifferenceResults);

            message.ShouldEqual(@"  | One   | Two | Three |
  | testa | 1   | W     |
- | testb | 2   | X     |
- | testc | 3   | Y     |
  | testd | 4   | Z     |
");
        }

        public class TestObject
        {
            public string One { get; set; }
            public int Two { get; set; }
            public string Three { get; set; }
        }
    }
}