using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TechTalk.SpecFlow.Assist
{
    public interface ITableDiffExceptionBuilder<T>
    {
        string GetTheTableDiffExceptionMessage(TableDifferenceResults<T> tableDifferenceResults);
    }

    internal class TableDiffExceptionBuilder<T> : ITableDiffExceptionBuilder<T>
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

            foreach (var item in tableDifferenceResults.ItemsInTheDataThatWereNotFoundInTheTable)
            {
                var line = "+ |";
                foreach (var header in tableDifferenceResults.Table.Header)
                    line += $" {GetTheValue(item, header)} |";
                realData.AppendLine(line);
            }

            return realData.ToString();
        }

        private static object GetTheValue(T item, string header)
        {
            var propertyValue = item.GetPropertyValue(header);
            return ThisIsAnEnumerable(propertyValue)
                ? ConvertThisEnumerableToACommaDelimitedString(propertyValue)
                : propertyValue;
        }

        private static string ConvertThisEnumerableToACommaDelimitedString(object propertyValue)
        {
            return string.Join(",", ((IEnumerable) propertyValue).Cast<object>().Select(x => x?.ToString() ?? ""));
        }

        private static bool ThisIsAnEnumerable(object propertyValue)
        {
            return propertyValue is IEnumerable && !(propertyValue is string);
        }
    }
}