using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TechTalk.SpecFlow.Assist
{
    public class TableDiffExceptionBuilder<T>
    {
        public string GetTheTableDiffExceptionMessage(IEnumerable<int> listOfMissingItems, IEnumerable<T> itemsThatWereNotFoundInTheTable, Table table)
        {
            return "  " + table;
            //var realData = new StringBuilder();
            //var index = 0;
            //var everyLineInTheTable = table.ToString().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            //foreach (var line in everyLineInTheTable)
            //{
            //    var prefix = "  ";
            //    if (listOfMissingItems.Contains(index))
            //        prefix = "- ";
            //    realData.AppendLine(prefix + line);
            //    index++;
            //}
            //foreach (var item in itemsThatWereNotFoundInTheTable)
            //    realData.AppendLine(string.Format("+ | {0} |", ((item.GetPropertyValue("StringProperty") as string) ?? "").PadRight(14)));
            //return realData.ToString();
        }
    }
}