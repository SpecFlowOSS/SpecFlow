using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TechTalk.SpecFlow.Assist
{
    internal class FormattingTableDiffExceptionBuilder<T> : ITableDiffExceptionBuilder<T>
    {
        private readonly ITableDiffExceptionBuilder<T> parent;

        public FormattingTableDiffExceptionBuilder(ITableDiffExceptionBuilder<T> parent)
        {
            this.parent = parent;
        }

        public string GetTheTableDiffExceptionMessage(TableDifferenceResults<T> tableDifferenceResults)
        {
            var message = parent.GetTheTableDiffExceptionMessage(tableDifferenceResults);

            if (string.IsNullOrEmpty(message)) return message;

            var dictionary = GetTheMaximumLengthsOfEachColumn(message);

            var stringBuilder = new StringBuilder();
            foreach (var line in GetLines(message))
                stringBuilder.AppendLine(ReformatLineUsingTheseColumnLengths(dictionary, line));

            return stringBuilder.ToString();
        }

        private Dictionary<int, int> GetTheMaximumLengthsOfEachColumn(string message)
        {
            var lines = GetLines(message);

            var dictionary = new Dictionary<int, int>();
            foreach (var block in GetIndexedColumns(lines))
                SetMaximumLengthForThisColumn(dictionary, block.index, block.value.Length);

            return dictionary;
        }

        private static void SetMaximumLengthForThisColumn(IDictionary<int, int> dictionary, int index, int length)
        {
            if (dictionary.ContainsKey(index) == false) dictionary[index] = 2;
            dictionary[index] = dictionary[index] > length ? dictionary[index] : length + 2;
        }

        private static IEnumerable<IndexedColumn> GetIndexedColumns(IEnumerable<string> lines)
        {
            return lines.Select(x => x.Split('|'))
                .SelectMany(column => column.Select(x => (x ?? string.Empty).Trim()).ToArray()
                                          .Select((value, index) => new IndexedColumn {value = value, index = index}));
        }

        private static string ReformatLineUsingTheseColumnLengths(IDictionary<int, int> dictionary, string line)
        {
            var eachColumn = line.Split('|').Select((value, index) => new {value, index});
            var eachColumnPaddedToTheRightLength = eachColumn.Select(x => x.value.PadRight(dictionary[x.index]));
            var newLine = string.Join("|", eachColumnPaddedToTheRightLength.ToArray());
            return newLine.TrimEnd();
        }

        private static IEnumerable<string> GetLines(string message)
        {
            return message.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        }

        private class IndexedColumn
        {
            public int index { get; set; }
            public string value { get; set; }
        }
    }
}