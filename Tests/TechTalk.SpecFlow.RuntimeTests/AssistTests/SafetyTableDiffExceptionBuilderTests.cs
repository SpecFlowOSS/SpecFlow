using System;
using Moq;
using Xunit;
using FluentAssertions;
using TechTalk.SpecFlow.Assist;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests
{
    
    public class SafetyTableDiffExceptionBuilderTests
    {
        [Fact]
        public void Returns_expected_results_when_that_is_returned_from_the_parent()
        {
            var parentResults = new TableDifferenceResults<TestClass>(null, null, null);

            var fakeBuilder = new Mock<ITableDiffExceptionBuilder<TestClass>>();
            fakeBuilder.Setup(x => x.GetTheTableDiffExceptionMessage(parentResults))
                .Returns("Expected Results");

            var builder = new SafetyTableDiffExceptionBuilder<TestClass>(fakeBuilder.Object);

            var result = builder.GetTheTableDiffExceptionMessage(parentResults);

            result.Should().Be("Expected Results");
        }

        [Fact]
        public void Returns_expected_results_times_two_when_that_is_returned_from_the_parent()
        {
            var parentResults = new TableDifferenceResults<TestClass>(null, null, null);

            var fakeBuilder = new Mock<ITableDiffExceptionBuilder<TestClass>>();
            fakeBuilder.Setup(x => x.GetTheTableDiffExceptionMessage(parentResults))
                .Returns("Expected Results Times Two");

            var builder = new SafetyTableDiffExceptionBuilder<TestClass>(fakeBuilder.Object);

            var result = builder.GetTheTableDiffExceptionMessage(parentResults);

            result.Should().Be("Expected Results Times Two");
        }

        [Fact]
        public void Returns_a_special_warning_message_if_the_parent_throws_an_exception()
        {
            var parentResults = new TableDifferenceResults<TestClass>(null, null, null);

            var fakeBuilder = new Mock<ITableDiffExceptionBuilder<TestClass>>();
            fakeBuilder.Setup(x => x.GetTheTableDiffExceptionMessage(parentResults))
                .Throws(new Exception("the parent failed"));

            var builder = new SafetyTableDiffExceptionBuilder<TestClass>(fakeBuilder.Object);

            var result = builder.GetTheTableDiffExceptionMessage(parentResults);

            result.Should().Be("The table and the set do not match.");
        }

        public class TestClass{}
    }
}