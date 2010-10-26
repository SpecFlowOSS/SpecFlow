using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Should;
using TechTalk.SpecFlow.Assist;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests
{
    [TestFixture]
    public class SetComparisonExtensionMethods_MessageTests
    {
        [Test]
        public void Returns_the_names_of_any_fields_that_do_not_exist()
        {
            var table = new Table("StringProperty", "AFieldThatDoesNotExist", "AnotherFieldThatDoesNotExist");

            var items = new[] { new SetTestObject() };

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

            var items = new[] { new SetTestObject()};

            var exception = GetTheExceptionThrowByComparingThese(table, items);

            exception.Message.ShouldEqual(
                @"There was 1 result when expecting no results.");
        }

        [Test]
        public void Returns_descriptive_message_when_two_results_exist_but_there_should_be_no_results()
        {
            var table = new Table("StringProperty");

            var items = new[] { new SetTestObject(), new SetTestObject() };

            var exception = GetTheExceptionThrowByComparingThese(table, items);

            exception.Message.ShouldEqual(
                @"There were 2 results when expecting no results.");
        }

        private ComparisonException GetTheExceptionThrowByComparingThese(Table table, SetTestObject[] items)
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
