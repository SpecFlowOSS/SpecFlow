using System;
using System.Linq;

namespace TechTalk.SpecFlow.Assist
{
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

            var instanceTable = new Table("Field", "Value");
            foreach (var header in table.Header)
                instanceTable.AddRow(header, table.Rows[index][header]);

            return instanceTable;
        }
    }
}