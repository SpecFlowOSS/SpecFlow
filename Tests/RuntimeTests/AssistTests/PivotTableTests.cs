using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

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
            } catch(Exception ex)
            {
                exception = ex;
            }
            Assert.IsNotNull(exception);
            Assert.AreEqual(typeof (IndexOutOfRangeException), exception.GetType());
        }

        private Table PivotThisTable(Table setTable, int index)
        {
            var pivotTable = new PivotTable(setTable);
            return pivotTable.GetInstanceTable(index);
        }

    }

    public class PivotTable
    {
        private readonly Table table;

        public PivotTable(Table table)
        {
            this.table = table;
        }

        public Table GetInstanceTable(int index)
        {
            if (table.Rows.Count() <= index)
                throw new IndexOutOfRangeException();
            return new Table("x");
        }
    }
}
