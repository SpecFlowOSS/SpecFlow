using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Enumerable = System.Linq.Enumerable;

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

            var lines = Enumerable.Select(message.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries), x => Enumerable.ToArray<string>(x.Split('|')));

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

            foreach (var line in message.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
            {
                var data = "";

                var blocks = line.Split('|').ToArray();

                var precedingCharacter = "";
                foreach (var block in blocks.Select((value, index) => new {value, index}))
                {
                    data += string.Format("{1}{0}", block.value.PadRight(dictionary[block.index]), precedingCharacter);
                    precedingCharacter = "|";
                }

                stringBuilder.AppendLine(data.TrimEnd());
            }

            return stringBuilder.ToString();
        }
    }
}