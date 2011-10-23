using NUnit.Framework;
using Should;
using TechTalk.SpecFlow.Assist;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests
{
    [TestFixture]
    public class TableDiffExceptionBuilderTests
    {
        [Test]
        public void Returning_a_table_with_no_missing_items_or_remaining_data_returns_the_table_with_two_character_prefix()
        {
            var table = new Table("One", "Two", "Three");

            var builder = new TableDiffExceptionBuilder<TestObject>();

            var message = builder.GetTheTableDiffExceptionMessage(new TableDifferenceResults<TestObject>(table, new int[] {}, new TestObject[] {}));

            message.ShouldEqual(@"  | One | Two | Three |
");
        }

        public class TestObject
        {
            public string One { get; set; }
            public string Two { get; set; }
            public string Three { get; set; }
        }
    }
}