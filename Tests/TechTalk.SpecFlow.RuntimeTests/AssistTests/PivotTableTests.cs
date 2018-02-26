using System;
using System.Linq;
using Xunit;
using TechTalk.SpecFlow.Assist;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests
{
    
    public class PivotTableTests
    {
        [Fact]
        public void Returns_a_table_when_asked_for_an_instance_of_the_first_item_in_a_set()
        {
            var setTable = new Table("Col1");
            setTable.AddRow("first row");

            var instanceTable = PivotThisTable(setTable, 0);
            Assert.NotNull(instanceTable);
        }

        [Fact]
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
            Assert.NotNull(exception);
            Assert.Equal(typeof (IndexOutOfRangeException), exception.GetType());
        }

        [Fact]
        public void The_first_row_in_the_table_is_Field()
        {
            var setTable = new Table("Col1");
            setTable.AddRow("first row");

            var instanceTable = PivotThisTable(setTable, 0);

            Assert.Equal("Field", instanceTable.Header.First());
        }

        [Fact]
        public void The_second_row_in_the_table_is_Value()
        {
            var setTable = new Table("Col1");
            setTable.AddRow("first row");

            var instanceTable = PivotThisTable(setTable, 0);

            Assert.Equal("Value", instanceTable.Header.ToArray()[1]);
        }

        [Fact]
        public void The_first_row_contains_the_key_and_value_of_the_first_column_in_the_set()
        {
            var setTable = new Table("Col1");
            setTable.AddRow("Val1");

            var instanceTable = PivotThisTable(setTable, 0);

            Assert.Equal("Col1", instanceTable.Rows.Single()["Field"]);
            Assert.Equal("Val1", instanceTable.Rows.Single()["Value"]);
        }

        [Fact]
        public void The_second_row_contains_the_key_and_value_of_the_second_column_in_the_set()
        {
            var setTable = new Table("Col1", "Col2");
            setTable.AddRow("Val1", "Val2");

            var instanceTable = PivotThisTable(setTable, 0);

            Assert.Equal("Col2", instanceTable.Rows[1]["Field"]);
            Assert.Equal("Val2", instanceTable.Rows[1]["Value"]);
        }

        [Fact]
        public void Pulls_values_from_the_second_row_if_the_index_is_1()
        {
            var setTable = new Table("Col1", "Col2");
            setTable.AddRow("Val1", "Val2");
            setTable.AddRow("expected1", "expected2");

            var instanceTable = PivotThisTable(setTable, 1);

            Assert.Equal("expected1", instanceTable.Rows[0]["Value"]);
            Assert.Equal("expected2", instanceTable.Rows[1]["Value"]);
        }

        private static Table PivotThisTable(Table setTable, int index)
        {
            var pivotTable = new PivotTable(setTable);
            return pivotTable.GetInstanceTable(index);
        }
    }
}