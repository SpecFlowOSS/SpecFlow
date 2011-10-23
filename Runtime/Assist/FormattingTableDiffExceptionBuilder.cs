using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TechTalk.SpecFlow.Assist
{
    public class FormattingTableDiffExceptionBuilder<T> : ITableDiffExceptionBuilder<T>
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
            var lines = GetLines(message).Select(x => x.Split('|'));

            var dictionary = new Dictionary<int, int>();
            foreach (var line in lines)
            {
                var index = 0;
                foreach (var block in line.Select(x => (x ?? string.Empty).Trim()))
                {
                    if (dictionary.ContainsKey(index) == false) dictionary[index] = 0;
                    dictionary[index] = dictionary[index] > block.Length ? dictionary[index] : block.Length + 2;
                    index++;
                }
            }

            dictionary[0] = 2;
            return dictionary;
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
    }
}