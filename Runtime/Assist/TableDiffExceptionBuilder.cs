using System;
using System.Linq;
using System.Text;

namespace TechTalk.SpecFlow.Assist
{
    public class TableDiffExceptionBuilder<T>
    {
        public string GetTheTableDiffExceptionMessage(TableDifferenceResults<T> tableDifferenceResults)
        {
            var realData = new StringBuilder();
            var index = 0;
            var everyLineInTheTable = tableDifferenceResults.Table.ToString().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in everyLineInTheTable)
            {
                var prefix = "  ";
                if (tableDifferenceResults.IndexesOfTableRowsThatWereNotMatched.Contains(index))
                    prefix = "- ";
                realData.AppendLine(prefix + line);
                index++;
            }

            foreach (var item in  tableDifferenceResults.ItemsInTheDataThatWereNotFoundInTheTable)
            {
                var line = "+ |";
                foreach (var header in tableDifferenceResults.Table.Header)
                    line += string.Format(" {0} |", item.GetPropertyValue(header));
                realData.AppendLine(line);
            }

            return realData.ToString();
        }
    }
}