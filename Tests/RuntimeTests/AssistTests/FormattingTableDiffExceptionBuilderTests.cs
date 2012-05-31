using Moq;
using NUnit.Framework;
using Should;
using TechTalk.SpecFlow.Assist;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests
{
    [TestFixture]
    public class FormattingTableDiffExceptionBuilderTests
    {
        [Test]
        public void Returns_null_if_parent_passes_null()
        {
            var tableDifferenceResults = GetATestDiffResult();

            var builder = GetABuilderThatReturnsThisString(null, tableDifferenceResults);

            var result = builder.GetTheTableDiffExceptionMessage(tableDifferenceResults);

            result.ShouldBeNull();
        }

        [Test]
        public void Returns_empty_if_parent_returns_empty()
        {
            var tableDifferenceResults = GetATestDiffResult();

            var builder = GetABuilderThatReturnsThisString("", tableDifferenceResults);

            var result = builder.GetTheTableDiffExceptionMessage(tableDifferenceResults);

            result.ShouldEqual("");
        }

        [Test]
        public void Makes_width_of_columns_match_and_adds_space_for_preceding_characters()
        {
            var tableDifferenceResults = GetATestDiffResult();

            var builder = GetABuilderThatReturnsThisString(@"| One | Two | Three |
  | 1234567 | 1 | 1234567890 |
+ | 1| 2 | 3 |", tableDifferenceResults);

            var result = builder.GetTheTableDiffExceptionMessage(tableDifferenceResults);

            result.ShouldEqual(@"  | One     | Two | Three      |
  | 1234567 | 1   | 1234567890 |
+ | 1       | 2   | 3          |
");
        }

        private static FormattingTableDiffExceptionBuilder<TestObject> GetABuilderThatReturnsThisString(string returnValue, TableDifferenceResults<TestObject> tableDifferenceResults)
        {
            var parentFake = new Mock<ITableDiffExceptionBuilder<TestObject>>();
            parentFake.Setup(x => x.GetTheTableDiffExceptionMessage(tableDifferenceResults))
                .Returns(() => returnValue);

            return new FormattingTableDiffExceptionBuilder<TestObject>(parentFake.Object);
        }

        private static TableDifferenceResults<TestObject> GetATestDiffResult()
        {
            return new TableDifferenceResults<TestObject>(null, null, null);
        }

        public class TestObject
        {
        }
    }
}