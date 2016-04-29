using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist
{
    public class TableDifferenceResults<TT>
    {
        public TableDifferenceResults(Table table, IEnumerable<int> indexesOfTableRowsThatWereNotMatched, IEnumerable<TT> itemsInTheDataThatWereNotFoundInTheTable)
        {
            this.Table = table;
            this.IndexesOfTableRowsThatWereNotMatched = indexesOfTableRowsThatWereNotMatched;
            this.ItemsInTheDataThatWereNotFoundInTheTable = itemsInTheDataThatWereNotFoundInTheTable;
        }

        public Table Table { get; }

        public IEnumerable<int> IndexesOfTableRowsThatWereNotMatched { get; }

        public IEnumerable<TT> ItemsInTheDataThatWereNotFoundInTheTable { get; }
    }
}