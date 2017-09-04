using Moq;
using Xunit;
using FluentAssertions;
using TechTalk.SpecFlow.Assist;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests
{
    
    public class FormattingTableDiffExceptionBuilderTests
    {
        [Fact]
        public void Returns_null_if_parent_passes_null()
        {
            var tableDifferenceResults = GetATestDiffResult();

            var builder = GetABuilderThatReturnsThisString(null, tableDifferenceResults);

            var result = builder.GetTheTableDiffExceptionMessage(tableDifferenceResults);

            result.Should().Be(null);
        }

        [Fact]
        public void Returns_empty_if_parent_returns_empty()
        {
            var tableDifferenceResults = GetATestDiffResult();

            var builder = GetABuilderThatReturnsThisString("", tableDifferenceResults);

            var result = builder.GetTheTableDiffExceptionMessage(tableDifferenceResults);

            result.Should().Be("");
        }

        [Fact]
        public void Makes_width_of_columns_match_and_adds_space_for_preceding_characters()
        {
            var tableDifferenceResults = GetATestDiffResult();

            var builder = GetABuilderThatReturnsThisString(@"| One | Two | Three |
  | 1234567 | 1 | 1234567890 |
+ | 1| 2 | 3 |", tableDifferenceResults);

            var result = builder.GetTheTableDiffExceptionMessage(tableDifferenceResults);

            result.AgnosticLineBreak().Should().Be(@"  | One     | Two | Three      |
  | 1234567 | 1   | 1234567890 |
+ | 1       | 2   | 3          |
".AgnosticLineBreak());
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