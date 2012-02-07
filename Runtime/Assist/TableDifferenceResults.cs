using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist
{
    public class TableDifferenceResults<TT>
    {
        private readonly Table table;
        private readonly IEnumerable<int> indexesOfTableRowsThatWereNotMatched;
        private readonly IEnumerable<TT> itemsInTheDataThatWereNotFoundInTheTable;

        public TableDifferenceResults(Table table, IEnumerable<int> indexesOfTableRowsThatWereNotMatched, IEnumerable<TT> itemsInTheDataThatWereNotFoundInTheTable)
        {
            this.table = table;
            this.indexesOfTableRowsThatWereNotMatched = indexesOfTableRowsThatWereNotMatched;
            this.itemsInTheDataThatWereNotFoundInTheTable = itemsInTheDataThatWereNotFoundInTheTable;
        }

        public Table Table
        {
            get { return table; }
        }

        public IEnumerable<int> IndexesOfTableRowsThatWereNotMatched
        {
            get { return indexesOfTableRowsThatWereNotMatched; }
        }

        public IEnumerable<TT> ItemsInTheDataThatWereNotFoundInTheTable
        {
            get { return itemsInTheDataThatWereNotFoundInTheTable; }
        }
    }
}