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

            var stringBuilder = new StringBuilder();

            var lines = message.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(x => x.Split('|').ToArray());

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

            foreach (var line in GetLines(message))
                stringBuilder.AppendLine(ReformatLineUsingTheseColumnLengths(dictionary, line));

            return stringBuilder.ToString();
        }

        private static string ReformatLineUsingTheseColumnLengths(Dictionary<int, int> dictionary, string line)
        {
            var eachColumn = line.Split('|').ToArray().Select((value, index) => new {value, index}).ToArray();
            var eachColumnPaddedToTheRightLength = eachColumn.Select(x => x.value.PadRight(dictionary[x.index])).ToArray();
            var newLine = string.Join("|", eachColumnPaddedToTheRightLength);
            return newLine.TrimEnd();
        }

        private static string[] GetLines(string message)
        {
            return message.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        }
    }
}