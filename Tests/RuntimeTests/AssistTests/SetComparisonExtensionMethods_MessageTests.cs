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
        public void Returns_the_names_of_any_fields_that_do_not_match()
        {
            var table = new Table("StringProperty", "AFieldThatDoesNotExist", "AnotherFieldThatDoesNotExist");

            var items = new[] { new SetTestObject() };

            var exception = GetTheExceptionThrowByComparingThese(table, items);

            exception.Message.ShouldEqual(
                @"The following fields do not exist:
AFieldThatDoesNotExist
AnotherFieldThatDoesNotExist");
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
