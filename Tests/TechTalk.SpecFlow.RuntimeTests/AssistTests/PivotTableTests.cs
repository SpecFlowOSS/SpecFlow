using System;
using System.Linq;
using NUnit.Framework;
using TechTalk.SpecFlow.Assist;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests
{
    [TestFixture]
    public class PivotTableTests
    {
        [Test]
        public void Returns_a_table_when_asked_for_an_instance_of_the_first_item_in_a_set()
        {
            var setTable = new Table("Col1");
            setTable.AddRow("first row");

            var instanceTable = PivotThisTable(setTable, 0);
            Assert.IsNotNull(instanceTable);
        }

        [Test]
        public void Throws_an_out_of_index_exception_when_asked_for_an_instance_that_does_not_exist()
        {
            var setTable = new Table("Col1");
            setTable.AddRow("first row");

            Exception exception = null;
            try
            {
                PivotThisTable(setTable, 1);
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            Assert.IsNotNull(exception);
            Assert.AreEqual(typeof (IndexOutOfRangeException), exception.GetType());
        }

        [Test]
        public void The_first_row_in_the_table_is_Field()
        {
            var setTable = new Table("Col1");
            setTable.AddRow("first row");

            var instanceTable = PivotThisTable(setTable, 0);

            Assert.AreEqual("Field", instanceTable.Header.First());
        }

        [Test]
        public void The_second_row_in_the_table_is_Value()
        {
            var setTable = new Table("Col1");
            setTable.AddRow("first row");

            var instanceTable = PivotThisTable(setTable, 0);

            Assert.AreEqual("Value", instanceTable.Header.ToArray()[1]);
        }

        [Test]
        public void The_first_row_contains_the_key_and_value_of_the_first_column_in_the_set()
        {
            var setTable = new Table("Col1");
            setTable.AddRow("Val1");

            var instanceTable = PivotThisTable(setTable, 0);

            Assert.AreEqual("Col1", instanceTable.Rows.Single()["Field"]);
            Assert.AreEqual("Val1", instanceTable.Rows.Single()["Value"]);
        }

        [Test]
        public void The_second_row_contains_the_key_and_value_of_the_second_column_in_the_set()
        {
            var setTable = new Table("Col1", "Col2");
            setTable.AddRow("Val1", "Val2");

            var instanceTable = PivotThisTable(setTable, 0);

            Assert.AreEqual("Col2", instanceTable.Rows[1]["Field"]);
            Assert.AreEqual("Val2", instanceTable.Rows[1]["Value"]);
        }

        [Test]
        public void Pulls_values_from_the_second_row_if_the_index_is_1()
        {
            var setTable = new Table("Col1", "Col2");
            setTable.AddRow("Val1", "Val2");
            setTable.AddRow("expected1", "expected2");

            var instanceTable = PivotThisTable(setTable, 1);

            Assert.AreEqual("expected1", instanceTable.Rows[0]["Value"]);
            Assert.AreEqual("expected2", instanceTable.Rows[1]["Value"]);
        }

        private static Table PivotThisTable(Table setTable, int index)
        {
            var pivotTable = new PivotTable(setTable);
            return pivotTable.GetInstanceTable(index);
        }
    }
}