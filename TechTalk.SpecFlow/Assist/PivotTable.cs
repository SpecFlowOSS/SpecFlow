using System;
using System.Linq;

namespace TechTalk.SpecFlow.Assist
{
    internal class PivotTable
    {
        private readonly Table table;

        public PivotTable(Table table)
        {
            this.table = table;
        }

        public Table GetInstanceTable(int index)
        {
            AssertThatThisItemExistsInTheSet(index);

            return CreateAnInstanceTableForThisItemInTheSet(index);
        }

        private Table CreateAnInstanceTableForThisItemInTheSet(int index)
        {
            var instanceTable = new Table("Field", "Value");
            foreach (var header in table.Header)
                instanceTable.AddRow(header, table.Rows[index][header]);
            return instanceTable;
        }

        private void AssertThatThisItemExistsInTheSet(int index)
        {
            if (table.Rows.Count() <= index)
                throw new IndexOutOfRangeException();
        }
    }
}