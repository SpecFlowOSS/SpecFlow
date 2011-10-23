using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TechTalk.SpecFlow.Assist
{
    public interface ITableDiffExceptionBuilder<T>
    {
        string GetTheTableDiffExceptionMessage(TableDifferenceResults<T> tableDifferenceResults);
    }

    public class TableDiffExceptionBuilder<T> : ITableDiffExceptionBuilder<T>
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

            var lines = message.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Split('|').ToArray());

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

            foreach (var line in message.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
            {
                var data = "";

                var blocks = line.Split('|');

                var index = 0;
                var precedingCharacter = "";
                foreach (var block in blocks)
                {
                    data += string.Format("{1}{0}", block.PadRight(dictionary[index]), precedingCharacter);
                    index++;
                    precedingCharacter = "|";
                }

                stringBuilder.AppendLine(data.Trim());
            }

            return stringBuilder.ToString().Trim();
        }
    }
}